using System.Collections;

namespace CsArena.Core.Collections;

public class SkipListDictionary<TKey, TValue>:IDictionary<TKey, TValue>
{
    private SkipListCollection<KeyValuePair<TKey, TValue>> _set;

    public SkipListDictionary()
        :this(Comparer<TKey>.Default)
    {
    }

    public SkipListDictionary(IComparer<TKey> comparer)
    {
        _set = new SkipListCollection<KeyValuePair<TKey, TValue>>(new KeyValuePairComparer(comparer));
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) _set).GetEnumerator();
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        _set.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _set.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        _set.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _set.Remove(item);
    }

    public int Count => _set.Count;

    public bool IsReadOnly => _set.IsReadOnly;

    // Lookups build a probe pair: KeyValuePairComparer compares keys only, so the value is never read (hence default!).
    public bool ContainsKey(TKey key)
    {
        var node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default!));
        return node != null;
    }

    // Matches Dictionary<TKey, TValue>: a null key throws ArgumentNullException, a duplicate key throws ArgumentException.
    public void Add(TKey key, TValue value)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (!_set.TryAdd(new KeyValuePair<TKey, TValue>(key, value)))
        {
            throw new ArgumentException($"An item with the same key has already been added. Key: {key}", nameof(key));
        }
    }

    public bool Remove(TKey key)
    {
        return _set.Remove(new KeyValuePair<TKey, TValue>(key, default!));
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        throw new System.NotImplementedException();
    }

    public TValue this[TKey key]
    {
        get
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            var node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default!));
            if (node == null)
            {
                throw new KeyNotFoundException();
            }
            return node.Key.Value;
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Matches Dictionary<TKey, TValue>: an existing key keeps its stored key object and gets the new value
            // (in place, so the enumeration version does not change); a missing key is added.
            var node = _set.FindNode(new KeyValuePair<TKey, TValue>(key, default!));
            if (node != null)
            {
                node.Key = new KeyValuePair<TKey, TValue>(node.Key.Key, value);
            }
            else
            {
                _set.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
        }
    }

    public ICollection<TKey> Keys { get; private set; } = null!;
    public ICollection<TValue> Values { get; private set; } = null!;

    internal class KeyValuePairComparer : Comparer<KeyValuePair<TKey, TValue>>
    {
        internal IComparer<TKey> keyComparer;

        public KeyValuePairComparer(IComparer<TKey> keyComparer)
        {
            if (keyComparer == null)
            {
                this.keyComparer = Comparer<TKey>.Default;
            }
            else
            {
                this.keyComparer = keyComparer;
            }
        }

        public override int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
        {
            return keyComparer.Compare(x.Key, y.Key);
        }
    }
}