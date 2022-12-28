using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    public class WaveFunctionCollapse : MonoBehaviour {
        [SerializeField] private CellData[] cellData;
        [SerializeField] private RuleData[] rules;

        // Internal Data //
        private Queue<WFCResultData> spawnedData;
        private Dictionary<string, Sprite> cellDict;
        private Dictionary<string, RuleData> ruleDict;
        private Grid<TileData> tiles;
        private GroupedMinHeap<TileData> groupedTiles;
        private string[] allCellNames;
        private Stack<TileData> updatingTiles;

        public void Initialize(Vector2Int gridSize) {
            this.spawnedData = new Queue<WFCResultData>();
            this.cellDict = new Dictionary<string, Sprite>();
            this.ruleDict = new Dictionary<string, RuleData>();
            this.tiles = new Grid<TileData>(gridSize.x, gridSize.y);
            this.groupedTiles = new GroupedMinHeap<TileData>(
                gridSize.x * gridSize.y,
                (TileData tileData) => tileData.EntropyLevel
            );
            this.allCellNames = new string[this.cellData.Length];
            this.updatingTiles = new Stack<TileData>();

            for (int i = 0; i < this.cellData.Length; i++) {
                this.cellDict.Add(this.cellData[i].name, this.cellData[i].tileSprite);
                this.allCellNames[i] = this.cellData[i].name;
            }

            foreach (RuleData rule in this.rules) {
                this.ruleDict.Add(rule.CellName, rule);
            }

            for (int y = 0; y < gridSize.y; y++) {
                for (int x = 0; x < gridSize.x; x++) {
                    this.tiles[x, y]
                        = new TileData(new Vector2Int(x, y), ref this.allCellNames);
                    this.groupedTiles.Add(this.tiles[x, y]);
                }
            }
        }

        public WFCResultData GetNextTile() {
            if (this.spawnedData.Count <= 0) return null;
            return this.spawnedData.Dequeue();
        }

        public void Generate() {
            while (this.groupedTiles.Count > 0) {
                // Find lowest entropy
                TileData lowestEntropyTile = this.groupedTiles.Pop();

                if (!lowestEntropyTile.collapsed) {
                    // Collapse superposition
                    CollapseTile(lowestEntropyTile);
                    // Proprogate data
                    AddSurroundingTiles(lowestEntropyTile.x, lowestEntropyTile.y);
                    ProprogateData();
                    this.groupedTiles.Update();
                }
            }
        }

        private void CollapseTile(TileData tileData) {
            if (tileData.collapsed) return;

            tileData.Collapse();
            this.spawnedData.Enqueue(new WFCResultData(
                this.cellDict[tileData.tileName],
                tileData.CellPosition
            ));
        }

        private void AddSurroundingTiles(int x, int y) {
            TileData[] surroundingTiles = this.tiles.GetSurroundingCells(x, y);
            foreach (TileData tile in surroundingTiles) {
                if (tile == null) continue;

                if (!this.updatingTiles.Contains(tile)) {
                    this.updatingTiles.Push(tile);
                }
            }
        }

        private void ProprogateData() {
            while (this.updatingTiles.Count > 0) {
                TileData tile = this.updatingTiles.Pop();
                if (tile == null) {
                    throw new System.Exception("Popped Invalid Tile!");
                }

                string[] surroundingTileNames
                    = this.tiles.GetSurroundingCellData<string>(
                        tile.x, tile.y, null, TileData.NameAccessor);

                bool collapsed = tile.KeepPossibleCells((string cellName) => {
                    return KeepDecider(cellName, surroundingTileNames);
                });
            }
        }

        private bool KeepDecider(string self, string[] neighbors) {
            for (int i = 0; i < neighbors.Length; i++) {
                if (neighbors[i] == null) continue; // Tile isn't collapsed

                bool canExist = this.ruleDict[self].CanBeNeighbor(neighbors[i], i);
                if (!canExist) return false;
            }
            return true;
        }
    }
}
