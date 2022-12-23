public class Grid<T> {
    // Consts
    public delegate void Enerumerator(T item);
    private int[,] eightCells = {
        {-1, 1}, {0, 1}, {1, 1},
        {1, 0}, {1, -1}, {0, -1},
        {-1, -1}, {-1, 0}
    };

    // Publics
    public int Length { get { return data.Length; } }
    public int x {
        get {
            return (int)System.MathF.Floor((float)this.Length / (float)this.width);
        }
    }
    public int y {
        get {
            return (int)System.MathF.Floor((float)this.Length % (float)this.width);
        }
    }

    // Privates
    private int width;
    private int height;
    private T[] data;

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
        get { return this[this.width * y + x]; }
        set { this[this.width * y + x] = value; }
    }
    #endregion Operator Overloading

    #region Member Methods
    public T[] GetSurroundingCells(int x, int y) {
        T[] surroundingCells = new T[8];
        for (int i = 0; i < 8; i++) {
            surroundingCells[i] = this[eightCells[i, 0], eightCells[i, 1]];
        }
        return surroundingCells;
    }

    public void ForEach(Enerumerator function) {
        foreach (T item in this.data) {
            function(item);
        }
    }
    #endregion Member Methods
}
