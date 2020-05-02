using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PicPreview
{
    public partial class MainForm : Form
    {
        #region Initialization and Disposal
        public MainForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.pic_preview;
            this.MouseWheel += MainForm_MouseWheel;

            this.textBrush = Brushes.Black;
            this.textFont = new Font(this.Font.FontFamily, this.Font.Size * 2);

            this.imageCollection = new ImageCollection();
            this.imageCollection.ImageReady += ImageCollection_ImageReady;
            this.imageCollection.ImageLoadingError += ImageCollection_ImageLoadingError;

            //FileAssociation.Associate(".jpg");
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.WindowX > -1000000 && Properties.Settings.Default.WindowY > -1000000 && Properties.Settings.Default.WindowWidth > 0 && Properties.Settings.Default.WindowHeight > 0)
            {
                //TODO check if in screen
                this.Left = Properties.Settings.Default.WindowX;
                this.Top = Properties.Settings.Default.WindowY;
                this.Width = Properties.Settings.Default.WindowWidth;
                this.Height = Properties.Settings.Default.WindowHeight;
            }
            if (Properties.Settings.Default.WindowMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            if (Properties.Settings.Default.CheckFileAssociations)
            {
                //TODO check file associations and show window

                Properties.Settings.Default.CheckFileAssociations = false;
                Properties.Settings.Default.Save();
            }

            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                LoadImage(args[1]);
            else
                UpdateControlStates();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.WindowMaximized = true;
            }
            else if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowMaximized = false;
                Properties.Settings.Default.WindowX = this.Left;
                Properties.Settings.Default.WindowY = this.Top;
                Properties.Settings.Default.WindowWidth = this.Width;
                Properties.Settings.Default.WindowHeight = this.Height;
            }
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Generic UI
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    LoadPreviousImage();
                    return true;
                case Keys.Right:
                    LoadNextImage();
                    return true;
                case Keys.Space:
                    btnAnimation.PerformClick();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void tsbZoomModeFit_ButtonClick(object sender, EventArgs e)
        {
            SetZoomMode(ImageZoomModes.Fill);
        }

        private void tsbZoomModeOriginal_ButtonClick(object sender, EventArgs e)
        {
            SetZoomMode(ImageZoomModes.Original);
        }


        private void tsbNext_ButtonClick(object sender, EventArgs e)
        {
            LoadNextImage();
        }

        private void tsbPrevious_ButtonClick(object sender, EventArgs e)
        {
            LoadPreviousImage();
        }


        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (e.Data.GetData(DataFormats.FileDrop) as string[]);
                if (files != null && files.Length > 0)
                {
                    LoadImage(files[0]);
                }
            }
        }




        private void tsbOptions_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                FileAssociation.RegisterApplicationForUser();
                FileAssociation.AssociateForUser();
                //MessageBox.Show("Files associated!", "PicPreview", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show("File association failed: " + ex.Message, "PicPreview", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void UpdateTitle()
        {
            if (this.imageCollection.IsFileSelected)
            {
                //if (Math.Abs(1 - this.zoom) <= 0.001)
                if (!this.imageCollection.IsImageLoaded || this.imageCollection.IsLoading || (this.zoom == 1))
                    this.Text = this.imageCollection.CurrentFileName + " - PicPreview";
                else
                    this.Text = this.imageCollection.CurrentFileName + " (" + Math.Round(100 * this.zoom) + "%) - PicPreview";
            }
            else
                this.Text = "PicPreview";
        }

        private void UpdateControlStates()
        {
            tsbZoomModeFit.Enabled = (this.imageCollection.IsImageLoaded && this.zoomMode != ImageZoomModes.Fill);
            tsbZoomModeOriginal.Enabled = (this.imageCollection.IsImageLoaded && this.zoomMode != ImageZoomModes.Original);
            tsbPrevious.Enabled = this.imageCollection.CanSwipeImages;
            tsbNext.Enabled = this.imageCollection.CanSwipeImages;
            tsbImageEffects.Enabled = false;
            tsbRotateCW.Enabled = false;
            tsbRotateCCW.Enabled = false;
            tsbSave.Enabled = false;
            pnlAnimation.Visible = (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation);

            // only change when really needed, so the resizing-cursor doesn't get unnecessarily changed
            Cursor targetCursor = (this.CanDragImage ? Cursors.SizeAll : Cursors.Default);
            if (this.Cursor != targetCursor)
            {
                this.Cursor = targetCursor;
                statusStrip.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Zooming and Scrolling
        enum ImageZoomModes
        {
            Original = 1,
            Fill = 2,
            Manual = 3
        }

        private ImageZoomModes zoomMode = ImageZoomModes.Manual;
        private readonly float[] ZoomLevels = new float[] { 0.001f, 0.0025f, 0.005f, 0.0075f, 0.01f, 0.025f, 0.05f, 0.075f, 0.1f, 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 16f, 18f, 20f };
        private float MinZoom { get { return ZoomLevels[0]; } }
        private float MaxZoom { get { return ZoomLevels[ZoomLevels.Length - 1]; } }
        private float zoom;
        private int offsetX, offsetY;

        private bool dragging = false;
        private Point lastDragPos;

        private bool CanZoomIn { get { return (this.imageCollection.IsImageLoaded && this.zoom < MaxZoom); } }
        private bool CanZoomOut { get { return (this.imageCollection.IsImageLoaded && (this.zoom > 1 || this.CanDragImage)); } }

        private void SetZoomMode(ImageZoomModes mode)
        {
            this.zoomMode = mode;
            UpdateControlStates();
            this.Invalidate();
        }

        private void ValidateZoom()
        {
            Rectangle imageRect = GetImageRect();
            Size zoomedSize = GetZoomedImageSize();
            if (this.zoom <= 1 && zoomedSize.Width <= imageRect.Width && zoomedSize.Height <= imageRect.Height)
            {
                SetZoomMode(ImageZoomModes.Fill);
            }
        }

        private void ChangeZoom(float newZoom, Point fix)
        {
            this.zoomMode = ImageZoomModes.Manual;

            float oldZoom = this.zoom;
            float oldOffsetX = this.offsetX;
            float oldOffsetY = this.offsetY;

            this.zoom = newZoom;
            this.offsetX = (int)((float)fix.X - this.zoom * ((float)fix.X - oldOffsetX) / oldZoom - 0.5f);
            this.offsetY = (int)((float)fix.Y - this.zoom * ((float)fix.Y - oldOffsetY) / oldZoom - 0.5f);

            ValidateZoom();

            this.Invalidate();
        }

        private void ZoomIn(Point fix)
        {
            if (!this.CanZoomIn)
                return;

            // find next higher zoom level
            float targetZoom = this.zoom;
            for (int i = 0; i < this.ZoomLevels.Length; i++)
            {
                if (this.ZoomLevels[i] > targetZoom)
                {
                    targetZoom = this.ZoomLevels[i];
                    break;
                }
            }

            ChangeZoom(targetZoom, fix);
        }
        private void ZoomIn()
        {
            Rectangle rect = GetImageRect();
            ZoomIn(new Point(rect.Width / 2, rect.Height / 2));
        }

        private void ZoomOut(Point fix)
        {
            if (!this.CanZoomOut)
                return;

            // find next lower zoom level
            float targetZoom = this.zoom;
            for (int i = (this.ZoomLevels.Length - 1); i >= 0; i--)
            {
                if (this.ZoomLevels[i] < targetZoom)
                {
                    targetZoom = this.ZoomLevels[i];
                    break;
                }
            }

            ChangeZoom(targetZoom, fix);
        }
        private void ZoomOut()
        {
            Rectangle rect = GetImageRect();
            ZoomOut(new Point(rect.Width / 2, rect.Height / 2));
        }

        private void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            Rectangle imageRect = GetImageRect();
            if (e.Delta > 0)
                ZoomIn(new Point(e.X - imageRect.X, e.Y - imageRect.Y));
            else if (e.Delta < 0)
                ZoomOut(new Point(e.X - imageRect.X, e.Y - imageRect.Y));
        }


        private bool CanDragImage
        {
            get
            {
                if (!this.imageCollection.IsImageLoaded)
                    return false;

                Rectangle imageRect = GetImageRect();
                Size zoomedSize = GetZoomedImageSize();
                return (zoomedSize.Width > imageRect.Width || zoomedSize.Height > imageRect.Height);
            }
        }

        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.dragging = this.CanDragImage;
                this.lastDragPos = e.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragging)
            {
                if (this.CanDragImage)
                {
                    SetZoomMode(ImageZoomModes.Manual);

                    this.offsetX += (e.X - this.lastDragPos.X);
                    this.offsetY += (e.Y - this.lastDragPos.Y);
                    this.lastDragPos = e.Location;
                    this.Invalidate();
                }
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.dragging = false;
            }
        }
        #endregion

        #region Animation Control
        bool noAnimationUpdate = false;
        bool wasAnimationPaused;


        private void tbrImageFrame_Scroll(object sender, EventArgs e)
        {
            if (this.noAnimationUpdate)
                return;

            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
                this.imageCollection.CurrentImage.CurrentFrame = tbrImageFrame.Value;
        }

        private void tbrImageFrame_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
            {
                this.wasAnimationPaused = this.imageCollection.CurrentImage.AnimationPaused;
                this.imageCollection.CurrentImage.PauseAnimation();
            }
        }

        private void tbrImageFrame_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
            {
                if (!this.wasAnimationPaused)
                    this.imageCollection.CurrentImage.StartAnimation();
            }
        }

        private void btnAnimation_Click(object sender, EventArgs e)
        {
            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
            {
                if (this.imageCollection.CurrentImage.AnimationPaused)
                    this.imageCollection.CurrentImage.StartAnimation();
                else
                    this.imageCollection.CurrentImage.PauseAnimation();
                btnAnimation.Image = (this.imageCollection.CurrentImage.AnimationPaused ? Properties.Resources.Run_32x : Properties.Resources.Pause_32x);
            }
        }


        private void CurrentImage_FrameChanged(Image sender, bool isUserChange)
        {
            this.Invoke(new Action(() =>
            {
                // maybe the image has changed but the event handler is still active
                if (sender != this.imageCollection.CurrentImage)
                    return;

                RedrawImage();

                if (!isUserChange)
                {
                    this.noAnimationUpdate = true;
                    tbrImageFrame.Value = Math.Max(tbrImageFrame.Minimum, Math.Min(tbrImageFrame.Maximum, sender.CurrentFrame));
                    this.noAnimationUpdate = false;
                }
            }));
        }
        #endregion

        #region Image Loading
        ImageCollection imageCollection;
        private Exception loadException;


        public void LoadImage(string file)
        {
            if (file == null)
                return;

            bool doRedraw = true;
            try
            {
                // paint will render exceptions instead of images, when it is set
                this.loadException = null;

                LoadImageResults result = this.imageCollection.LoadImage(file);
                // ImageReady-Event was fired and no redraw is necessary here
                if (result == LoadImageResults.ImageInCache)
                    doRedraw = false;
            }
            catch (Exception ex)
            {
                //TODO log and show error message
            }
            UpdateTitle();
            if (doRedraw)
            {
                UpdateControlStates();
                RedrawImage();
            }
        }

        public void LoadPreviousImage()
        {
            if (this.imageCollection.CanSwipeImages)
                LoadImage(this.imageCollection.GetPreviousImage());
        }

        public void LoadNextImage()
        {
            if (this.imageCollection.CanSwipeImages)
                LoadImage(this.imageCollection.GetNextImage());
        }


        private void ImageCollection_ImageReady(ImageCollection sender)
        {
            this.Invoke(new Action(() =>
            {
                try
                {
                    if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
                    {
                        this.noAnimationUpdate = true;
                        tbrImageFrame.Maximum = (this.imageCollection.CurrentImage.FrameCount - 1);
                        tbrImageFrame.Value = 0;
                        this.noAnimationUpdate = false;

                        btnAnimation.Image = Properties.Resources.Pause_32x;
                        this.imageCollection.CurrentImage.FrameChanged += CurrentImage_FrameChanged;
                        this.imageCollection.CurrentImage.StartAnimation();
                    }
                }
                catch (Exception ex)
                {
                    this.loadException = ex;
                }
                
                UpdateTitle();
                if (this.zoomMode == ImageZoomModes.Manual)
                    SetZoomMode(ImageZoomModes.Fill);
                
                UpdateControlStates();
                RedrawImage();
            }));
        }
       

        private void ImageCollection_ImageLoadingError(ImageCollection sender, Exception ex)
        {
            this.Invoke(new Action(() =>
            {
                this.loadException = ex;
                UpdateControlStates();
                RedrawImage();
            }));
        }
        #endregion

        #region Rendering
        Brush textBrush;
        Font textFont;


        private Rectangle GetImageRect()
        {
            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
                return new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - statusStrip.Height - pnlAnimation.Height - 2);
            else
                return new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - statusStrip.Height - 2);
        }

        private Size GetZoomedImageSize()
        {
            return new Size((int)(this.zoom * this.imageCollection.CurrentImage.Width), (int)(this.zoom * this.imageCollection.CurrentImage.Height));
        }

        private void UpdateViewParameters()
        {
            Rectangle imageRect = GetImageRect();
            if (this.zoomMode == ImageZoomModes.Fill)
            {
                if (this.imageCollection.CurrentImage.Width <= imageRect.Width && this.imageCollection.CurrentImage.Height <= imageRect.Height)
                    this.zoom = 1;
                else
                    this.zoom = Math.Min((float)imageRect.Width / (float)this.imageCollection.CurrentImage.Width, (float)imageRect.Height / (float)this.imageCollection.CurrentImage.Height);
                Size zoomedSize = GetZoomedImageSize();
                this.offsetX = (imageRect.Width - zoomedSize.Width) / 2;
                this.offsetY = (imageRect.Height - zoomedSize.Height) / 2;
            }
            else if (this.zoomMode == ImageZoomModes.Original)
            {
                this.zoom = 1;
                this.offsetX = (imageRect.Width - this.imageCollection.CurrentImage.Width) / 2;
                this.offsetY = (imageRect.Height - this.imageCollection.CurrentImage.Height) / 2;
            }
            else
            {
                Size zoomedSize = GetZoomedImageSize();

                if (zoomedSize.Width <= imageRect.Width)
                {
                    this.offsetX = (imageRect.Width - zoomedSize.Width) / 2;
                }
                else
                {
                    this.offsetX = Math.Min(0, Math.Max(-(zoomedSize.Width - imageRect.Width), this.offsetX));
                }

                if (zoomedSize.Height <= imageRect.Height)
                {
                    this.offsetY = (imageRect.Height - zoomedSize.Height) / 2;
                }
                else
                {
                    this.offsetY = Math.Min(0, Math.Max(-(zoomedSize.Height - imageRect.Height), this.offsetY));
                }
            }

            UpdateControlStates();
            UpdateTitle();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (this.imageCollection.IsFileSelected)
            {
                if (this.imageCollection.IsLoading)
                {
                    PrintCenteredString(e.Graphics, "Loading image...");
                }
                else if (this.imageCollection.IsImageLoaded)
                {
                    // update zoom and offset
                    UpdateViewParameters();

                    if (this.zoom > 0)
                    {
                        Rectangle imageRect = GetImageRect();
                        Size zoomedSize = GetZoomedImageSize();

                        Rectangle src, dst;

                        //TODO partial image rendering for better speed
                        if (this.zoom == 1)
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            src = new Rectangle(0, 0, zoomedSize.Width, zoomedSize.Height);
                            dst = new Rectangle(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        else if (this.zoom < 1)
                        {
                            e.Graphics.InterpolationMode = (this.zoom < 0.5 ? InterpolationMode.HighQualityBicubic : (this.zoom < 0.75 ? InterpolationMode.Bilinear : InterpolationMode.NearestNeighbor));
                            src = new Rectangle(0, 0, this.imageCollection.CurrentImage.Width, this.imageCollection.CurrentImage.Height);
                            dst = new Rectangle(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        else
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            src = new Rectangle(0, 0, this.imageCollection.CurrentImage.Width, this.imageCollection.CurrentImage.Height);
                            dst = new Rectangle(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }

                        // move to target rect
                        dst.X += imageRect.X;
                        dst.Y += imageRect.Y;
                        // draw
                        lock (this.imageCollection.CurrentImage.Bitmap)
                            e.Graphics.DrawImage(this.imageCollection.CurrentImage.Bitmap, dst, src, GraphicsUnit.Pixel);
                        e.Graphics.DrawRectangle(Pens.Black, dst);
                    }
                }
                else
                {
                    PrintCenteredString(e.Graphics, "Could not load image.");
                }
            }
            else
            {
                PrintCenteredString(e.Graphics, "No image selected.");
            }
        }

        private void RedrawImage()
        {
            Invalidate();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            RedrawImage();
        }

        private void PrintCenteredString(Graphics g, string str)
        {
            SizeF size = g.MeasureString(str, this.textFont);
            Rectangle imageRect = GetImageRect();
            g.DrawString(str, this.textFont, this.textBrush, imageRect.Left + (int)(imageRect.Width - size.Width) / 2, imageRect.Top + (int)(imageRect.Height - size.Height) / 2);
        }
        #endregion
    }
}
