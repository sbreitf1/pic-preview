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

    delegate void ImageReadyHandler(ImageCollection sender);
    delegate void ThumbnailReadyHandler(ImageCollection sender);
    delegate void ImageLoadingErrorHandler(ImageCollection sender, Exception ex);


    class ImageCollection
    {
        HashSet<string> imageExtensions;
        ImageCache cache;

        public string CurrentDirectory { get; private set; }
        public string CurrentFile { get; private set; }
        public string CurrentFileName { get { return Path.GetFileName(this.CurrentFile); } }
        public Image CurrentImage { get; private set; }

        int issueCount = 0;
        Mutex imageMutex = new Mutex();

        /// <summary>
        /// Returns true, when there is currently an image beeing loaded in the background. The CurrentImage object can still be valid.
        /// </summary>
        public bool IsLoading { get; private set; }
        /// <summary>
        /// Returns whether any file is currently selected regardless of the loading state.
        /// </summary>
        public bool IsFileSelected { get { return !string.IsNullOrWhiteSpace(this.CurrentFile); } }
        /// <summary>
        /// Returns whether the CurrentImage object contains a valid image.
        /// </summary>
        public bool HasCurrentImage { get { return this.CurrentImage != null; } }
        /// <summary>
        /// Returns whether the CurrentImage object contains a valid image and no loading process is active.
        /// </summary>
        public bool IsCurrentImageLoaded { get { return this.HasCurrentImage && !this.IsLoading; } }
        /// <summary>
        /// Returns whether navigation is available for the current image location.
        /// </summary>
        public bool CanSwipeImages { get { return !string.IsNullOrWhiteSpace(this.CurrentDirectory) && Directory.Exists(this.CurrentDirectory); } }

        public event ThumbnailReadyHandler ThumbnailReady;
        public event ImageReadyHandler ImageReady;
        public event ImageLoadingErrorHandler ImageLoadingError;


        public ImageCollection()
        {
            this.imageExtensions = new HashSet<string>();
            this.imageExtensions.Add(".bmp");
            this.imageExtensions.Add(".gif");
            this.imageExtensions.Add(".jpeg");
            this.imageExtensions.Add(".jpg");
            this.imageExtensions.Add(".png");
            this.imageExtensions.Add(".tif");
            this.imageExtensions.Add(".tiff");
            this.imageExtensions.Add(".tga");
            this.imageExtensions.Add(".webp");

            this.cache = new ImageCache(128 * 1024 * 1024);
        }


        public LoadImageResults LoadImage(string imageFile)
        {
            // the user might load another image, while the current one is still beein loaded
            // save the counter when the loading began, so outdated results can be omitted when they are ready
            this.imageMutex.WaitOne();
            int issueIndex = ++this.issueCount;
            
            // safe location information first for browsing (left/right)
            try
            {
                this.CurrentFile = imageFile;
                this.CurrentDirectory = Path.GetDirectoryName(this.CurrentFile);
            }
            catch (Exception ex)
            {
                try
                {
                    this.ImageLoadingError?.Invoke(this, ex);
                }
                catch { }
                this.imageMutex.ReleaseMutex();
                return LoadImageResults.Error;
            }

            // disabled cache until reload after change is fixed
            /*Image cachedImg = this.cache.GetImage(imageFile.ToLower());
            if (cachedImg != null)
            {
                try
                {
                    // disposing unused Bitmap objects is important as they most likely introduce memory leaks
                    if (this.CurrentImage != null)
                    {
                        this.CurrentImage.Unload();
                        GC.Collect();
                    }
                }
                catch { }

                this.CurrentImage = cachedImg;
                // maybe another thread has set this value, just override it
                this.IsLoading = false;
                this.imageMutex.ReleaseMutex();

                try
                {
                    // do not call inside of mutex as it might cause a deadlock
                    this.ImageReady?.Invoke(this);
                }
                catch { }

                return LoadImageResults.ImageInCache;
            }*/

            string currentFile = this.CurrentFile;
            this.imageMutex.ReleaseMutex();

            // now try to load the image
            this.IsLoading = true;
            Thread loadThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    Image img = new Image(currentFile);

                    this.imageMutex.WaitOne();
                    // has another image been requested in the meantime?
                    if (this.issueCount == issueIndex)
                    {
                        try
                        {
                            // disposing unused Bitmap objects is important as they most likely introduce memory leaks
                            if (this.CurrentImage != null)
                            {
                                this.CurrentImage.Unload();
                                GC.Collect();
                            }
                        }
                        catch { }

                        try
                        {
                            this.cache.StoreImage(currentFile.ToLower(), img);
                        }
                        catch { }

                        this.CurrentImage = img;
                        this.IsLoading = false;
                        this.imageMutex.ReleaseMutex();

                        try
                        {
                            // do not call inside of mutex as it might cause a deadlock
                            this.ImageReady?.Invoke(this);
                        }
                        catch { }
                    }
                    else
                        this.imageMutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    try
                    {
                        if (this.CurrentImage != null)
                        {
                            this.CurrentImage.Unload();
                            this.CurrentImage = null;
                        }
                    }
                    catch { }

                    this.IsLoading = false;
                    this.ImageLoadingError?.Invoke(this, ex);
                }
            }));
            loadThread.IsBackground = true;
            loadThread.Start();
            return LoadImageResults.AsyncLoadingStarted;
        }


        private string[] GetImageFiles(string dir)
        {
            string[] rawFiles = Directory.GetFiles(dir);
            List<string> files = new List<string>(rawFiles.Length);
            foreach (string rawFile in rawFiles)
                if (IsImageFile(rawFile))
                    files.Add(rawFile);
            return files.ToArray();
        }

        private bool IsImageFile(string file)
        {
            string ext = Path.GetExtension(file).ToLower();
            return this.imageExtensions.Contains(ext);
        }

        private int FindFileName(string file, string[] files)
        {
            for (int i = 0; i < files.Length; i++)
                if (files[i].Equals(file, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            return -1;
        }

        public string GetNextImage()
        {
            string[] files = GetImageFiles(this.CurrentDirectory);
            if (files.Length <= 1)
                return null;
            int index = FindFileName(this.CurrentFile, files);
            return files[(index + 1) % files.Length];
        }

        public string GetPreviousImage()
        {
            string[] files = GetImageFiles(this.CurrentDirectory);
            if (files.Length <= 1)
                return null;
            int index = FindFileName(this.CurrentFile, files);
            return files[(index + files.Length - 1) % files.Length];
        }
    }
}
