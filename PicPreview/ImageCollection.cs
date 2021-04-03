using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace PicPreview
{
    enum LoadImageResults
    {
        Error = 0,
        AsyncLoadingStarted = 1,
        ImageInCache = 2
    }

    delegate void ImageReadyHandler(ImageCollection sender, FilePath file, Image img);
    delegate void ImageLoadingErrorHandler(ImageCollection sender, FilePath file, Exception ex);


    class ImageCollection
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int maxNavDist = 0;
        private int maxLoadedImages = 5; //TODO compute from maxNavDist
        private int numBackgroundWorkers = 4;
        private static HashSet<string> imageExtensions;

        public FilePath CurrentDirectory { get; private set; }

        // list of images in current directory for navigation
        private OrderedMap<FilePath> knownImages;

        private object imagesLock = new object();
        private List<FilePath> issuedImages;
        private HashSet<FilePath> loadingImages;
        private Dictionary<FilePath, Image> loadedImages;

        private Thread[] backgroundImageLoaders;

        private object currentImageLock = new object();
        private FilePath currentFile;

        /// <summary>
        /// Returns whether navigation is available for the current image location.
        /// </summary>
        public bool CanSwipeImages { get { return !this.CurrentDirectory.IsEmpty && Directory.Exists(this.CurrentDirectory); } }

        public event ImageReadyHandler ImageReady;
        public event ImageLoadingErrorHandler ImageLoadingError;

        static ImageCollection()
        {
            imageExtensions = new HashSet<string>();
            imageExtensions.Add(".bmp");
            imageExtensions.Add(".gif");
            imageExtensions.Add(".jpeg");
            imageExtensions.Add(".jpg");
            imageExtensions.Add(".png");
            imageExtensions.Add(".tif");
            imageExtensions.Add(".tiff");
            imageExtensions.Add(".tga");
            imageExtensions.Add(".webp");
        }

        public ImageCollection()
        {
            this.knownImages = new OrderedMap<FilePath>();
            this.issuedImages = new List<FilePath>();
            this.loadingImages = new HashSet<FilePath>();
            this.loadedImages = new Dictionary<FilePath, Image>();
            this.currentFile = null;

            this.backgroundImageLoaders = new Thread[this.numBackgroundWorkers];
            for (int i = 0; i < this.backgroundImageLoaders.Length; i++)
            {
                this.backgroundImageLoaders[i] = new Thread(BackgroundImageLoaderLoop);
                this.backgroundImageLoaders[i].IsBackground = true;
                this.backgroundImageLoaders[i].Start();
            }
            logger.Info("Started " + this.numBackgroundWorkers + " background worker(s)");
        }


        public LoadImageResults LoadImage(FilePath path)
        {
            lock (this.knownImages)
            {
                if (this.CurrentDirectory == null || this.CurrentDirectory != path.Dir)
                {
                    logger.Info("Enter new directory '" + path.Dir + "'");
                    // entering another directory, reload known files for navigation
                    FilePath dir = path.Dir;
                    FilePath[] newKnownFiles = GetImageFiles(dir);
                    lock (this.knownImages)
                    {
                        this.CurrentDirectory = dir;
                        this.knownImages.Clear();
                        foreach (FilePath file in newKnownFiles)
                            this.knownImages.Add(file);
                    }
                }

                lock (this.imagesLock)
                {
                    // image is not cached, request loading if not already done
                    if (!this.loadedImages.ContainsKey(path) && !this.issuedImages.Contains(path) && !this.loadingImages.Contains(path))
                    {
                        // request loading
                        this.issuedImages.Add(path);
                    }

                    lock (this.currentImageLock)
                    {
                        // update currently selected image
                        this.currentFile = path;
                        RequestPreCacheOfSurroundingImages();
                    }

                    // lookup image in cached images
                    if (this.loadedImages.TryGetValue(path, out Image img))
                    {
                        this.ImageReady?.Invoke(this, path, img);
                        // set current image from cache
                        return LoadImageResults.ImageInCache;
                    }

                    return LoadImageResults.AsyncLoadingStarted;
                }
            }
        }

        private void RequestPreCacheOfSurroundingImages()
        {
            if (this.knownImages.TryGetIndex(this.currentFile, out int index))
            {
                if (this.knownImages.Count <= this.maxLoadedImages)
                {
                    // all images of directory fit into cache
                    for (int i = 0; i < this.knownImages.Count; i++)
                    {
                        FilePath path = this.knownImages.Get(i);
                        if (!this.issuedImages.Contains(path) && !this.loadingImages.Contains(path) && !this.loadedImages.ContainsKey(path))
                        {
                            // request pre-cache loading
                            this.issuedImages.Add(path);
                        }
                    }
                }
                else
                {
                    // only pre-cache a small range around current image
                    int maxDist = (this.maxLoadedImages - 1) / 2;
                    for (int i = -maxDist; i <= maxDist; i++)
                    {
                        FilePath path = this.knownImages.Get((this.knownImages.Count + index + i) % this.knownImages.Count);
                        if (!this.issuedImages.Contains(path) && !this.loadingImages.Contains(path) && !this.loadedImages.ContainsKey(path))
                        {
                            // request pre-cache loading
                            this.issuedImages.Add(path);
                        }
                    }
                }
            }
        }

        public void ForceReloadCurrentImage()
        {
            lock (this.imagesLock)
            {
                lock (this.currentImageLock)
                {
                    InnerForceReloadCurrentImage();
                }
            }
        }

        private void InnerForceReloadCurrentImage()
        {
            //TODO reload current image
        }

        public void InvalidateCache(FilePath file)
        {
            lock (this.imagesLock)
            {
                lock (this.currentImageLock)
                {
                    if (this.loadedImages.TryGetValue(file, out Image image))
                    {
                        if (file == this.currentFile)
                        {
                            ForceReloadCurrentImage();
                        }
                        else
                        {
                            this.loadedImages[file].Dispose();
                            this.loadedImages.Remove(file);
                            if (!this.issuedImages.Contains(file) && !this.loadingImages.Contains(file))
                            {
                                this.issuedImages.Add(file);
                            }
                        }
                    }
                }
            }
        }

        private static FilePath[] GetImageFiles(FilePath dir)
        {
            string[] rawFiles = Directory.GetFiles(dir);
            List<FilePath> files = new List<FilePath>(rawFiles.Length);
            foreach (string rawFile in rawFiles)
                if (IsImageFile(rawFile))
                    files.Add(rawFile);
            return files.ToArray();
        }

        private static bool IsImageFile(FilePath file)
        {
            string ext = file.Extension.ToLower();
            return imageExtensions.Contains(ext);
        }

        public FilePath GetNextImage()
        {
            lock (this.knownImages)
            {
                //TODO update file index on not found
                if (this.knownImages.TryGetIndex(this.currentFile, out int index))
                {
                    return this.knownImages.Get((index + 1) % this.knownImages.Count);
                }
                //TODO handle list is empty
                return this.knownImages.Get(0);
            }
        }

        public FilePath GetPreviousImage()
        {
            lock (this.knownImages)
            {
                //TODO update file index on not found
                if (this.knownImages.TryGetIndex(this.currentFile, out int index))
                {
                    return this.knownImages.Get((index + this.knownImages.Count - 1) % this.knownImages.Count);
                }
                //TODO handle list is empty
                return this.knownImages.Get(0);
            }
        }

        private int ComputeNavigationDistance(FilePath path1, FilePath path2)
        {
            int index1, index2;
            if (!this.knownImages.TryGetIndex(path1, out index1))
            {
                return int.MaxValue;
            }
            if (!this.knownImages.TryGetIndex(path2, out index2))
            {
                return int.MaxValue;
            }
            int directDist = Math.Abs(index1 - index2);
            int cyclicDist1 = index1 + (this.knownImages.Count - index2);
            int cyclicDist2 = index2 + (this.knownImages.Count - index1);
            return Math.Min(directDist, Math.Min(cyclicDist1, cyclicDist2));
        }


        private void BackgroundImageLoaderLoop()
        {
            while (true)
            {
                FilePath loadPath = null;
                lock (this.imagesLock)
                {
                    if (this.issuedImages.Count > 0)
                    {
                        // take oldest requested image from stack
                        loadPath = this.issuedImages[0];
                        this.issuedImages.RemoveAt(0);
                        if (this.loadingImages.Contains(loadPath) || this.loadedImages.ContainsKey(loadPath))
                        {
                            // image has been loaded in the meantime, skip:
                            loadPath = null;
                        }
                        else
                        {
                            lock (this.currentImageLock)
                            {
                                int dist = ComputeNavigationDistance(loadPath, this.currentFile);
                                if (dist <= maxNavDist)
                                {
                                    // image still needs to be loaded
                                    this.loadingImages.Add(loadPath);
                                }
                                else
                                {
                                    // the navigation has changed in the meantime, no need to cache this image anymore
                                    logger.Debug("Skipped caching of '" + loadPath + "' due to navigation change");
                                    loadPath = null;
                                }
                            }
                        }
                    }
                }

                if (loadPath != null)
                {
                    // load new image
                    try
                    {
                        // try to load image
                        Image img = new Image(loadPath);

                        lock (this.imagesLock)
                        {
                            // success! update image in loaded images list
                            this.loadingImages.Remove(loadPath);
                            this.loadedImages.Add(loadPath, img);
                        }

                        // inform observers
                        this.ImageReady?.Invoke(this, loadPath, img);
                    }
                    catch (Exception ex)
                    {
                        lock (this.imagesLock)
                        {
                            // only take image from processing list in case of error
                            this.loadingImages.Remove(loadPath);
                        }

                        if (ex is ThreadAbortException)
                            throw;

                        logger.Error(ex.GetType().Name + " while loading image: " + ex.Message);

                        // inform observers
                        this.ImageLoadingError?.Invoke(this, loadPath, ex);
                    }
                }

                // detect images in cache that can be deleted
                lock (this.knownImages)
                {
                    lock (this.imagesLock)
                    {
                        if (this.loadedImages.Count > this.maxLoadedImages)
                        {
                            List<FilePath> removeItems = new List<FilePath>();
                            lock (this.currentImageLock)
                            {
                                foreach (KeyValuePair<FilePath, Image> kvp in this.loadedImages)
                                {
                                    int dist = ComputeNavigationDistance(kvp.Key, this.currentFile);
                                    if (dist > maxNavDist)
                                        removeItems.Add(kvp.Key);
                                }
                            }

                            foreach (FilePath removePath in removeItems)
                            {
                                this.loadedImages[removePath].Dispose();
                                this.loadedImages.Remove(removePath);
                                logger.Debug("Removed '" + removePath + "' from cache");
                            }
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}
