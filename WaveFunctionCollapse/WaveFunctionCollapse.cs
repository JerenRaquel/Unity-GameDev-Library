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
        private Stack<TileData> updatingTiles;
        private string[] allCellNames;
        private Grid<TileData> tiles;
        private int tilesInSuperPostion;

        public void Initialize(Vector2Int gridSize) {
            this.spawnedSprites = new Queue<Sprite>();
            this.cellDict = new Dictionary<string, Sprite>();
            this.ruleDict = new Dictionary<string, RuleData>();
            this.tiles = new Grid<TileData>(gridSize.x, gridSize.y);
            this.updatingTiles = new Stack<TileData>();
            this.allCellNames = new string[this.cellData.Length];

            for (int i = 0; i < this.cellData.Length; i++) {
                this.cellDict.Add(this.cellData[i].name, this.cellData[i].tileSprite);
                this.allCellNames[i] = this.cellData[i].name;
            }

            foreach (RuleData rule in this.rules) {
                this.ruleDict.Add(rule.CellName, rule);
            }

            this.tilesInSuperPostion = gridSize.x * gridSize.y;
            for (int y = 0; y < gridSize.y; y++) {
                for (int x = 0; x < gridSize.x; x++) {
                    this.tiles[x, y] = new TileData(new Vector2Int(x, y), ref this.allCellNames);
                }
            }
        }

        public void Generate() {
            // Find lowest entropy
            TileData lowestEntropyTile = FetchLowestEntropyTile();
            // Collapse superposition
            CollapseTile(lowestEntropyTile);
            // Proprogate data
            ProprogateData(lowestEntropyTile.CellPosition);
        }

        public Sprite GetNextTile() {
            if (this.spawnedSprites.Count <= 0) return null;
            return this.spawnedSprites.Dequeue();
        }

        private TileData FetchLowestEntropyTile() {
            List<int> lowestIndices = new List<int>();
            int minEntropyLevel = int.MaxValue;

            for (int i = 0; i < this.tiles.Length; i++) {
                if (this.tiles[i].collapsed) continue;

                int currentTileEntropyLevel = this.tiles[i].EntropyLevel;
                if (currentTileEntropyLevel < minEntropyLevel) {
                    lowestIndices.Clear();
                    lowestIndices.Add(i);
                    minEntropyLevel = currentTileEntropyLevel;
                } else if (currentTileEntropyLevel == minEntropyLevel) {
                    lowestIndices.Add(i);
                }
            }

            if (lowestIndices.Count == 0) {
                Debug.LogException(new System.Exception("No lower entropy than max int..."));
            }

            int rng = Random.Range(0, lowestIndices.Count);
            return this.tiles[lowestIndices[rng]];
        }

        private void CollapseTile(TileData tileData) {
            int rng = Random.Range(0, tileData.EntropyLevel);
            string chosenCellData = tileData.GetValueAndRemove(rng);
            CollapseCell(tileData, this.cellDict[chosenCellData]);
        }

        private void ProprogateData(Vector2Int origin) {
            AddNonExistingSurroundingTiles(origin.x, origin.y, this.tiles.x);

            while (this.updatingTiles.Count > 0) {
                TileData tile = updatingTiles.Pop();
                // Lower the entropy and add surroundings if entropy changed
                if (LowerEntropy(tile)) {
                    AddNonExistingSurroundingTiles(tile.x, tile.y, this.tiles.x);
                }
            }
        }

        private void CollapseCell(TileData tileData, Sprite sprite) {
            if (tileData.collapsed) return;

            tileData.MarkAsCollapsed();
            this.tilesInSuperPostion--;
            this.spawnedSprites.Enqueue(sprite);
        }

        private bool LowerEntropy(TileData tileData) {
            int entropyLevelBefore = tileData.EntropyLevel;
            UpdateTileBasedOnRule(tileData);

            if (entropyLevelBefore == 1) {
                CollapseCell(tileData, this.cellDict[tileData.tileName]);
                return true;
            }

            return false;
        }

        private void UpdateTileBasedOnRule(TileData tileData) {
            int[] surroundingCells = this.tiles.GetSurroundingCellIndices(tileData.x, tileData.y);
            tileData.KeepPossibleCells((string possibleCellName) => {
                for (int dir = 0; dir < 8; dir++) {
                    int tileIndex = surroundingCells[dir];
                    if (tileIndex < 0) continue;
                    if (!this.tiles[tileIndex].collapsed) continue;

                    RuleData rule = this.ruleDict[possibleCellName];
                    if (!rule.ContainsSearchInDirection(this.tiles[tileIndex].tileName, dir)) {
                        return false;
                    }
                }
                return true;
            });
        }

        private void AddNonExistingSurroundingTiles(int x, int y, int gridWidth) {
            int[] surroundingCells = this.tiles.GetSurroundingCellIndices(x, y);
            foreach (int i in surroundingCells) {
                if (i < 0 || i >= this.tiles.Length) continue;
                if (this.updatingTiles.Count > 0
                    && this.updatingTiles.Contains(this.tiles[i])) continue;
                if (this.tiles[i].collapsed) continue;

                this.updatingTiles.Push(this.tiles[i]);
            }
        }
    }
}
