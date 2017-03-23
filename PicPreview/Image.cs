using System;
using System.Drawing;
using System.IO;

namespace PicPreview
{
    class Image : IDisposable
    {
        protected Bitmap bitmap;
        public Bitmap Bitmap { get { return this.bitmap; } }

        public bool HasAnimation { get { return false; } }


        public Image(string file)
        {
            string extension = Path.GetExtension(file);
            switch (extension.ToLower())
            {
                default:
                    this.bitmap = (Bitmap)Bitmap.FromFile(file);
                    break;
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
    }
}
