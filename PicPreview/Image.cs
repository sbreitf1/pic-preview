using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace PicPreview
{
    delegate void FrameChangedEventHandler(Image sender, bool isUserChange);

    class Image : IDisposable
    {
        #region Basic Image Loading and Disposal
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public FilePath SourceFile { get; private set; }
        protected Bitmap bitmap;
        public Bitmap Bitmap { get { return this.bitmap; } }
        protected int width;
        public int Width { get { return this.width; } }
        protected int height;
        public int Height { get { return this.height; } }

        protected bool hasAlphaChannel;
        public bool HasAlphaChannel { get { return this.hasAlphaChannel; } }

        private enum ImageLoader
        {
            Unknown,
            Default,
            DefaultGif,
            TGA,
            WEBP
        }

        public Image(FilePath file)
        {
            this.SourceFile = file;

            ImageLoader loader = GetImageLoaderFromFileContent(file);
            if (loader == ImageLoader.Unknown)
            {
                loader = GetImageLoaderFromExtension(file.Extension);
                logger.Debug("Detected image loader '" + loader + "' from extension");
            }
            else
            {
                logger.Debug("Detected image loader '" + loader + "' from file content");
            }
            switch (loader)
            {
                case ImageLoader.WEBP:
                    WebP webp = new WebP();
                    this.bitmap = webp.Load(file);
                    break;

                case ImageLoader.TGA:
                    Paloma.TargaImage img = new Paloma.TargaImage(file);
                    this.bitmap = img.Image;
                    //TODO dispose tga image?
                    //img.Dispose();
                    break;

                case ImageLoader.DefaultGif:
                    // animations seem to be read from streams on-the-fly, thus the stream should be managed by the bitmap object
                    this.bitmap = (Bitmap)Bitmap.FromFile(file);
                    break;

                case ImageLoader.Default:
                    // read static images from streamy to force closing the file
                    using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        this.bitmap = (Bitmap)Bitmap.FromStream(stream);
                        // force pre-loading
                        int[] list = this.bitmap.PropertyIdList;
                    }
                    break;

                default:
                    throw new Exception("Image format not recognized");
            }

            NormalizeImageRotation();
            this.width = this.bitmap.Width;
            this.height = this.bitmap.Height;

            logger.Debug("Meta data for image '" + file + "': " + this.width + "x" + this.height + " [" + this.bitmap.PixelFormat + "]");
            switch (this.bitmap.PixelFormat)
            {
                case PixelFormat.Alpha:
                case PixelFormat.Canonical:
                case PixelFormat.Format16bppArgb1555:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                case PixelFormat.Format64bppArgb:
                case PixelFormat.Format64bppPArgb:
                case PixelFormat.PAlpha:
                    this.hasAlphaChannel = true;
                    break;

                default:
                    this.hasAlphaChannel = false;
                    break;
            }

            ReadAnimation();
        }

        private static ImageLoader GetImageLoaderFromFileContent(FilePath file)
        {
            try
            {
                // read sufficient part of file header
                byte[] buffer = new byte[64];
                int len;
                using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    len = stream.Read(buffer, 0, buffer.Length);
                }

                // see https://en.wikipedia.org/wiki/List_of_file_signatures

                // WEBP header (RIFF....WEBP)
                if (len >= 12 && buffer[0] == 'R' && buffer[1] == 'I' && buffer[2] == 'F' && buffer[3] == 'F' && buffer[8] == 'W' && buffer[9] == 'E' && buffer[10] == 'B' && buffer[11] == 'P')
                {
                    return ImageLoader.WEBP;
                }

                // JPEG header (ÿØÿÛ  /  ÿØÿà..JFIF..  /  ÿØÿî  /  ÿØÿá..Exif..)
                if (len >= 3 && buffer[0] == 'ÿ' && buffer[1] == 'Ø' && buffer[2] == 'ÿ')
                {
                    return ImageLoader.Default;
                }

                // PNG header (.PNG....)
                if (len >= 4 && buffer[1] == 'P' && buffer[2] == 'N' && buffer[3] == 'G')
                {
                    return ImageLoader.Default;
                }

                // GIF header (GIF87a  /  GIF89a)
                if (len >= 4 && buffer[0] == 'G' && buffer[1] == 'I' && buffer[2] == 'F' && buffer[3] == '8')
                {
                    return ImageLoader.DefaultGif;
                }

                // BMP header (BM)
                if (len >= 2 && buffer[0] == 'B' && buffer[1] == 'M')
                {
                    return ImageLoader.Default;
                }
            }
            catch { }

            // default case
            return ImageLoader.Unknown;
        }

        private static ImageLoader GetImageLoaderFromExtension(string extension)
        {
            switch (extension.ToLower())
            {
                case ".webp": return ImageLoader.WEBP;
                case ".tga": return ImageLoader.TGA;
                case ".gif": return ImageLoader.DefaultGif;
                default: return ImageLoader.Default;
            }
        }

        public void Dispose()
        {
            if (this.bitmap != null)
            {
                this.bitmap.Dispose();
                this.bitmap = null;
            }
        }

        public void Unload()
        {
            if (this.hasAnimation)
                this.StopAnimation();
            this.FrameChanged = null;
        }
        #endregion

        #region Rotation Correction
        protected void NormalizeImageRotation()
        {
            try
            {
                bool found = false;
                foreach (int propId in this.bitmap.PropertyIdList)
                {
                    if (propId == 0x0112)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                    return;

                PropertyItem propItem = this.bitmap.GetPropertyItem(0x0112);
                RotateFlipType rotFlip = GetImageRotation(propItem.Value[0]);
                if (rotFlip != RotateFlipType.RotateNoneFlipNone)
                {
                    logger.Debug("Rotate image '" + this.SourceFile + "': " + rotFlip);
                    this.bitmap.RotateFlip(rotFlip);
                    this.bitmap.RemovePropertyItem(0x0112);
                }
            }
            catch { }
        }

        protected RotateFlipType GetImageRotation(int orientation)
        {
            switch (orientation)
            {
                case 1: return RotateFlipType.RotateNoneFlipNone;
                case 2: return RotateFlipType.RotateNoneFlipX;
                case 3: return RotateFlipType.Rotate180FlipNone;
                case 4: return RotateFlipType.Rotate180FlipX;
                case 5: return RotateFlipType.Rotate90FlipX;
                case 6: return RotateFlipType.Rotate90FlipNone;
                case 7: return RotateFlipType.Rotate270FlipX;
                case 8: return RotateFlipType.Rotate270FlipNone;
                default: return RotateFlipType.RotateNoneFlipNone;
            }
        }
        #endregion

        #region Animation
        const int DefaultFrameDelay = 50;


        protected bool hasAnimation;
        public bool HasAnimation { get { return this.hasAnimation; } }

        protected Thread animationThread;
        protected int[] frameDelays;
        protected int nextFrameTick;
        protected int currentFrame;
        public int CurrentFrame
        {
            get
            {
                if (!this.hasAnimation)
                    throw new InvalidOperationException();

                return Math.Max(0, this.currentFrame) % this.frameDelays.Length;
            }
            set
            {
                if (!this.hasAnimation)
                    throw new InvalidOperationException();

                SelectActiveFrame(value, true);
            }
        }
        public int FrameCount
        {
            get
            {
                if (!this.hasAnimation)
                    throw new InvalidOperationException();

                return this.frameDelays.Length;
            }
        }
        protected bool animationPaused;
        public bool AnimationPaused
        {
            get
            {
                if (!this.hasAnimation)
                    throw new InvalidOperationException();

                return this.animationPaused;
            }
        }

        public event FrameChangedEventHandler FrameChanged;


        protected void ReadAnimation()
        {
            try
            {
                bool found = false;
                foreach (Guid dimGuid in this.bitmap.FrameDimensionsList)
                {
                    if (dimGuid == FrameDimension.Time.Guid)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this.hasAnimation = false;
                    return;
                }

                //TODO check if frame dimension exists
                this.frameDelays = new int[this.bitmap.GetFrameCount(FrameDimension.Time)];
                this.hasAnimation = (this.frameDelays.Length > 1);
                if (this.hasAnimation)
                {
                    PropertyItem propItem = this.bitmap.GetPropertyItem(0x5100);
                    for (int i = 0; i < this.frameDelays.Length; i++)
                    {
                        // frame delays are encoded in the binary data in 1/100 seconds as 32-bit integers (factor 4 to the indices)
                        // the delays are processed as milliseconds => factor 10 for the result
                        frameDelays[i] = (propItem.Value.Length >= (i * 4 + 4) ? BitConverter.ToInt32(propItem.Value, i * 4) * 10 : 0);
                        // frame delay could not be loaded? use default interval instead
                        if (frameDelays[i] <= 0)
                            frameDelays[i] = DefaultFrameDelay;
                    }
                    SelectActiveFrame(0, false);
                    this.animationPaused = true;
                }
            }
            catch
            {
                this.hasAnimation = false;
            }
        }

        public void StartAnimation()
        {
            if (!this.hasAnimation)
                throw new InvalidOperationException();

            this.animationPaused = false;
            if (this.animationThread == null)
            {
                this.animationThread = new Thread(new ThreadStart(AnimationLoop));
                this.animationThread.IsBackground = true;
                this.animationThread.Start();
            }
        }

        public void StopAnimation()
        {
            if (!this.hasAnimation)
                throw new InvalidOperationException();

            this.animationPaused = true;
            this.animationThread.Abort();
            this.animationThread = null;
        }

        public void PauseAnimation()
        {
            if (!this.hasAnimation)
                throw new InvalidOperationException();

            this.animationPaused = true;
        }

        protected void SelectActiveFrame(int frameIndex, bool isUserChange)
        {
            if (frameIndex < 0)
                throw new ArgumentException("frameIndex cannot be negative");

            lock (this.bitmap)
            {
                this.currentFrame = (frameIndex % this.frameDelays.Length);
                this.bitmap.SelectActiveFrame(FrameDimension.Time, this.currentFrame);
                this.nextFrameTick = (Environment.TickCount + frameDelays[this.currentFrame]);
            }

            if (this.FrameChanged != null)
                this.FrameChanged(this, isUserChange);
        }

        protected void AnimationLoop()
        {
            try
            {
                while (this.bitmap != null)
                {
                    if (!this.animationPaused && (Environment.TickCount >= this.nextFrameTick))
                    {
                        SelectActiveFrame(this.currentFrame + 1, false);
                    }
                    else
                        Thread.Sleep(10);
                }
            }
            catch (ThreadAbortException) { }
            catch { }
        }
        #endregion
    }
}
