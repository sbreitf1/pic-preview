using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PicPreview
{
    public partial class MainForm : Form
    {
        #region Initialization and Disposal
        public MainForm()
        {
            InitializeComponent();
            this.MouseWheel += MainForm_MouseWheel;

            this.textBrush = Brushes.Black;
            this.textFont = new Font(this.Font.FontFamily, this.Font.Size * 2);

            this.imageCollection = new ImageCollection();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                LoadImage(args[1]);
            else
                UpdateControlStates();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        #endregion

        #region Generic UI
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                    LoadPreviousImage();
                    break;
                case Keys.Right:
                    LoadNextImage();
                    break;
            }
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

        }


        private void UpdateTitle()
        {
            if (this.imageCollection.IsFileSelected)
            {
                //if (Math.Abs(1 - this.zoom) <= 0.001)
                if (this.zoom == 1)
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
            tsbSave.Enabled = this.imageCollection.IsImageLoaded;

            // only change when really needed, so the resizing-cursor doesn't get unnecessarily changed
            Cursor targetCursor = (this.CanDragImage ? Cursors.SizeAll : Cursors.Default);
            if (this.Cursor != targetCursor)
            {
                this.Cursor = targetCursor;
                statusStrip.Cursor = Cursors.Default;
            }

            UpdateTitle();
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
        private float zoom;
        private int offsetX, offsetY;

        private bool dragging = false;
        private Point lastDragPos;

        private bool CanZoomIn { get { return true; } }
        private bool CanZoomOut { get { return (this.zoom > 1 || this.CanDragImage); } }

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

        private void ZoomIn(Point fix)
        {
            if (!this.CanZoomIn)
                return;

            this.zoomMode = ImageZoomModes.Manual;

            this.zoom *= 1.1f;
            ValidateZoom();

            this.Invalidate();
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

            this.zoomMode = ImageZoomModes.Manual;

            this.zoom /= 1.1f;
            ValidateZoom();

            this.Invalidate();
        }
        private void ZoomOut()
        {
            Rectangle rect = GetImageRect();
            ZoomOut(new Point(rect.Width / 2, rect.Height / 2));
        }

        private void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                ZoomIn();
            else if (e.Delta < 0)
                ZoomOut();
        }


        private bool CanDragImage
        {
            get
            {
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

        #region Image Loading
        ImageCollection imageCollection;
        private Exception loadException;


        public void LoadImage(string file)
        {
            if (file == null)
                return;

            try
            {
                try
                {
                    this.imageCollection.SelectImage(file);

                    // paint will render exceptions instead of images, when it is set
                    this.loadException = null;
                }
                catch (Exception ex)
                {
                    this.loadException = ex;
                }

                UpdateTitle();
                if (this.zoomMode == ImageZoomModes.Manual)
                    SetZoomMode(ImageZoomModes.Fill);
            }
            catch (Exception ex)
            {
                //TODO log and show error message

                this.loadException = ex;
            }
            UpdateControlStates();
            RedrawImage();
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
        #endregion

        #region Rendering
        Brush textBrush;
        Font textFont;


        private Rectangle GetImageRect()
        {
            return new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - statusStrip.Height - 2);
        }

        private Size GetZoomedImageSize()
        {
            return new Size((int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Width), (int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Height));
        }

        private void UpdateViewParameters()
        {
            Rectangle imageRect = GetImageRect();
            if (this.zoomMode == ImageZoomModes.Fill)
            {
                if (this.imageCollection.CurrentImage.Bitmap.Width <= imageRect.Width && this.imageCollection.CurrentImage.Bitmap.Height <= imageRect.Height)
                    this.zoom = 1;
                else
                    this.zoom = Math.Min((float)imageRect.Width / (float)this.imageCollection.CurrentImage.Bitmap.Width, (float)imageRect.Height / (float)this.imageCollection.CurrentImage.Bitmap.Height);
                Size zoomedSize = GetZoomedImageSize();
                this.offsetX = (imageRect.Width - zoomedSize.Width) / 2;
                this.offsetY = (imageRect.Height - zoomedSize.Height) / 2;
            }
            else if (this.zoomMode == ImageZoomModes.Original)
            {
                this.zoom = 1;
                this.offsetX = (imageRect.Width - this.imageCollection.CurrentImage.Bitmap.Width) / 2;
                this.offsetY = (imageRect.Height - this.imageCollection.CurrentImage.Bitmap.Height) / 2;
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
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (this.imageCollection.IsFileSelected)
            {
                if (this.imageCollection.IsImageLoaded)
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
                            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            src = new Rectangle(0, 0, this.imageCollection.CurrentImage.Bitmap.Width, this.imageCollection.CurrentImage.Bitmap.Height);
                            dst = new Rectangle(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }
                        else
                        {
                            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            src = new Rectangle(0, 0, this.imageCollection.CurrentImage.Bitmap.Width, this.imageCollection.CurrentImage.Bitmap.Height);
                            dst = new Rectangle(this.offsetX, this.offsetY, zoomedSize.Width, zoomedSize.Height);
                        }

                        // move to target rect
                        dst.X += imageRect.X;
                        dst.Y += imageRect.Y;
                        // draw
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
