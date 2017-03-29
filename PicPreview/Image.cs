﻿using System;
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

            NormalizeImageRotation();
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

                //TODO check for frame dimension first
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
            catch { }
        }
        #endregion
    }
}
