using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestMODBUS.Models.Data
{
    public class ObservableConcurrentList : INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private ConcurrentBag<int> _collection;

        public ObservableConcurrentList() 
        {
            _collection = new ConcurrentBag<int>();
        }

        public void Add(int item)
        {
            _collection.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Remove(int item)
        {
            var temp = new ConcurrentBag<int>();
            foreach(var channel in _collection)
            {
                if (channel != item)
                    temp.Add(channel);
            }

            _collection = temp;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public void Clear()
        {
            _collection = new ConcurrentBag<int>();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public int Max()
        {
            return _collection.Max();
        }

        public int Min() => _collection.Min();

        public int Count => _collection.Count();

        public bool Contains(int element) => _collection.Contains(element);

        public IEnumerator<int> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        public int[] ToArray()
        {
            return _collection.ToArray();
        }
    }
}
