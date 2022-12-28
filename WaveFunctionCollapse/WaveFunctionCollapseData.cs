using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    [System.Serializable]
    public class CellData {
        public string name;
        public Sprite tileSprite;
    }

    [System.Serializable]
    public class WFCResultData {
        public Sprite sprite;
        public Vector2Int gridPosition;

        public WFCResultData(Sprite sprite, Vector2Int gridPosition) {
            this.sprite = sprite;
            this.gridPosition = gridPosition;
        }
    }

    public class TileData {
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

        public bool KeepPossibleCells(System.Func<string, bool> conditional) {
            List<string> newList = new List<string>();
            foreach (string cellName in this.possibleCells) {
                if (conditional(cellName)) {
                    newList.Add(cellName);
                }
            }
            if (newList.Count == 1) {
                MarkAsCollapsed(newList[0]);
                return true;
            } else {
                this.possibleCells = newList.ToArray();
                return false;
            }
        }

        public void Collapse() {
            int rng = UnityEngine.Random.Range(0, this.EntropyLevel);
            string chosenCellData = this.GetValueAndRemove(rng);
            MarkAsCollapsed(chosenCellData);
        }

        public static string NameAccessor(TileData tile) { return tile.tileName; }

        private void MarkAsCollapsed(string name) {
            this.collapsed = true;
            this.tileName = name;
        }

        private string GetValueAndRemove(int index) {
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
    }
}