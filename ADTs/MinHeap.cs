using System;

/* Min Heap design by Egor Grishechko
    Grishechko's Website: https://egorikas.com/max-and-min-heap-implementation-with-csharp/

    NOTES:
      This is basically the orginal minheap that Grishechko created; however, I took the time to alter this in able to allow any data type to be used.
    The original was only capable of the data type int. This now allows for any data type as seen as T.

    Docs:
      For the constructor:
        size: the max size of the heap, must be allocated [may be changed to allow for a list, but research is required]
        lessThanFunc: this is some function defined by the user to determin how to organinize the heap. Allows to determine what part of the data type
                      T to be seen as the smaller one. [Classes and structs are supported this way]
        equal: This is the same as the lessThanFunc, but to determine equality.
      IsEmpty(): Checks if the heap is empty
      Count(): Returns the amount of added items
      Peek(): Returns the smallest T in the heap, if applicable
      Pop(): Returns the smallest T in the heap and removes it from the heap, if applicable
      Add(T): Adds to the heap
      Find(T): returns true if T was found in the heap
      ForEach(Iterator accessor(in T)): Apply a function on each element; every element is immutable
      Every(Accessor accessor(in T)): Apply a conditional function on each immutable element; stop on the first false state
 */

public class MinHeap<T> {
    public delegate bool Comparator(T lhs, T rhs);
    public delegate void Iterator(in T item);
    public delegate bool Accessor(in T item);

    private Comparator _comparatorLess;
    private Comparator _comparatorEqual;
    private readonly T[] _elements;
    private int _size;

    public MinHeap(int size, Comparator lessThanFunc, Comparator equal) {
        _elements = new T[size];
        _comparatorLess = lessThanFunc;
        _comparatorEqual = equal;
    }

    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    private T GetLeftChild(int elementIndex) => _elements[GetLeftChildIndex(elementIndex)];
    private T GetRightChild(int elementIndex) => _elements[GetRightChildIndex(elementIndex)];
    private T GetParent(int elementIndex) => _elements[GetParentIndex(elementIndex)];

    private void Swap(int firstIndex, int secondIndex) {
        var temp = _elements[firstIndex];
        _elements[firstIndex] = _elements[secondIndex];
        _elements[secondIndex] = temp;
    }

    public bool IsEmpty() { return (_size == 0); }

    public int Count { get { return _size; } }

    public T Peek() {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        return _elements[0];
    }

    public T Pop() {
        if (_size == 0)
            throw new IndexOutOfRangeException();

        var result = _elements[0];
        _elements[0] = _elements[_size - 1];
        _size--;

        ReCalculateDown();

        return result;
    }

    public void Add(T element) {
        if (_size == _elements.Length)
            throw new IndexOutOfRangeException();

        _elements[_size] = element;
        _size++;

        ReCalculateUp();
    }

    public bool Find(T element) {
        if (_size == 0)
            return false;

        for (int i = 0; i < _size; i++) {
            if (_comparatorEqual(element, _elements[i]))
                return true;
        }
        return false;
    }

    public void ForEach(Iterator accessor) {
        for (int i = 0; i < _size; i++) {
            accessor(_elements[i]);
        }
    }

    public bool Every(Accessor accessor) {
        bool trueForAll = true;
        for (int i = 0; i < _size; i++) {
            if (!accessor(_elements[i])) {
                trueForAll = false;
                break;
            }
        }
        return trueForAll;
    }

    private void ReCalculateDown() {
        int index = 0;
        while (HasLeftChild(index)) {
            var smallerIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && _comparatorLess(GetRightChild(index), GetLeftChild(index))) {
                smallerIndex = GetRightChildIndex(index);
            }

            if (!_comparatorLess(_elements[smallerIndex], _elements[index]))  // !(_elements[smallerIndex] >= _elements[index])
                break;

            Swap(smallerIndex, index);
            index = smallerIndex;
        }
    }

    private void ReCalculateUp() {
        var index = _size - 1;
        while (!IsRoot(index) && _comparatorLess(_elements[index], GetParent(index))) {
            var parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }
}