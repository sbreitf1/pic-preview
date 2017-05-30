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
        ImageCache cache;

        private string currentDirectory;
        public string CurrentDirectory { get { return this.currentDirectory; } }
        private string currentFile;
        public string CurrentFile { get { return this.currentFile; } }
        public string CurrentFileName { get { return Path.GetFileName(this.currentFile); } }
        private Image currentImage;
        public Image CurrentImage { get { return this.currentImage; } }
        
        int issueCount = 0;
        Mutex imageMutex = new Mutex();

        private bool isLoading;
        public bool IsLoading { get { return this.isLoading; } }
        public bool IsFileSelected { get { return !string.IsNullOrWhiteSpace(this.currentFile); } }
        public bool IsImageLoaded { get { return this.currentImage != null; } }
        public bool CanSwipeImages { get { return !string.IsNullOrWhiteSpace(this.currentDirectory) && Directory.Exists(this.currentDirectory); } }

        HashSet<string> imageExtensions;

        public event ThumbnailReadyHandler ThumbnailReady;
        public event ImageReadyHandler ImageReady;
        public event ImageLoadingErrorHandler ImageLoadingError;


        public ImageCollection()
        {
            this.imageExtensions = new HashSet<string>();
            this.imageExtensions.Add(".jpg");
            this.imageExtensions.Add(".jpeg");
            this.imageExtensions.Add(".png");
            this.imageExtensions.Add(".bmp");
            this.imageExtensions.Add(".gif");
            this.imageExtensions.Add(".tiff");

            this.cache = new ImageCache(128 * 1024 * 1024);
        }


        public LoadImageResults LoadImage(string imageFile)
        {
            // the user might load another image, while the current one is still beein loaded
            // save the counter when the loading began, so outdated results can be omitted when they are ready
            this.imageMutex.WaitOne();
            int issueIndex = ++this.issueCount;

            try
            {
                // disposing unused Bitmap objects is important as they most likely introduce memory leaks
                if (this.currentImage != null)
                {
                    this.currentImage.Unload();
                }
            }
            catch { }

            // safe location information first for browsing (left/right)
            try
            {
                this.currentFile = imageFile;
                this.currentDirectory = Path.GetDirectoryName(this.currentFile);
            }
            catch (Exception ex)
            {
                try
                {
                    if (this.ImageLoadingError != null)
                        this.ImageLoadingError(this, ex);
                }
                catch { }
                this.imageMutex.ReleaseMutex();
                return LoadImageResults.Error;
            }

            Image cachedImg = this.cache.GetImage(imageFile.ToLower());
            if (cachedImg != null)
            {
                this.currentImage = cachedImg;
                // maybe another thread has set this value, just override it
                this.isLoading = false;
                this.imageMutex.ReleaseMutex();

                try
                {
                    // do not call inside of mutex as it might cause a deadlock
                    if (this.ImageReady != null)
                        this.ImageReady(this);
                }
                catch { }
                
                return LoadImageResults.ImageInCache;
            }

            string currentFile = this.currentFile;
            this.imageMutex.ReleaseMutex();

            // now try to load the image
            this.isLoading = true;
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
                            this.cache.StoreImage(currentFile.ToLower(), img);
                        }
                        catch { }

                        this.currentImage = img;
                        this.isLoading = false;
                        this.imageMutex.ReleaseMutex();

                        try
                        {
                            // do not call inside of mutex as it might cause a deadlock
                            if (this.ImageReady != null)
                                this.ImageReady(this);
                        }
                        catch { }
                    }
                    else
                        this.imageMutex.ReleaseMutex();
                }
                catch (Exception ex)
                {
                    if (this.ImageLoadingError != null)
                        this.ImageLoadingError(this, ex);
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
            string[] files = GetImageFiles(this.currentDirectory);
            if (files.Length <= 1)
                return null;
            int index = FindFileName(this.currentFile, files);
            return files[(index + 1) % files.Length];
        }

        public string GetPreviousImage()
        {
            string[] files = GetImageFiles(this.currentDirectory);
            if (files.Length <= 1)
                return null;
            int index = FindFileName(this.currentFile, files);
            return files[(index + files.Length - 1) % files.Length];
        }
    }
}
