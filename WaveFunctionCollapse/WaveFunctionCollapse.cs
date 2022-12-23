using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    public class WaveFunctionCollapse : MonoBehaviour {
        [SerializeField] private CellData[] cellData;
        [SerializeField] private RuleData[] rules;

        // Internal Data //
        private Queue<Sprite> spawnedSprites;
        private Dictionary<string, CellData> cellDict;
        private Dictionary<string, RuleData> ruleDict;
        private Stack<TileData> updatingTiles;
        private List<string> allCellNames;
        private TileData[] tiles;
        private int tilesInSuperPostion;
        private Vector2Int gridSize;

        public void Initialize(Vector2Int gridSize) {
            this.spawnedSprites = new Queue<Sprite>();
            this.cellDict = new Dictionary<string, CellData>();
            this.ruleDict = new Dictionary<string, RuleData>();
            this.tiles = new TileData[gridSize.x * gridSize.y];
            this.updatingTiles = new Stack<TileData>();
            this.allCellNames = new List<string>();
            this.gridSize = gridSize;

            foreach (CellData cd in this.cellData) {
                this.cellDict.Add(cd.name, cd);
                this.allCellNames.Add(cd.name);
            }

            foreach (RuleData rule in this.rules) {
                this.ruleDict.Add(rule.CellName, rule);
            }

            this.tilesInSuperPostion = gridSize.x * gridSize.y;
            for (int y = 0; y < gridSize.y; y++) {
                for (int x = 0; x < gridSize.x; x++) {
                    this.tiles[Helpers.CellCoordinatesToIndex(x, y, gridSize.x)]
                        = new TileData(new Vector2Int(x, y), this.allCellNames);
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
            int rng = Random.Range(0, tileData.possibleCells.Count);
            string chosenCellData = tileData.GetValueAndRemove(rng);
            CollapseCell(tileData, this.cellDict[chosenCellData].tileSprite);
        }

        private void ProprogateData(Vector2Int origin) {
            AddNonExistingSurroundingTiles(origin, this.gridSize.x);

            while (this.updatingTiles.Count > 0) {
                TileData tile = updatingTiles.Pop();
                // Lower the entropy and add surroundings if entropy changed
                if (LowerEntropy(tile)) {
                    AddNonExistingSurroundingTiles(tile.CellPosition, this.gridSize.x);
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
                CollapseCell(tileData, this.cellDict[tileData.tileName].tileSprite);
                return true;
            }

            return false;
        }

        private void UpdateTileBasedOnRule(TileData tileData) {
            int[] surroundingCells
                = Helpers.ConvertSurroundingCoordinates(tileData.CellPosition, this.gridSize.x);
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

        private void AddNonExistingSurroundingTiles(Vector2Int center, int gridWidth) {
            int[] surroundingCellIndices
                = Helpers.ConvertSurroundingCoordinates(center, gridWidth);
            foreach (int i in surroundingCellIndices) {
                if (i < 0 || i >= this.tiles.Length) continue;

                if (this.updatingTiles.Count > 0
                    && !this.updatingTiles.Contains(this.tiles[i])
                    && !this.tiles[i].collapsed) {
                    this.updatingTiles.Push(this.tiles[i]);
                }
            }
        }
    }
}
