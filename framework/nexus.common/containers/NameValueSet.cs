using System;
using System.Collections;
using System.Text;

namespace nexus.common.helper
{
    #region IValueCols Interface Definition

    /// <summary>Represents a nongeneric collection of key/value pairs</summary>
    public interface IValueCols : ICollection, IEnumerable
    {
        /// <summary>Gets a value indicating whether the IValueCols object has a fixed size (unalterable number of columns)</summary>
        bool IsFixedSize { get; }

        /// <summary>Gets a value indicating whether the IValueCols object is read-only</summary>
        bool IsReadOnly { get; }

        /// <summary>Gets an System.Collections.ICollection object containing the keys of the IValueCols object</summary>
        ICollection Keys { get; }

        /// <summary>Gets an System.Collections.ICollection object containing the values in the IValueCols object</summary>
        ICollection Values { get; }

        /// <summary>Gets or sets the element associated with the specified key</summary>
        /// <param name="key">The key of the element to get or set</param>
        /// <returns>The value object associated with the specified key</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.NotSupportedException">The property is set and the IValueCols object is read-only</exception>
        /// <exception cref="System.NotSupportedException">The property is set and the key does not exist in the collection and the IValueCols object has a fixed size.</exception>
        object this[string key] { get; set; }

        /// <summary>Adds an element with the provided key and value to the IValueCols object</summary>
        /// <param name="key"  >The String to use as the key of the element to add</param>
        /// <param name="value">The Object to use as the key of the element to add</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentException">key is not a String</exception>
        /// <exception cref="System.ArgumentException">An element with the same key already exists in the IValueCols object</exception>
        /// <exception cref="System.NotSupportedException">The IValueCols object has a fixed size or the ?NameValueItem element is read-only?</exception>
        void Add(String key, Object value);

        /// <summary>Adds an element with the provided key and value to the IValueCols object</summary>
        /// <param name="key"   >The String to use as the key of the element to add</param>
        /// <param name="value" >The Object to use as the value of the element to add</param>
        /// <param name="status">The ValueItemStatus to use as the status of the element to add</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentException">key is not a String</exception>
        /// <exception cref="System.ArgumentException">An element with the same key already exists in the IValueCols object</exception>
        /// <exception cref="System.NotSupportedException">The IValueCols object has a fixed size or the ?NameValueItem element is read-only?</exception>
        void Add(String key, Object value, ValueItemStatus status);

        /// <summary>Removes all elements from the IValueCols object</summary>
        /// <exception cref="System.NotSupportedException">Whenever encountering any IValueCols object having a <paramref name="State"/> other than ReadWriteDelete</exception>
        void Clear();

        /// <summary>Determines whether the IValueCols object contains an element with the specified key</summary>
        /// <param name="key">The key to locate in the IValueSet object</param>
        /// <returns>true if the IValueCols object contains an element with the key otherwise false</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentException">key is not a String</exception>
        bool Contains(String key);

        /// <summary>Returns an IValueColsEnumerator object for the IValueCols object</summary>
        /// <returns>An IValueColsEnumerator object for the IValueCols object</returns>
        new IValueColsEnumerator GetEnumerator();

        /// <summary>Removes the element with the specified key from the IValueCols object</summary>
        /// <param name="key">The key of the element to remove</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentException">key is not a String</exception>
        /// <exception cref="System.NotSupportedException">The IValueCols object has a fixed size or the ?NameValueItem element is read-only?</exception>
        void Remove(String key);
    }

    public interface IValueColsEnumerator : IEnumerator
    {
        NameValueItem Entry { get; }
        String Key { get; }
        Object Value { get; }
        ValueItemStatus State { get; }
    }

    #endregion

    public class NameValueSet : IValueCols
    {
        private NameValueItem[] Items;
        private SetStatus State = SetStatus.ReadWriteInsert;
        private Int16 ItemsInUse = 0;

        #region Contructors
        public NameValueSet(ICollection Keys)
        {
            String[] keys = new String[Keys.Count];

            Keys.CopyTo(keys, 0);

            //ValueSet(keys);
        }

