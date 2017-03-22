using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

namespace PicPreview
{
    class ImageCollection
    {
        private string currentDirectory;
        public string CurrentDirectory { get { return this.currentDirectory; } }
        private string currentFile;
        public string CurrentFile { get { return this.currentFile; } }
        public string CurrentFileName { get { return Path.GetFileName(this.currentFile); } }
        private Image currentImage;
        public Image CurrentImage { get { return this.currentImage; } }

        public bool IsFileSelected { get { return !string.IsNullOrWhiteSpace(this.currentFile); } }
        public bool IsImageLoaded { get { return this.currentImage != null; } }
        public bool CanSwipeImages { get { return !string.IsNullOrWhiteSpace(this.currentDirectory) && Directory.Exists(this.currentDirectory); } }

        HashSet<string> imageExtensions;


        public ImageCollection()
        {
            this.imageExtensions = new HashSet<string>();
            this.imageExtensions.Add(".jpg");
            this.imageExtensions.Add(".jpeg");
            this.imageExtensions.Add(".png");
            this.imageExtensions.Add(".bmp");
            this.imageExtensions.Add(".gif");
            this.imageExtensions.Add(".tiff");
        }


        public void SelectImage(string imageFile)
        {
            try
            {
                // disposing unused Bitmap objects is important as they most likely introduce memory leaks
                if (this.currentImage != null)
                {
                    this.currentImage.Dispose();
                    this.currentImage = null;
                }
            }
            catch { }

            // safe location information first for browsing (left/right)
            this.currentFile = imageFile;
            this.currentDirectory = Path.GetDirectoryName(this.currentFile);

            // now try to load the image
            this.currentImage = new Image(this.currentFile);
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
