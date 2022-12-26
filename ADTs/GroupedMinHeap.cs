using System.Collections.Generic;

/*
    Description: This is a data structure that acts like a MinHeap and Dictionary<ID, List>.

    Notes: The initial purpose of this data structure is to hold a group of items for a "key" 
        acting as an ID. However, the keys are organized in min ordered by ID. The ID is defined
        on the size of each element. Each group will be fetched by random order from that group.
        This was made with Wave Function Collapse in mind.

    Docs:
        For the constructor:
            maxKeySize: The max amount of keys that can be held
            lengthIdentifier: How to evaluate the size of an element
        IsEmpty(): Checks if there is no data stored
        Count(): Returns the amount of added items
        Peek(): Returns a random T from the smallest group, if applicable
        Pop(): Returns a random T from the smallest group and removes it from the group, 
            if applicable
        Add(T): Adds to a group
        Find(T): Returns true if T was found
        Update(): Reorders the internals; lazy calculation [Plans for eager calculations]
*/

public class GroupedMinHeap<T> {
    public delegate int Length(T item);

    public int Count { get; private set; } = 0;

    private int maxKeySize;
    private Dictionary<int, List<T>> data;
    private MinHeap<int> keys;
    private Length lengthIdentifier;
    private System.Random rng;

    public GroupedMinHeap(int maxKeySize, Length lengthIdentifier) {
        this.data = new Dictionary<int, List<T>>();
        this.keys = new MinHeap<int>(
            maxKeySize,
            (int LHS, int RHS) => LHS < RHS,
            (int LHS, int RHS) => LHS == RHS
        );
        this.rng = new System.Random();
        this.lengthIdentifier = lengthIdentifier;
        this.maxKeySize = maxKeySize;
    }

    public bool IsEmpty() { return (this.Count == 0); }

    public T Peek() {
        if (this.Count <= 0) {
            throw new System.IndexOutOfRangeException("No Data Availiable");
        }

        int key = this.keys.Peek();
        int amt = this.data[key].Count;
        if (amt == 0) {
            throw new System.Exception("UnSynced Data");
        }
        int rngIndex = this.rng.Next(0, amt);
        return this.data[key][rngIndex];
    }

    public T Pop() {
        T item = this.Peek();
        this.data[this.keys.Peek()].Remove(item);
        this.Count--;

        if (this.data[this.keys.Peek()].Count <= 0) {
            this.keys.Pop();
        }

        return item;
    }

    public void Add(T item) {
        int key = this.lengthIdentifier(item);

        if (!this.keys.Find(key)) {
            if (this.keys.Count >= this.maxKeySize) {
                throw new System.IndexOutOfRangeException("Can't Hold any more unique data");
            }
            if (!this.data.ContainsKey(key)) {
                this.data.Add(key, new List<T>());
            }
            this.keys.Add(key);
        }

        this.data[key].Add(item);
        this.Count++;
    }

    public bool Find(T item) {
        if (this.Count <= 0) return false;

        // Stop once found, returns true if never found
        return !this.keys.Every((in int key) => {
            return !this.data[key].Contains(item);
        });
    }

    public void Update() {
        GroupedMinHeap<T> newGroupedMinHeap = new GroupedMinHeap<T>(
            this.maxKeySize, this.lengthIdentifier
        );

        // Copy keys
        int[] oldKeys = new int[this.keys.Count];
        int count = 0;
        this.keys.ForEach((in int key) => {
            oldKeys[count] = key;
            count++;
        });

        foreach (int key in oldKeys) {
            foreach (T item in this.data[key]) {
                newGroupedMinHeap.Add(item);
            }
        }

        this.Count = newGroupedMinHeap.Count;
        this.data = newGroupedMinHeap.data;
        this.keys = newGroupedMinHeap.keys;
    }
}
