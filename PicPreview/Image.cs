using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;

namespace PicPreview
{
    delegate void RequestRedrawEventHandler(Image sender);


    class Image : IDisposable
    {
        protected Bitmap bitmap;
        public Bitmap Bitmap { get { return this.bitmap; } }
        protected int width;
        public int Width { get { return this.width; } }
        protected int height;
        public int Height { get { return this.height; } }



        public Image(string file)
        {
            string extension = Path.GetExtension(file);
            switch (extension.ToLower())
            {
                default:
                    this.bitmap = (Bitmap)Bitmap.FromFile(file);
                    break;
            }

            this.width = this.bitmap.Width;
            this.height = this.bitmap.Height;

            ReadAnimation();
        }


        public void Dispose()
        {
            if (this.bitmap != null)
            {
                this.bitmap.Dispose();
                this.bitmap = null;
            }
        }


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

                SelectActiveFrame(value);
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

        public event RequestRedrawEventHandler RequestRedraw;


        protected void ReadAnimation()
        {
            try
            {
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
                    SelectActiveFrame(0);
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

        public void PauseAnimation()
        {
            if (!this.hasAnimation)
                throw new InvalidOperationException();

            this.animationPaused = true;
        }

        protected void SelectActiveFrame(int frameIndex)
        {
            if (frameIndex < 0)
                throw new ArgumentException("frameIndex cannot be negative");

            lock (this.bitmap)
            {
                this.currentFrame = (frameIndex % this.frameDelays.Length);
                this.bitmap.SelectActiveFrame(FrameDimension.Time, this.currentFrame);
                this.nextFrameTick = (Environment.TickCount + frameDelays[this.currentFrame]);
            }

            if (this.RequestRedraw != null)
                this.RequestRedraw(this);
        }

        protected void AnimationLoop()
        {
            try
            {
                while (this.bitmap != null)
                {
                    if (!this.animationPaused && (Environment.TickCount >= this.nextFrameTick))
                    {
                        SelectActiveFrame(this.currentFrame + 1);
                    }
                    else
                        Thread.Sleep(10);
                }
            }
            catch { }
        }
        #endregion
    }
}
