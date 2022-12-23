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
            for (int i = 0; i < 8; i++) {
                result += this[i].Length;
            }
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
        public int x { get { return this.CellPosition.x; } }
        public int y { get { return this.CellPosition.y; } }
        public string[] possibleCells { get; private set; }
        public int EntropyLevel { get { return this.possibleCells.Length; } }

        public TileData(Vector2Int cellPosition, ref string[] allCellCopy) {
            this.CellPosition = cellPosition;
            this.possibleCells = new string[allCellCopy.Length];
            for (int i = 0; i < allCellCopy.Length; i++) {
                this.possibleCells[i] = allCellCopy[i];
            }
        }

        public string GetValueAndRemove(int index) {
            if (index >= this.possibleCells.Length) return null;

            string value = this.possibleCells[index];
            string[] newCells = new string[this.possibleCells.Length - 1];
            for (int i = 0, c = 0; i < newCells.Length; i++) {
                if (i != index) {
                    newCells[c++] = this.possibleCells[i];
                }
            }
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
                MarkAsCollapsed(newList[0]);
            } else {
                this.possibleCells = newList.ToArray();
            }
        }

        public void MarkAsCollapsed() {
            MarkAsCollapsed(this.possibleCells[0]);
        }

        private void MarkAsCollapsed(string name) {
            this.collapsed = true;
            this.tileName = name;
        }
    }
}