        public NameValueSet(string[] Keys)
        {
            Items = new NameValueItem[Keys.Length];

            for (int n = 0; n < Items.GetLength(0); n++)
            {
                Items[n] = new NameValueItem(Keys[n], null, ValueItemStatus.ReadOnly);
                ItemsInUse++;
            }
        }

        // Construct the ValueSet with the desired maximum number of items (columns).
        // The number of items cannot change for the life time of instance.
        public NameValueSet(Int32 maxItems)
        {
            Items = new NameValueItem[maxItems];
        }

        public NameValueSet()
        {
        }
        #endregion

        #region IValueCols Members
        public bool IsFixedSize { get { return (State == SetStatus.ReadOnly || State == SetStatus.ReadWrite); } }
        public bool IsReadOnly { get { return (State == SetStatus.ReadOnly); } }
        public ICollection Keys
        {
            get
            {
                // Return an array where each item is a key.
                String[] keys = new String[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                {
                    keys[n] = Items[n].name;
                }
                return (keys);
            }
        }
        public ICollection Values
        {
            get
            {
                // Return an array where each item is a value.
                Object[] values = new Object[ItemsInUse];
                for (Int32 n = 0; n < ItemsInUse; n++)
                {
                    values[n] = Items[n].value;
                }
                return (values);
            }
        }

        public object this[string key]
        {
            get
            {
                // If this key is in the value set, return its value.
                Int32 index;

                ValidateKey(key); // Check for legal key - throw exception as required
                if (GetKeyIndex(key, out index))
                {
                    // The key was found; return its value.
                    return Items[index].value;
                }
                else
                {
                    // The key was not found; return null.
                    return null;
                }
            }

            set
            {
                // If this key is in the dictionary, change its value. 
                Int32 index;

                ValidateKey(key); // Check for legal key - throw exception as required
                if (GetKeyIndex(key, out index))
                {
                    ValidateMde(SetStatus.ReadWrite); // Check if mode permits alteration of a column

                    // The key was found; change its value.
                    Items[index].value = value;
                }
                else
                {
                    // This key is not in the dictionary; add this key/value pair.
                    Add(key, value);
                }
            }
        }

        public void Add(string key, object value)
        {
            // Add the new key/value pair even if this key already exists in the dictionary.
            Add(key, value, ValueItemStatus.ReadOnly);
        }

        public void Add(string key, object value, ValueItemStatus state)
        {

            ValidateKey(key); // Check for legal key - throw exception as required
            ValidateMde(SetStatus.ReadWriteInsert);    // Check if mode permits addition of a column
            ValidateAdd(key); // Check whether safe to add this key

            Items[ItemsInUse++] = new NameValueItem(key, value, state);
        }

        public void Clear()
        {
            ValidateMde(SetStatus.ReadWriteDelete);   // Check if mode permits deletion of each column

            // Set to empty
            ItemsInUse = 0;

            // Any contained objects will be released on overwrite or on disposal
        }

        public bool Contains(string key)
        {
            Int32 index;

            ValidateKey(key); // Check for legal key - throw exception as required

            return (GetKeyIndex(key, out index));
        }

        public string Value(string key)
        {
            Int32 index;

            ValidateKey(key); // Check for legal key - throw exception as required

            GetKeyIndex(key, out index);

            if (index >= 0)
                return (string)Items[index].value;
            else
                return String.Empty;

        }

        public IValueColsEnumerator GetEnumerator()
        {
            // Construct and return an enumerator.
            return new ValueSetEnumerator(this);
        }

        public void Remove(string key)
        {
            ValidateKey(key); // Check for legal key - throw exception as required
            ValidateMde(SetStatus.ReadWriteDelete);   // Check if mode permits addition of a column

            // Try to find the key in the DictionaryEntry array
            Int32 index;
            if (GetKeyIndex(key, out index))
            {
                //              if ( Items[index].State != ValueItemStatus.ReadWriteDelete )
                //                  throw new NotSupportedException("Object element is not deletable");

                // If the key is found, slide all the items up.
                Array.Copy(Items, index + 1, Items, index, ItemsInUse - index - 1);
                ItemsInUse--;
            }
            else
            {
                // If the key is not in the dictionary, just return. 
            }
        }
        #endregion

        #region ICollection Members
        public bool IsSynchronized { get { return false; } }
        public object SyncRoot { get { throw new NotImplementedException(); } }
        public int Count { get { return ItemsInUse; } }
        public void CopyTo(Array array, int index) { throw new NotImplementedException(); }
        #endregion

        #region IEnumerable Members
        IEnumerator IEnumerable.GetEnumerator()
        {
            // Construct and return an enumerator.
            return (this).GetEnumerator();
        }
        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private Boolean GetKeyIndex(string key, out Int32 index)
        {
            for (index = 0; index < ItemsInUse; index++)
            {
                // If the key is found, return true (the index is also returned).
                if (Items[index].name.Equals(key)) return true;
            }

            // Key not found, return false (index should be ignored by the caller).
            return false;
        }

        private void ValidateKey(string key)
        {
            // Check for non null key
            if (key == null)
                throw new ArgumentNullException("Cannot add a null key");

            // Check for non empty key
            if (key == "")
                throw new ArgumentException("Cannot add an empty key");
        }

        private void ValidateAdd(string key)
        {
            // Realistically, should check overflow of keys during development debugging only
            // Add the new key/value pair even if this key already exists in the dictionary.
            if (ItemsInUse >= Items.Length)
                throw new InvalidOperationException("The ValueSet cannot hold any more items.");

            // Realistically, should check for duplication of keys during development debugging only
            // During normal operation, the keys are restricted to valid entries externally
            if (Contains(key))
                throw new InvalidOperationException("The ValueSet cannot hold duplicate keys.");
        }

        private void ValidateMde(SetStatus op)
        {
            switch (op)
            {
                case SetStatus.ReadOnly: // Trying to read a value - we always have read authority
                    break;

                case SetStatus.ReadWrite: // Trying to modify a value
                    if (this.IsReadOnly)
                    {
                        throw new NotSupportedException("The ValueSet can not be modified");
                    }
                    break;

                case SetStatus.ReadWriteDelete: //
                case SetStatus.ReadWriteInsert: //
                    if (this.IsReadOnly || this.IsFixedSize)
                    {
                        throw new NotSupportedException("Object is either read-only or fixed-size and unable to Remove elements");
                    }
                    break;
            }

            // Getting here means all is ok! 
            return;
        }
        #endregion

        #region Private Classes
        private class ValueSetEnumerator : IValueColsEnumerator
        {
            // A copy of the ValueSet object's key/value pairs.
            NameValueItem[] items;
            Int32 index = -1;

            #region Contructors
            public ValueSetEnumerator(NameValueSet sd)
            {
                // Make a copy of the dictionary entries currently in the ValueSet object.
                items = new NameValueItem[sd.Count];
                Array.Copy(sd.Items, 0, items, 0, sd.Count);
            }
            #endregion

            #region IValueColsEnumerator Members
            // Return the current item.
            public Object Current { get { ValidateIndex(); return items[index]; } }

            // Return the current value set entry.
            public NameValueItem Entry { get { return (NameValueItem)Current; } }

            // Return the key of the current item.
            public String Key { get { ValidateIndex(); return items[index].name; } }

            // Return the value of the current item.
            public Object Value { get { ValidateIndex(); return items[index].value; } }

            public ValueItemStatus State { get { ValidateIndex(); return (items[index].state); } }

            // Advance to the next item.
            public Boolean MoveNext()
            {
                if (index < items.Length - 1) { index++; return true; }
                return false;
            }

            // Reset the index to restart the enumeration.
            public void Reset()
            {
                index = -1;
            }
            #endregion

            #region Private Methods
            // Validate the enumeration index and throw an exception if the index is out of range.
            private void ValidateIndex()
            {
                if (index < 0 || index >= items.Length)
                {
                    throw new InvalidOperationException("Enumerator is before or after the collection.");
                }
            }
            #endregion
        }
        #endregion
    }
}
