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
    
    delegate void ImageReadyHandler(ImageCollection sender, string file, Image img);
    delegate void ImageLoadingErrorHandler(ImageCollection sender, string file, Exception ex);


    class ImageCollection
    {
        private int maxLoadedImages = 5;
        private int numBackgroundWorkers = 2;
        private static HashSet<string> imageExtensions;

        public string CurrentDirectory { get; private set; }

        // list of images in current directory for navigation
        private OrderedMap<string> knownImages;

        private object imagesLock = new object();
        private List<string> issuedImages;
        private HashSet<string> loadingImages;
        private Dictionary<string, Image> loadedImages;

        private Thread[] backgroundImageLoaders;

        private object currentImageLock = new object();
        private string currentFile;

        /// <summary>
        /// Returns whether navigation is available for the current image location.
        /// </summary>
        public bool CanSwipeImages { get { return !string.IsNullOrWhiteSpace(this.CurrentDirectory) && Directory.Exists(this.CurrentDirectory); } }

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
            this.knownImages = new OrderedMap<string>();
            this.issuedImages = new List<string>();
            this.loadingImages = new HashSet<string>();
            this.loadedImages = new Dictionary<string, Image>();
            this.currentFile = null;

            this.backgroundImageLoaders = new Thread[this.numBackgroundWorkers];
            for (int i = 0; i < this.backgroundImageLoaders.Length; i++)
            {
                this.backgroundImageLoaders[i] = new Thread(BackgroundImageLoaderLoop);
                this.backgroundImageLoaders[i].IsBackground = true;
                this.backgroundImageLoaders[i].Start();
            }
        }


        public LoadImageResults LoadImage(string path)
        {
            lock (this.knownImages)
            {
                if (this.CurrentDirectory == null || this.CurrentDirectory.ToLower() != Path.GetDirectoryName(path).ToLower())
                {
                    // entering another directory, reload known files for navigation
                    string dir = Path.GetDirectoryName(path);
                    string[] newKnownFiles = GetImageFiles(dir);
                    lock (this.knownImages)
                    {
                        this.CurrentDirectory = dir;
                        this.knownImages.Clear();
                        foreach (string file in newKnownFiles)
                            this.knownImages.Add(file.ToLower());
                    }
                }

                lock (this.imagesLock)
                {
                    lock(this.currentImageLock)
                    {
                        // update currently selected image
                        this.currentFile = path;
                        RequestPreCacheOfSurroundingImages();
                    }

                    // lookup image in cached images
                    if (this.loadedImages.TryGetValue(path.ToLower(), out Image img))
                    {
                        this.ImageReady?.Invoke(this, path.ToLower(), img);
                        // set current image from cache
                        return LoadImageResults.ImageInCache;
                    }
                    else
                    {
                        // image is not cached, request loading if not already done
                        if (!this.issuedImages.Contains(path.ToLower()) && !this.loadingImages.Contains(path.ToLower()))
                        {
                            // request loading
                            this.issuedImages.Add(path.ToLower());
                        }
                        return LoadImageResults.AsyncLoadingStarted;
                    }
                }
            }
        }

        private void RequestPreCacheOfSurroundingImages()
        {
            if (this.knownImages.TryGetIndex(this.currentFile.ToLower(), out int index))
            {
                if (this.knownImages.Count <= this.maxLoadedImages)
                {
                    // all images of directory fit into cache
                    for (int i = 0; i < this.knownImages.Count; i++)
                    {
                        string path = this.knownImages.Get(i);
                        if (!this.issuedImages.Contains(path.ToLower()) && !this.loadingImages.Contains(path.ToLower()) && !this.loadedImages.ContainsKey(path.ToLower()))
                        {
                            // request pre-cache loading
                            this.issuedImages.Add(path.ToLower());
                        }
                    }
                }
                else
                {
                    // only pre-cache a small range around current image
                    int maxDist = (this.maxLoadedImages - 1) / 2;
                    for (int i = -maxDist; i <= maxDist; i++)
                    {
                        string path = this.knownImages.Get((this.knownImages.Count + index + i) % this.knownImages.Count);
                        if (!this.issuedImages.Contains(path.ToLower()) && !this.loadingImages.Contains(path.ToLower()) && !this.loadedImages.ContainsKey(path.ToLower()))
                        {
                            // request pre-cache loading
                            this.issuedImages.Add(path.ToLower());
                        }
                    }
                }
            }
        }

        private static string[] GetImageFiles(string dir)
        {
            string[] rawFiles = Directory.GetFiles(dir);
            List<string> files = new List<string>(rawFiles.Length);
            foreach (string rawFile in rawFiles)
                if (IsImageFile(rawFile))
                    files.Add(rawFile);
            return files.ToArray();
        }

        private static bool IsImageFile(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            return imageExtensions.Contains(ext);
        }

        public string GetNextImage()
        {
            lock (this.knownImages)
            {
                //TODO update file index on not found
                if (this.knownImages.TryGetIndex(this.currentFile.ToLower(), out int index))
                {
                    return this.knownImages.Get((index + 1) % this.knownImages.Count);
                }
                //TODO handle list is empty
                return this.knownImages.Get(0);
            }
        }

        public string GetPreviousImage()
        {
            lock (this.knownImages)
            {
                //TODO update file index on not found
                if (this.knownImages.TryGetIndex(this.currentFile.ToLower(), out int index))
                {
                    return this.knownImages.Get((index + this.knownImages.Count - 1) % this.knownImages.Count);
                }
                //TODO handle list is empty
                return this.knownImages.Get(0);
            }
        }

        private int ComputeNavigationDistance(string path1, string path2)
        {
            int index1, index2;
            if (!this.knownImages.TryGetIndex(path1.ToLower(), out index1))
            {
                return int.MaxValue;
            }
            if (!this.knownImages.TryGetIndex(path2.ToLower(), out index2))
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
                string loadPath = null;
                lock (this.imagesLock)
                {
                    if (this.issuedImages.Count > 0)
                    {
                        // take oldest requested image from stack
                        loadPath = this.issuedImages[this.issuedImages.Count - 1];
                        this.issuedImages.RemoveAt(this.issuedImages.Count - 1);
                        if (this.loadingImages.Contains(loadPath.ToLower()) || this.loadedImages.ContainsKey(loadPath.ToLower()))
                        {
                            // image has been loaded in the meantime, skip:
                            loadPath = null;
                        }
                        else
                        {
                            // image still needs to be loaded
                            this.loadingImages.Add(loadPath.ToLower());
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
                            this.loadingImages.Remove(loadPath.ToLower());
                            this.loadedImages.Add(loadPath.ToLower(), img);
                        }

                        // inform observers
                        this.ImageReady?.Invoke(this, loadPath, img);
                    }
                    catch (Exception ex)
                    {
                        lock (this.imagesLock)
                        {
                            // only take image from processing list in case of error
                            this.loadingImages.Remove(loadPath.ToLower());
                        }

                        if (ex is ThreadAbortException)
                            throw;

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
                            List<string> removeItems = new List<string>();
                            lock (this.currentImageLock)
                            {
                                int maxDist = (this.maxLoadedImages - 1) / 2;
                                foreach (KeyValuePair<string, Image> kvp in this.loadedImages)
                                {
                                    int dist = ComputeNavigationDistance(kvp.Key, this.currentFile);
                                    if (dist > maxDist)
                                        removeItems.Add(kvp.Key);
                                }
                            }

                            foreach(string removePath in removeItems)
                            {
                                this.loadedImages[removePath].Dispose();
                                this.loadedImages.Remove(removePath);
                            }
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}
