using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarryUtils {
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, ISerializationCallbackReceiver {
        [Serializable]
        public struct Entry {
            public TKey key;
            public TValue value;

            // Constructor
            public Entry(TKey key, TValue value) {
                this.key = key;
                this.value = value;
            }
        }

        [SerializeField] private List<Entry> _entries = new();
        private Dictionary<TKey, TValue> _dictionary;

        public SerializableDictionary() {
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary) {
            _dictionary = new Dictionary<TKey, TValue>(dictionary);
        }

        private bool ContainsDuplicatedKeys() {
            return _entries.GroupBy(e => e.key).Where(g => g.Count() > 1).Select(g => g.Key).Any();
        }


        #region Dictionary implementation
        public TValue this[TKey key] {
            get => ((IDictionary<TKey, TValue>)_dictionary)[key];
            set => ((IDictionary<TKey, TValue>)_dictionary)[key] = value;
        }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;
        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;
        public int Count => ((IDictionary<TKey, TValue>)_dictionary).Count;
        public bool IsReadOnly => ((IDictionary<TKey, TValue>)_dictionary).IsReadOnly;
        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;
        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

        public bool ContainsKey(TKey key) => ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
        public bool TryGetValue(TKey key, out TValue value) => ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
        public void Add(TKey key, TValue value) => ((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
        public bool Remove(TKey key) => ((IDictionary<TKey, TValue>)_dictionary).Remove(key);
        public void Clear() => ((IDictionary<TKey, TValue>)_dictionary).Clear();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => ((IDictionary<TKey, TValue>)_dictionary).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<TKey, TValue>)_dictionary).GetEnumerator();

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Contains(item);
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Add(item);
        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) => ((IDictionary<TKey, TValue>)_dictionary).Remove(item);
        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
        #endregion

        #region Serialization callbacks
        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            if (!ContainsDuplicatedKeys()) {
                _entries.Clear();
                foreach (var e in this)
                    _entries.Add(new Entry(e.Key, e.Value));
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            if (!ContainsDuplicatedKeys()) {
                _dictionary.Clear();
                foreach (var e in _entries)
                    _dictionary.Add(e.key, e.value);
            }
        }
        #endregion
    }
}