using System;

public class Grid<T> {
    #region Variables
    // Consts
    public delegate void Enerumerator(in T item);
    private int[,] eightCells = {
        {-1, 1}, {0, 1}, {1, 1},
        {1, 0}, {1, -1}, {0, -1},
        {-1, -1}, {-1, 0}
    };

    // Publics
    public int Length { get { return data.Length; } }
    public int x { get { return this.width; } }
    public int y { get { return this.height; } }

    // Privates
    private int width;
    private int height;
    private T[] data;
    #endregion Variables

    public Grid(int width, int height) {
        this.width = width;
        this.height = height;
        this.data = new T[width * height];
    }

    #region Operator Overloading
    public T this[int rawIndex] {
        get { return this.data[rawIndex]; }
        set { this.data[rawIndex] = value; }
    }

    public T this[int x, int y] {
        get { return this[FlattenCoordinate(x, y, this.width)]; }
        set { this[FlattenCoordinate(x, y, this.width)] = value; }
    }
    #endregion Operator Overloading

    #region Static Methods
    public static int FlattenCoordinate(int x, int y, int width) {
        return width * y + x;
    }

    public static void ConvertIndexToCoordinate(int i, int width, out int x, out int y) {
        x = (int)System.MathF.Floor((float)i / (float)width);
        y = i % width;
    }
    #endregion Static Methods

    #region Member Methods
    public T[] GetSurroundingCells(int x, int y) {
        T[] surroundingCells = new T[8];
        for (int i = 0; i < 8; i++) {
            int dx = eightCells[i, 0] + x;
            int dy = eightCells[i, 1] + y;

            if (dx < 0 || dx >= this.width) continue;
            if (dy < 0 || dy >= this.height) continue;

            surroundingCells[i] = this[dx, dy];
        }
        return surroundingCells;
    }

    public K[] GetSurroundingCellData<K>(int x, int y, K fillerVar, Func<T, K> accessor) {
        K[] gatheredData = new K[8];
        for (int i = 0; i < 8; i++) {
            int dx = eightCells[i, 0] + x;
            int dy = eightCells[i, 1] + y;

            if (dx < 0 || dx >= this.width || dy < 0 || dy >= this.height) {
                gatheredData[i] = fillerVar;
            } else {
                gatheredData[i] = accessor(this[dx, dy]);
            }
        }
        return gatheredData;
    }

    public int[] GetSurroundingCellIndices(int x, int y) {
        int[] surroundingCells = new int[8];
        for (int i = 0; i < 8; i++) {
            surroundingCells[i]
                = FlattenCoordinate(eightCells[i, 0], eightCells[i, 1], this.width);
        }
        return surroundingCells;
    }

    public void ForEach(Enerumerator function) {
        foreach (T item in this.data) {
            function(in item);
        }
    }
    #endregion Member Methods
}
