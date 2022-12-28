using System;
using System.Collections.Generic;

public class RNGMultimap<K, V> {
    private Dictionary<K, List<V>> data;
    private Func<V, V, bool> equalsEvaluator;
    private Random rng;

    public RNGMultimap(Func<V, V, bool> equalsEvaluator) {
        this.data = new Dictionary<K, List<V>>();
        this.equalsEvaluator = equalsEvaluator;
        this.rng = new Random();
    }

    public void Add(K key, V value) {
        if (key == null) throw new ArgumentNullException("Null Key");
        if (value == null) throw new ArgumentNullException("Null Value");

        if (!this.ContainsKey(key)) {
            this.data.Add(key, new List<V>());
        }

        this.data[key].Add(value);
    }

    public bool Remove(K key, V value) {
        if (!this.data.ContainsKey(key)) return false;

        return this.data[key].Remove(value);
    }

    public V this[K key] {
        get {
            if (key == null) throw new ArgumentException("Key can't be null");
            if (!ContainsKey(key)) throw new IndexOutOfRangeException("Invalid Key: " + key);

            int index = this.rng.Next(0, this.data[key].Count);
            return this.data[key][index];
        }
    }

    public bool ContainsKey(K key) { return this.data.ContainsKey(key); }

    public bool ContainsValue(V value) {
        foreach (K key in this.data.Keys) {
            foreach (V item in this.data[key]) {
                if (this.equalsEvaluator(item, value)) return true;
            }
        }
        return false;
    }

    public K[] GetKeys() {
        K[] keys = new K[this.data.Keys.Count];
        int i = 0;
        foreach (K key in this.data.Keys) {
            keys[i] = key;
            i++;
        }
        return keys;
    }
}
