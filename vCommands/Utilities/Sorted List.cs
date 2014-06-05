#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vCommands.Utilities
{
#pragma warning disable 1574
    /// <summary>
    /// Represents a list of key/value pairs that are sorted by key.
    /// </summary>
    /// <remarks>
    /// Is not the same as <see cref="System.Collections.Generic.SortedList{TKey,TValue}"/>, but provides identical functionality, except for the inability of acquring a collection of keys or values separately.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
#pragma warning restore 1574
    public sealed class SortedList<TKey, TValue>
        : IDictionary<TKey, TValue>, IList<KeyValuePair<TKey, TValue>>
    {
        internal KeyValuePair<TKey, TValue>[] inner = null;
        internal int size = 0;

        internal IComparer<TKey> comparer = null;
        internal KeyComparer keyComparer = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class.
        /// </summary>
        public SortedList()
            : this(1, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class with the specified key comparer.
        /// </summary>
        public SortedList(IComparer<TKey> comparer)
            : this(1, comparer)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class with the specified initial capacity.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given length is zero or less.</exception>
        public SortedList(int capacity)
            : this(capacity, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class with the specified initial elements.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when the given initial elements enumeration is null.</exception>
        public SortedList(IEnumerable<KeyValuePair<TKey, TValue>> elements)
            : this(elements, null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class with the specified initial capacity and key comparer.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the given length is zero or less.</exception>
        public SortedList(int capacity, IComparer<TKey> comparer)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException("capacity", "List capacity must be at least one.");

            this.comparer = comparer ?? Comparer<TKey>.Default;

            keyComparer = new KeyComparer { comparer = this.comparer };

            inner = new KeyValuePair<TKey, TValue>[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="vCommands.Utilities.SortedList{TKey,TValue}"/> class with the specified initial elements and key comparer.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when the given initial elements enumeration is null.</exception>
        public SortedList(IEnumerable<KeyValuePair<TKey, TValue>> elements, IComparer<TKey> comparer)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            this.comparer = comparer ?? Comparer<TKey>.Default;

            keyComparer = new KeyComparer { comparer = this.comparer };

            inner = elements.ToArray();

            if (inner.Length == 0)
                inner = new KeyValuePair<TKey, TValue>[1];
        }

        private void DoubleContainer()
        {
            var n = new KeyValuePair<TKey, TValue>[inner.Length * 2];
            Array.Copy(inner, n, inner.Length);
            inner = n;
        }

        /// <summary>
        /// Compacts the inner container to hold exactly the amount of items it currently contains.
        /// </summary>
        public void Compact()
        {
            if (inner.Length == size)
                return;

            var n = new KeyValuePair<TKey, TValue>[size];
            Array.Copy(inner, n, size);
            inner = n;
        }

        private int Lookup(TKey key)
        {
            return Array.BinarySearch(inner, 0, size, new KeyValuePair<TKey, TValue>(key, default(TValue)), keyComparer);
        }

        private int IndexOfKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int ind = Lookup(key);

            return ind < 0 ? -1 : ind;
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                var ind = Lookup(key);

                if (ind < 0)
                    throw new KeyNotFoundException("The given key does not exist in the array.");
                else
                    return inner[ind].Value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                var ind = Lookup(key);

                if (ind < 0)
                {
                    if (size == inner.Length)
                        DoubleContainer();

                    ind = ~ind;

                    for (int i = size - 1; i > ind; i--)
                        inner[i] = inner[i - 1];

                    inner[ind] = new KeyValuePair<TKey, TValue>(key, value);

                    size++;
                }
                else
                    inner[ind] = new KeyValuePair<TKey, TValue>(key, value);
            }
        }

        internal class KeyComparer : IComparer<KeyValuePair<TKey, TValue>>
        {
            public IComparer<TKey> comparer = null;

            public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
            {
                return comparer.Compare(x.Key, y.Key);
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value to the list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an element with the given key already exists.</exception>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var ind = Lookup(key);

            if (ind < 0)
            {
                if (size == inner.Length)
                    DoubleContainer();

                ind = ~ind;

                for (int i = size - 1; i > ind; i--)
                    inner[i] = inner[i - 1];

                inner[ind] = new KeyValuePair<TKey, TValue>(key, value);

                size++;
            }
            else
                throw new ArgumentException("The given key already exists in the list.");
        }

        /// <summary>
        /// Determines whether the list contains the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public bool ContainsKey(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return Lookup(key) >= 0;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Removes the element with the specified key from the list.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public bool Remove(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            int ind = Lookup(key);

            if (ind < 0)
                return false;

            for (int i = ind + 1; i < size; i++)
                inner[i - 1] = inner[i];

            inner[size] = new KeyValuePair<TKey, TValue>();

            size--;

            return true;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var ind = Lookup(key);

            if (ind < 0)
            {
                value = default(TValue);
                return false;
            }
            else
            {
                value = inner[ind].Value;
                return true;
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value to the list.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given key is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when an element with the given key already exists.</exception>
        public bool TryAdd(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            var ind = Lookup(key);

            if (ind < 0)
            {
                if (size == inner.Length)
                    DoubleContainer();

                ind = ~ind;

                for (int i = size - 1; i > ind; i--)
                    inner[i] = inner[i - 1];

                inner[ind] = new KeyValuePair<TKey, TValue>(key, value);

                size++;

                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public ICollection<TValue> Values
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Adds the specified element to the list.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item's key is null.</exception>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all elements from the list.
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < size; i++)
                inner[i] = new KeyValuePair<TKey, TValue>();

            size = 0;
        }

        /// <summary>
        /// Determines whether the list contains the specified element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item's key is null.</exception>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue val = default(TValue);

            if (TryGetValue(item.Key, out val))
                return val.Equals(item.Value);

            return false;
        }

        /// <summary>
        /// Copies the elements from the list to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from list. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex", "Starting index must be greater than or equal to zero.");

            for (int i = 0; i < size; i++)
                array[arrayIndex + i] = inner[i];
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the list.
        /// </summary>
        public int Count
        {
            get { return size; }
        }

        /// <summary>
        /// Gets the number of elements that the list can contain.
        /// </summary>
        public int Capacity
        {
            get { return inner.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes the specified element from the list.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Thrown when the given item's key is null.</exception>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
                throw new ArgumentNullException("item.Key");

            int ind = Lookup(item.Key);

            if (ind < 0)
                return false;

            if (!inner[ind].Value.Equals(item.Value))
                return false;

            for (int i = ind + 1; i < size; i++)
                inner[i - 1] = inner[i];

            inner[size] = new KeyValuePair<TKey, TValue>();

            size--;

            return true;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)inner).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        int IList<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
        {
            int ind = Lookup(item.Key);

            if (ind < 0 || !inner[ind].Value.Equals(item.Value))
                return -1;

            return ~ind;
        }

        void IList<KeyValuePair<TKey, TValue>>.Insert(int index, KeyValuePair<TKey, TValue> item)
        {
            if (index < 0 || index >= size)
                throw new IndexOutOfRangeException();

            var ind = Lookup(item.Key);

            if (index != ind)
                throw new InvalidOperationException("Cannot insert item at specified index.");

            Add(item);
        }

        void IList<KeyValuePair<TKey, TValue>>.RemoveAt(int index)
        {
            for (int i = index + 1; i < size; i++)
                inner[i - 1] = inner[i];

            inner[size] = new KeyValuePair<TKey, TValue>();

            size--;
        }

        KeyValuePair<TKey, TValue> IList<KeyValuePair<TKey, TValue>>.this[int index]
        {
            get
            {
                if (index < 0 || index >= size)
                    throw new IndexOutOfRangeException();

                return inner[index];
            }
            set
            {
                ((IList<KeyValuePair<TKey, TValue>>)this).Insert(index, value);
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count
        {
            get { return size; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif