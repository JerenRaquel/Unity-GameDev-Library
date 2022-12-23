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
            foreach (string item in GetRuleDirection(direction)) {
                if (item == key) return true;
            }
            return false;
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
            if (index >= this.possibleCells.Count) return null;

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
            if (newList.Count == 1) {
                this.tileName = newList[0];
                this.collapsed = true;
            } else {
                this.possibleCells = newList;
            }
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
            Vector2Int[] eightCells = {
                new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 1),
                new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1),
                new Vector2Int(-1, -1), new Vector2Int(-1, 0),
            };
            for (int i = 0; i < 8; i++) {
                result[i] = CellCoordinatesToIndex(center + eightCells[i], gridWidth);
            }
            return result;
        }
    }
}