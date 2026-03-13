using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KszUtil
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ICollection<SerializableKeyValuePair<TKey, TValue>>, ISerializationCallbackReceiver
    {
        public TValue this[TKey key]
        {
            get => KeyValuePairs.FirstOrDefault(pair => pair.Key.Equals(key)).Value;
            set => Add(new SerializableKeyValuePair<TKey, TValue>(key, value));
        }

        public ICollection<TKey> Keys => KeyValuePairs.Select(x => x.Key).ToArray();
        public ICollection<TValue> Values => KeyValuePairs.Select(x => x.Value).ToArray();
        public int Count => KeyValuePairs.Count;
        public bool IsReadOnly => false;

        [SerializeField] protected internal List<SerializableKeyValuePair<TKey, TValue>> keyValuePairs;

        private List<SerializableKeyValuePair<TKey, TValue>> KeyValuePairs =>
            // 空だった場合は再構築する
            keyValuePairs == default || !keyValuePairs.Any()
                ? keyValuePairs = CreateSerializableKeyValuePairList()
                : keyValuePairs;

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            foreach (var pair in dictionary)
            {
                this[pair.Key] = pair.Value;
            }
        }

        public IEnumerator<SerializableKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return KeyValuePairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = true;
            if (!ContainsKey(key))
            {
                this[key] = default;
                result = false;
            }

            value = this[key];
            return result;
        }

        public void Add(SerializableKeyValuePair<TKey, TValue> item)
        {
            var firstIndex = KeyValuePairs.FirstIndexOrNull(pair => pair.Key.Equals(item.key));
            if (firstIndex.HasValue)
            {
                KeyValuePairs[firstIndex.Value] = item;
            }
            else
            {
                KeyValuePairs.Add(item);
            }
        }

        public void Clear()
        {
            KeyValuePairs.Clear();
        }

        public bool Contains(SerializableKeyValuePair<TKey, TValue> item)
        {
            return KeyValuePairs.Contains(item);
        }

        public void CopyTo(SerializableKeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            KeyValuePairs.CopyTo(array, arrayIndex);
        }

        public bool Remove(SerializableKeyValuePair<TKey, TValue> item)
        {
            return KeyValuePairs.Remove(item);
        }

        public bool Remove(TKey key)
        {
            if (!ContainsKey(key))
            {
                return false;
            }

            var item = KeyValuePairs.FirstOrDefault(x => x.Key.Equals(key));
            return Remove(item);
        }

        public bool ContainsKey(TKey key)
        {
            return KeyValuePairs.Any(pair => pair.Key.Equals(key));
        }

        public bool ContainsValue(TValue value)
        {
            return KeyValuePairs.Any(pair => pair.Value.Equals(value));
        }

        public TValue GetOrCreate(TKey key)
        {
            if (!TryGetValue(key, out var value))
            {
                value = this[key] = Activator.CreateInstance<TValue>();
            }

            return value;
        }

        public IDictionary<TKey, TValue> ToDictionary()
        {
            return this.ToDictionary(x => x.Key, x => x.Value);
        }

        protected virtual List<SerializableKeyValuePair<TKey, TValue>> CreateSerializableKeyValuePairList()
        {
            return new List<SerializableKeyValuePair<TKey, TValue>>();
        }

        protected virtual bool ValidateKeys()
        {
            return true;
        }

        protected SerializableKeyValuePair<TKey, TValue> CreateSerializableKeyValuePair(TKey key)
        {
            return keyValuePairs != default && keyValuePairs.Any(pair => pair.Key.Equals(key))
                ? new SerializableKeyValuePair<TKey, TValue>(key, keyValuePairs.First(pair => pair.Key.Equals(key)).Value)
                : new SerializableKeyValuePair<TKey, TValue>(key);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (keyValuePairs == default || !keyValuePairs.Any() || !ValidateKeys())
            {
                keyValuePairs = CreateSerializableKeyValuePairList();
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (keyValuePairs == default || !keyValuePairs.Any() || !ValidateKeys())
            {
                keyValuePairs = CreateSerializableKeyValuePairList();
            }
        }
    }

    /// <summary>
    /// A dictionary of enums and variants that can be serialized.
    /// Note: If you changes int value of enum, serialized items can't follow these changes.
    /// </summary>
    /// <typeparam name="TKey">Type of enum to using as key of dictionary</typeparam>
    /// <typeparam name="TValue">Type of some variant as value of dictionary</typeparam>
    [Serializable]
    public class EnumSerializableDictionary<TKey, TValue> : SerializableDictionary<TKey, TValue>
        where TKey : Enum
    {
        protected override List<SerializableKeyValuePair<TKey, TValue>> CreateSerializableKeyValuePairList()
        {
            return Enum.GetValues(typeof(TKey)).Cast<TKey>()
                .Select(CreateSerializableKeyValuePair)
                .ToList();
        }

        protected override bool ValidateKeys()
        {
            if (keyValuePairs == default || !keyValuePairs.Any())
            {
                return false;
            }

            var enumValues = Enum.GetValues(typeof(TKey)).Cast<TKey>().ToList();
            return
                keyValuePairs.All(pair => enumValues.Contains(pair.Key)) &&
                enumValues.All(key => keyValuePairs.Exists(x => Equals(x.Key, key)));
        }
    }

    /// <summary>
    /// A dictionary of boolean and variants that can be serialized.
    /// </summary>
    /// <typeparam name="TValue">Type of some variant as value of dictionary</typeparam>
    [Serializable]
    public class BoolSerializableDictionary<TValue> : SerializableDictionary<bool, TValue>
    {
        protected override List<SerializableKeyValuePair<bool, TValue>> CreateSerializableKeyValuePairList()
        {
            return new List<SerializableKeyValuePair<bool, TValue>>
            {
                CreateSerializableKeyValuePair(false),
                CreateSerializableKeyValuePair(true),
            };
        }
    }

    [Serializable]
    public struct SerializableKeyValuePair<TKey, TValue>
    {
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] internal TKey key;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] internal TValue value;

        public TKey Key => key;

        public TValue Value
        {
            get => value;
            set => this.value = value;
        }

        public SerializableKeyValuePair(TKey key)
        {
            this.key = key;
            value = CanCreateValueInstance() ? Activator.CreateInstance<TValue>() : default;
        }

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        internal static bool CanCreateValueInstance()
        {
            var valueType = typeof(TValue);
            return
                valueType.GetConstructor(new Type[] { }) != default &&
                !typeof(MonoBehaviour).IsAssignableFrom(valueType) &&
                !typeof(ScriptableObject).IsAssignableFrom(valueType) &&
                !typeof(GameObject).IsAssignableFrom(valueType) &&
                !typeof(Component).IsAssignableFrom(valueType);
        }

        public void Deconstruct(out TKey k, out TValue v)
        {
            k = key;
            v = value;
        }
    }

    [Serializable]
    public class SerializableList<T> : List<T>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> _list;

        public void OnBeforeSerialize()
        {
            _list = this;
        }

        public void OnAfterDeserialize()
        {
            if (_list == null) _list = new();
            Clear();
            AddRange(_list);
            _list.Clear();
        }
    }

    [Serializable]
    public class SerializableHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<T> _list;

        public void OnBeforeSerialize()
        {
            _list = this.ToList();
        }

        public void OnAfterDeserialize()
        {
            Clear();
            if (_list == null) return;
            foreach (var item in _list)
            {
                Add(item);
            }

            _list.Clear();
        }
    }
}