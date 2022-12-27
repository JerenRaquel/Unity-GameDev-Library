using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    public class WaveFunctionCollapse : MonoBehaviour {
        [SerializeField] private CellData[] cellData;
        [SerializeField] private RuleData[] rules;

        // Internal Data //
        private Queue<Sprite> spawnedSprites;
        private Dictionary<string, Sprite> cellDict;
        private Dictionary<string, RuleData> ruleDict;
        private Grid<TileData> tiles;
        private GroupedMinHeap<TileData> groupedTiles;
        private string[] allCellNames;
        private Stack<TileData> updatingTiles;

        public void Initialize(Vector2Int gridSize) {
            this.spawnedSprites = new Queue<Sprite>();
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

        public Sprite GetNextTile() {
            if (this.spawnedSprites.Count <= 0) return null;
            return this.spawnedSprites.Dequeue();
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
            this.spawnedSprites.Enqueue(this.cellDict[tileData.tileName]);
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

                tile.KeepPossibleCells((string cellName) => {
                    return KeepDecider(cellName, surroundingTileNames);
                });
            }
        }

        //! BUGS
        private bool KeepDecider(string tileName, string[] surroundingTileNames) {
            for (int i = 0; i < surroundingTileNames.Length; i++) {
                if (surroundingTileNames[i] == null) continue;

                bool canExist
                    = this.ruleDict[tileName].ContainsSearchInDirection(
                        surroundingTileNames[i], i
                    );
                if (!canExist) {
                    Debug.Log(tileName + " can not be with " + surroundingTileNames[i]);
                    return false;
                }
            }

            return true;
        }
    }
}
