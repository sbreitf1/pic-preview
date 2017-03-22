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
    enum ImageZoomModes
    {
        Original = 1,
        Fill = 2,
        Manual = 3
    }


    public partial class MainForm : Form
    {
        ImageCollection imageCollection;
        private Exception loadException;
        private ImageZoomModes zoomMode;
        private float zoom;
        private int offsetX, offsetY;

        Brush textBrush;
        Font textFont;


        public MainForm()
        {
            InitializeComponent();

            this.textBrush = Brushes.Black;
            this.textFont = this.Font;

            this.imageCollection = new ImageCollection();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
                LoadImage(args[1]);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }


        private void UpdateTitle()
        {
            //if (Math.Abs(1 - this.zoom) <= 0.001)
            if (this.zoom == 1)
                this.Text = this.imageCollection.CurrentFileName + " - PicPreview";
            else
                this.Text = this.imageCollection.CurrentFileName + " (" + Math.Round(100 * this.zoom) + "%) - PicPreview";
        }

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
                this.zoomMode = ImageZoomModes.Fill;
            }
            catch (Exception ex)
            {
                //TODO log and show error message

                this.loadException = ex;
            }
            RedrawImage();
        }

        public void LoadPreviousImage()
        {
            LoadImage(this.imageCollection.GetPreviousImage());
        }

        public void LoadNextImage()
        {
            LoadImage(this.imageCollection.GetNextImage());
        }

        
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Left:
                    LoadPreviousImage();
                    break;
                case Keys.Right:
                    LoadNextImage();
                    break;
            }
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


        private Rectangle GetImageRect()
        {
            return new Rectangle(1, 1, this.ClientSize.Width - 2, this.ClientSize.Height - statusStrip.Height - 2);
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
                Size zoomedSize = new Size((int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Width), (int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Height));
                this.offsetX = (imageRect.Width - zoomedSize.Width) / 2;
                this.offsetY = (imageRect.Height - zoomedSize.Height) / 2;
            }

            UpdateTitle();
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            if (this.loadException == null)
            {
                if (this.imageCollection.CurrentImage != null)
                {
                    if (this.imageCollection.CurrentImage.Bitmap != null)
                    {
                        // update zoom and offset
                        UpdateViewParameters();

                        if (this.zoom > 0)
                        {
                            Rectangle imageRect = GetImageRect();
                            Size zoomedSize = new Size((int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Width), (int)(this.zoom * this.imageCollection.CurrentImage.Bitmap.Height));

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
                                src = Rectangle.Empty;
                                dst = Rectangle.Empty;
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
            else
            {
                PrintCenteredString(e.Graphics, "Could not load image.");
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
    }
}
