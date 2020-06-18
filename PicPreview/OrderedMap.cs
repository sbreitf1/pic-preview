using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PicPreview
{
    class OrderedMap<T>
    {
        private List<T> elements;
        private Dictionary<T, int> indexLookup;

        public int Count { get { return this.elements.Count; } }


        public OrderedMap()
        {
            this.elements = new List<T>();
            this.indexLookup = new Dictionary<T, int>();
        }

        public OrderedMap(T[] elements)
        {
            this.elements = new List<T>(elements);
            this.indexLookup = new Dictionary<T, int>();
            for (int i = 0; i < this.elements.Count; i++)
                this.indexLookup.Add(this.elements[i], i);
        }

        public void Clear()
        {
            lock (this.elements)
            {
                this.elements.Clear();
                this.indexLookup.Clear();
            }
        }

        public int Add(T elem)
        {
            int newIndex;
            lock (this.elements)
            {
                newIndex = this.elements.Count;
                this.elements.Add(elem);
                this.indexLookup.Add(elem, newIndex);
            }
            return newIndex;
        }

        public void AddRange(T[] elements)
        {
            lock (this.elements)
            {
                this.elements.AddRange(elements);
                for (int i = 0; i < elements.Length; i++)
                    this.indexLookup.Add(elements[i], this.indexLookup.Count);
            }
        }

        public T Get(int i)
        {
            lock (this.elements)
            {
                return this.elements[i];
            }
        }
        
        public bool TryGetIndex(T elem, out int index)
        {
            lock (this.elements)
            {
                return this.indexLookup.TryGetValue(elem, out index);
            }
        }
    }
}
