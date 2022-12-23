using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    [System.Serializable]
    public class CellData {
        public string name;
        public Sprite tileSprite;
    }

    [System.Serializable]
    public class RuleData {
        public string CellName;
        [Header("Allowable Adjacent Cells")]
        public string[] north;
        public string[] northEast;
        public string[] east;
        public string[] southEast;
        public string[] south;
        public string[] southWest;
        public string[] west;
        public string[] northWest;

        public int CalculateMaxEntropy() {
            int result = 0;
            result += north.Length;
            result += northEast.Length;
            result += east.Length;
            result += southEast.Length;
            result += south.Length;
            result += southWest.Length;
            result += west.Length;
            result += northWest.Length;
            return result;
        }

        public string[] this[int i] {
            get { return GetRuleDirection(i); }
        }

        public bool ContainsSearchInDirection(string key, int direction) {
            return Helpers.ArrayContains<string>(GetRuleDirection(direction), key);
        }

        private string[] GetRuleDirection(int direction) {
            switch (direction) {
                case 0:
                    return this.north;
                case 1:
                    return this.northEast;
                case 2:
                    return this.east;
                case 3:
                    return this.southEast;
                case 4:
                    return this.south;
                case 5:
                    return this.southWest;
                case 6:
                    return this.west;
                case 7:
                    return this.northWest;
                default:
                    return null;
            }
        }
    }

    public class TileData {
        public delegate bool Conditional(string cellName);

        public bool collapsed { get; private set; } = false;
        public string tileName { get; private set; } = null;
        public Vector2Int CellPosition { get; private set; }
        public List<string> possibleCells { get; private set; }
        public int EntropyLevel { get { return this.possibleCells.Count; } }

        public TileData(Vector2Int cellPosition, List<string> allCellCopy) {
            this.CellPosition = cellPosition;
            this.possibleCells = allCellCopy;
        }

        public string GetValueAndRemove(int index) {
            if (index >= this.possibleCells.Count) {
                Debug.LogException(new System.Exception("Index Out Of Bounds"));
            }

            string value = this.possibleCells[index];
            this.possibleCells.Remove(value);
            return value;
        }

        public void KeepPossibleCells(Conditional conditional) {
            List<string> newList = new List<string>();
            foreach (string cellName in this.possibleCells) {
                if (conditional(cellName)) {
                    newList.Add(cellName);
                }
            }
            this.possibleCells = newList;
        }

        public void MarkAsCollapsed() {
            this.collapsed = true;
            this.tileName = this.possibleCells[0];
        }
    }

    public static class Helpers {
        public static Vector2Int IndexToCellCoordinate(int index, int gridWidth) {
            return new Vector2Int(index / gridWidth, index % gridWidth);
        }

        public static int CellCoordinatesToIndex(int x, int y, int gridWidth) {
            return gridWidth * y + x;
        }

        public static int CellCoordinatesToIndex(Vector2Int coord, int gridWidth) {
            return CellCoordinatesToIndex(coord.x, coord.y, gridWidth);
        }

        public static int[] ConvertSurroundingCoordinates(Vector2Int center, int gridWidth) {
            int[] result = new int[8];
            result[0] = CellCoordinatesToIndex(center + new Vector2Int(-1, 1), gridWidth);
            result[1] = CellCoordinatesToIndex(center + new Vector2Int(0, 1), gridWidth);
            result[2] = CellCoordinatesToIndex(center + new Vector2Int(1, 1), gridWidth);
            result[3] = CellCoordinatesToIndex(center + new Vector2Int(1, 0), gridWidth);
            result[4] = CellCoordinatesToIndex(center + new Vector2Int(1, -1), gridWidth);
            result[5] = CellCoordinatesToIndex(center + new Vector2Int(0, -1), gridWidth);
            result[6] = CellCoordinatesToIndex(center + new Vector2Int(-1, -1), gridWidth);
            result[7] = CellCoordinatesToIndex(center + new Vector2Int(-1, 0), gridWidth);
            return result;
        }

        public static void AddNonExistingSurroundingTiles(
            ref MinHeap<TileData> updatingTiles, ref TileData[] tiles, Vector2Int center,
            int gridWidth) {
            int[] surroundingCellIndices
                = Helpers.ConvertSurroundingCoordinates(center, gridWidth);
            foreach (int i in surroundingCellIndices) {
                if (i < 0) continue;

                if (!updatingTiles.Find(tiles[i]) && !tiles[i].collapsed) {
                    updatingTiles.Add(tiles[i]);
                }
            }
        }

        public static bool ArrayContains<T>(T[] array, T searchKey) {
            foreach (T item in array) {
                if (EqualityComparer<T>.Default.Equals(item, searchKey)) return true;
            }
            return false;
        }
    }
}