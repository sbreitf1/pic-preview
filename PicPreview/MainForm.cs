using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PicPreview
{
    public partial class MainForm : Form
    {
        #region Initialization and Disposal
        public MainForm()
        {
            InitializeComponent();
            this.Text = Program.AppName;
            this.Icon = Properties.Resources.pic_preview;
            this.MouseWheel += MainForm_MouseWheel;

            this.textBrush = Brushes.Black;
            this.textFont = new Font(this.Font.FontFamily, this.Font.Size * 2);

            this.imageCollection = new ImageCollection();
            this.imageCollection.ImageReady += ImageCollection_ImageReady;
            this.imageCollection.ImageLoadingError += ImageCollection_ImageLoadingError;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // update window state befor showing to prevent flickering
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
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
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
            ConfigDialog dialog = new ConfigDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                RedrawImage(RedrawReason.GenericUpdate);
            }
        }


        private void UpdateTitle()
        {
            if (this.imageCollection.IsFileSelected)
            {
                if (!this.imageCollection.IsImageLoaded || this.imageCollection.IsLoading || (this.zoom == 1))
                    this.Text = this.imageCollection.CurrentFileName + " - " + Program.AppName;
                else
                    this.Text = this.imageCollection.CurrentFileName + " (" + Math.Round(100 * this.zoom) + "%) - " + Program.AppName;
            }
            else
                this.Text = Program.AppName;
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
            tsbSave.Enabled = (this.imageCollection.IsImageLoaded);
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
        private readonly float[] ZoomLevels = new float[] { 0.001f, 0.0025f, 0.005f, 0.0075f, 0.01f, 0.025f, 0.05f, 0.075f, 0.1f, 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.5f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f, 14f, 16f, 18f, 20f, 25f, 30f, 40f };
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
            RedrawImage(RedrawReason.GenericUpdate);
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

            RedrawImage(RedrawReason.UserInteraction);
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
                    RedrawImage(RedrawReason.UserInteraction);
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

                RedrawImage(RedrawReason.AnimationFrame);

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
                RedrawImage(RedrawReason.InitialDraw);
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
                RedrawImage(RedrawReason.InitialDraw);
            }));
        }


        private void ImageCollection_ImageLoadingError(ImageCollection sender, Exception ex)
        {
            this.Invoke(new Action(() =>
            {
                this.loadException = ex;
                UpdateControlStates();
                RedrawImage(RedrawReason.InitialDraw);
            }));
        }
        #endregion

        #region Image Exporting
        private void tsbSave_ButtonClick(object sender, EventArgs e)
        {
            if (this.imageCollection.CurrentImage.HasAnimation)
            {
                if (MessageBox.Show("Only the currently visible frame will be saved from the animated image.", "Save Image", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                {
                    return;
                }
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = this.imageCollection.CurrentDirectory;
            dialog.FileName = this.imageCollection.CurrentFileName;
            dialog.Filter = "PNG|*.png|JPEG|*.jpg;*.jpeg|WEBP|*.webp|Bitmap|*.bmp|GIF|*.gif";
            string ext = Path.GetExtension(this.imageCollection.CurrentFileName);
            switch (ext.ToLower())
            {
                case ".png":
                    dialog.FilterIndex = 1;
                    break;

                case ".jpg":
                case ".jpeg":
                    dialog.FilterIndex = 2;
                    break;

                case ".webp":
                    dialog.FilterIndex = 3;
                    break;

                case ".bmp":
                    dialog.FilterIndex = 4;
                    break;

                case ".gif":
                    dialog.FilterIndex = 5;
                    break;

                default:
                    dialog.FileName = this.imageCollection.CurrentFileName.Substring(0, this.imageCollection.CurrentFileName.Length - ext.Length) + ".png";
                    dialog.FilterIndex = 1;
                    break;
            }
            dialog.AddExtension = true;
            dialog.OverwritePrompt = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string newExt = Path.GetExtension(dialog.FileName);
                    switch (newExt.ToLower())
                    {
                        case ".png":
                            ExportPNG(dialog.FileName);
                            break;

                        case ".jpg":
                        case ".jpeg":
                            ExportJPG(dialog.FileName);
                            break;

                        case ".webp":
                            ExportWEBP(dialog.FileName);
                            break;

                        case ".bmp":
                            ExportBMP(dialog.FileName);
                            break;

                        case ".gif":
                            ExportGIF(dialog.FileName);
                            break;

                        default:
                            MessageBox.Show("Unsupported export format.", "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed: " + ex.Message, "Save Image", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportPNG(string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                this.imageCollection.CurrentImage.Bitmap.Save(stream, ImageFormat.Png);
            }
        }

        private void ExportJPG(string file)
        {
            if (this.imageCollection.CurrentImage.HasAlphaChannel)
            {
                if (MessageBox.Show("The alpha transparency channel will be lost when saving in this format.", "Save Image", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    return;
            }

            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                //TODO show compression options
                this.imageCollection.CurrentImage.Bitmap.Save(stream, ImageFormat.Jpeg);
            }
        }

        private void ExportWEBP(string file)
        {
            Bitmap b = this.imageCollection.CurrentImage.Bitmap;
            bool isClonedImage = false;
            if (b.PixelFormat != PixelFormat.Format24bppRgb && b.PixelFormat != PixelFormat.Format32bppArgb)
            {
                // need to change pixel format for webp encoder
                PixelFormat pf = (this.imageCollection.CurrentImage.HasAlphaChannel ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
                b = this.imageCollection.CurrentImage.Bitmap.Clone(new Rectangle(0, 0, b.Width, b.Height), pf);
                isClonedImage = true;
            }

            WebP webp = new WebP();
            //TODO show compression options
            webp.Save(b, file, 75);

            if (isClonedImage)
            {
                b.Dispose();
            }
        }

        private void ExportBMP(string file)
        {
            if (this.imageCollection.CurrentImage.HasAlphaChannel)
            {
                if (MessageBox.Show("The alpha transparency channel will be lost when saving in this format.", "Save Image", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    return;
            }

            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                this.imageCollection.CurrentImage.Bitmap.Save(stream, ImageFormat.Bmp);
            }
        }

        private void ExportGIF(string file)
        {
            if (this.imageCollection.CurrentImage.HasAlphaChannel)
            {
                if (MessageBox.Show("The alpha transparency channel will be lost when saving in this format.", "Save Image", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    return;
            }

            using (FileStream stream = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                this.imageCollection.CurrentImage.Bitmap.Save(stream, ImageFormat.Gif);
            }
        }
        #endregion

        #region Rendering
        Brush textBrush;
        Font textFont;

        DateTime hqRedrawAt = DateTime.MaxValue;
        bool renderHighQualityNextTime = false;


        private Rectangle GetImageRect()
        {
            if (this.imageCollection.IsImageLoaded && this.imageCollection.CurrentImage.HasAnimation)
                return new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - statusStrip.Height - pnlAnimation.Height - 1);
            else
                return new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - statusStrip.Height - 1);
        }

        private Size GetZoomedImageSize()
        {
            return new Size((int)Math.Round(this.zoom * this.imageCollection.CurrentImage.Width), (int)Math.Round(this.zoom * this.imageCollection.CurrentImage.Height));
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

        private InterpolationMode GetMaximizationFilter(bool hq)
        {
            if (!hq)
                return InterpolationMode.NearestNeighbor;

            if (this.imageCollection.CurrentImage.HasAnimation && Properties.Settings.Default.FastRenderAnimations)
                return InterpolationMode.NearestNeighbor;

            return Properties.Settings.Default.MaximizeFilter;
        }

        private InterpolationMode GetMinimizationFilter(bool hq)
        {
            if (!hq)
                return InterpolationMode.NearestNeighbor;

            if (this.imageCollection.CurrentImage.HasAnimation && Properties.Settings.Default.FastRenderAnimations)
                return InterpolationMode.NearestNeighbor;

            return Properties.Settings.Default.MinimizeFilter;
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

                        RectangleF src, dst;

                        if (this.zoom == 1)
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            src = new RectangleF(0, 0, zoomedSize.Width, zoomedSize.Height);
                            dst = new RectangleF(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        else if (this.zoom < 1)
                        {
                            e.Graphics.InterpolationMode = GetMinimizationFilter(renderHighQualityNextTime);
                            src = new RectangleF(0, 0, this.imageCollection.CurrentImage.Width, this.imageCollection.CurrentImage.Height);
                            dst = new RectangleF(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        else
                        {
                            e.Graphics.InterpolationMode = GetMaximizationFilter(renderHighQualityNextTime);
                            src = new RectangleF(0, 0, this.imageCollection.CurrentImage.Width, this.imageCollection.CurrentImage.Height);
                            dst = new RectangleF(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        if (zoomedSize.Width > imageRect.Width)
                        {
                            src = new RectangleF(-this.offsetX / zoom, src.Y, imageRect.Width / zoom, src.Height);
                            dst = new RectangleF(0, dst.Y, imageRect.Width, dst.Height);
                        }
                        if (zoomedSize.Height > imageRect.Height)
                        {
                            src = new RectangleF(src.X, -this.offsetY / zoom, src.Width, imageRect.Height / zoom);
                            dst = new RectangleF(dst.X, 0, dst.Width, imageRect.Height);
                        }

                        // move to target rect
                        dst.X += imageRect.X;
                        dst.Y += imageRect.Y;

                        if (this.imageCollection.CurrentImage.HasAlphaChannel && Properties.Settings.Default.RenderTransparencyGrid)
                        {
                            const int AlphaGridSize = 20;

                            int dstX = (int)dst.X;
                            int dstY = (int)dst.Y;
                            int dstWidth = (int)dst.Width;
                            int dstHeight = (int)dst.Height;

                            SmoothingMode oldSmoothingMode = e.Graphics.SmoothingMode;
                            e.Graphics.SmoothingMode = SmoothingMode.None;
                            // render grid for transparency visualization
                            int xStart = (dstX / AlphaGridSize) * AlphaGridSize;
                            int yStart = (dstY / AlphaGridSize) * AlphaGridSize;
                            for (int x = xStart; x <= (dst.X + dst.Width); x += AlphaGridSize)
                            {
                                for (int y = yStart; y <= (dst.Y + dst.Height); y += AlphaGridSize)
                                {
                                    Brush brush = ((x / AlphaGridSize + y / AlphaGridSize) % 2 == 0 ? Brushes.LightGray : Brushes.White);

                                    int tx = x;
                                    int ty = y;
                                    int tw = AlphaGridSize;
                                    int th = AlphaGridSize;
                                    if (tx < dst.X)
                                    {
                                        tw -= (dstX - tx);
                                        tx = dstX;
                                    }
                                    if (ty < dst.Y)
                                    {
                                        th -= (dstY - ty);
                                        ty = dstY;
                                    }
                                    if ((tx + tw) > (dstX + dstWidth))
                                    {
                                        tw -= (tx + tw) - (dstX + dstWidth);
                                    }
                                    if ((ty + th) > (dstY + dstHeight))
                                    {
                                        th -= (ty + th) - (dstY + dstHeight);
                                    }

                                    e.Graphics.FillRectangle(brush, tx, ty, tw, th);
                                }
                            }
                            e.Graphics.SmoothingMode = oldSmoothingMode;
                        }

                        // draw
                        PixelOffsetMode oldPixelOffset = e.Graphics.PixelOffsetMode;
                        if (zoom > 1)
                        {
                            // pixel offset half prevents ugly alignment errors when zooming
                            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
                        }
                        lock (this.imageCollection.CurrentImage.Bitmap)
                            e.Graphics.DrawImage(this.imageCollection.CurrentImage.Bitmap, dst, src, GraphicsUnit.Pixel);
                        e.Graphics.PixelOffsetMode = oldPixelOffset;

                        e.Graphics.DrawRectangle(Pens.Black, new Rectangle((int)dst.X, (int)dst.Y, (int)dst.Width, (int)dst.Height));

                        renderHighQualityNextTime = true;
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

        private void PrintCenteredString(Graphics g, string str)
        {
            SizeF size = g.MeasureString(str, this.textFont);
            Rectangle imageRect = GetImageRect();
            g.DrawString(str, this.textFont, this.textBrush, imageRect.Left + (int)(imageRect.Width - size.Width) / 2, imageRect.Top + (int)(imageRect.Height - size.Height) / 2);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            RedrawImage(RedrawReason.UserInteraction);
        }

        private enum RedrawReason : int
        {
            GenericUpdate,
            InitialDraw,
            UserInteraction,
            DeferredUpdate,
            AnimationFrame
        }

        private void RedrawImage(RedrawReason reason)
        {
            // render hq by default:
            renderHighQualityNextTime = true;
            switch (reason)
            {
                case RedrawReason.UserInteraction:
                    if (Properties.Settings.Default.FastRenderInteraction)
                        renderHighQualityNextTime = false;
                    break;
            }

            Invalidate();

            if (reason == RedrawReason.UserInteraction && Properties.Settings.Default.FastRenderInteraction)
            {
                this.hqRedrawAt = DateTime.Now.AddMilliseconds(250);
            }
        }

        private void tmrRedraw_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now >= this.hqRedrawAt)
            {
                this.hqRedrawAt = DateTime.MaxValue;
                RedrawImage(RedrawReason.DeferredUpdate);
            }
        }
        #endregion
    }
}
