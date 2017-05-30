using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PicPreview
{
    class ImageCache
    {
        long maxCacheSize;

        Dictionary<string, Image> cache;
        List<string> cacheOrder;
        long currentCacheSize;


        public ImageCache(long maxCacheSize)
        {
            this.maxCacheSize = maxCacheSize;
            this.cache = new Dictionary<string, Image>();
            this.cacheOrder = new List<string>();
            this.currentCacheSize = 0;
        }


        public Image GetImage(string key)
        {
            lock (this.cache)
            {
                Image img;
                if (this.cache.TryGetValue(key, out img))
                {
                    // the key was used again and should now be deleted last
                    this.cacheOrder.Remove(key);
                    this.cacheOrder.Add(key);
                    return img;
                }
                else
                    return null;
            }
        }

        public void StoreImage(string key, Image image)
        {
            long imgSize = image.GetMemorySize();
            // image doesn't fit into the cache -> don't cache...
            if (imgSize > this.maxCacheSize)
                return;

            lock (this.cache)
            {
                // delete old entries so the image fits
                FreeMemory(imgSize);

                // already in cache? delete old entry
                if (this.cache.ContainsKey(key))
                    InternalRemoveImage(key);

                // add new entry and update cache size
                this.cache.Add(key, image);
                this.cacheOrder.Add(key);
                this.currentCacheSize += imgSize;
            }
        }

        private void FreeMemory(long amount)
        {
            amount = Math.Min(this.currentCacheSize, amount);
            long target = this.maxCacheSize - amount;

            while (this.cacheOrder.Count > 0 && this.currentCacheSize > target)
            {
                InternalRemoveImage(this.cacheOrder[0]);
            }
        }

        public void RemoveImage(string key)
        {
            lock(this.cache)
            {
                InternalRemoveImage(key);
            }
        }
        private void InternalRemoveImage(string key)
        {
            Image img;
            if (this.cache.TryGetValue(key, out img))
            {
                this.currentCacheSize -= img.GetMemorySize();
                this.cache.Remove(key);
                this.cacheOrder.Remove(key);
                img.Dispose();
            }
        }
    }
}
