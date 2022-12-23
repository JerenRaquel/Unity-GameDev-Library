using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    public class WaveFunctionCollapse : MonoBehaviour {
        [SerializeField] private CellData[] cellData;
        [SerializeField] private RuleData[] rules;

        // Internal Data //
        private Dictionary<string, CellData> cellDict;
        private Dictionary<string, RuleData> ruleDict;
        private MinHeap<TileData> updatingTiles;
        private List<string> allCellNames;
        private TileData[] tiles;
        private int tilesInSuperPostion;
        private Vector2Int gridSize;

        private void Initialize(Vector2Int gridSize) {
            this.cellDict = new Dictionary<string, CellData>();
            this.ruleDict = new Dictionary<string, RuleData>();
            this.tiles = new TileData[gridSize.x * gridSize.y];
            this.updatingTiles = new MinHeap<TileData>(
                this.gridSize.x * this.gridSize.y,
                (TileData LHS, TileData RHS) => LHS.EntropyLevel < RHS.EntropyLevel,
                (TileData LHS, TileData RHS) => LHS.EntropyLevel == RHS.EntropyLevel
            );
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

        public Sprite GenerateCell() {
            if (this.tilesInSuperPostion <= 0) return null;

            // Find lowest entropy
            TileData lowestEntropyTile = FetchLowestEntropyTile();
            // Collapse superposition
            Sprite tile = CollapseTile(lowestEntropyTile);
            // Proprogate data
            ProprogateData(lowestEntropyTile.CellPosition);

            return tile;
        }

        private TileData FetchLowestEntropyTile() {
            List<int> lowestIndices = new List<int>();
            int minEntropyLevel = int.MaxValue;

            for (int i = 0; i < this.tiles.Length; i++) {
                if (this.tiles[i].collapsed) continue;

                int currentTileEntropyLevel = this.tiles[i].EntropyLevel;
                if (currentTileEntropyLevel < minEntropyLevel) {
                    if (lowestIndices.Count > 0
                        && this.tiles[lowestIndices[0]].EntropyLevel > currentTileEntropyLevel) {
                        lowestIndices.Clear();
                    }
                    lowestIndices.Add(i);
                }
            }

            if (lowestIndices.Count == 0) {
                Debug.LogException(new System.Exception("UHHHHHHHHHHH"));
            }

            int rng = Random.Range(0, lowestIndices.Count);
            return this.tiles[lowestIndices[rng]];
        }

        private Sprite CollapseTile(TileData tileData) {
            int rng = Random.Range(0, tileData.possibleCells.Count);
            string chosenCellData = tileData.GetValueAndRemove(rng);
            tileData.MarkAsCollapsed();
            this.tilesInSuperPostion--;
            return this.cellDict[chosenCellData].tileSprite;
        }

        private void ProprogateData(Vector2Int origin) {
            Helpers.AddNonExistingSurroundingTiles(
                ref this.updatingTiles,
                ref this.tiles,
                origin,
                this.gridSize.x
            );

            while (!this.updatingTiles.IsEmpty()) {
                TileData tile = updatingTiles.Pop();
                // Lower the entropy and add surroundings if entropy changed
                if (LowerEntropy(tile)) {
                    Helpers.AddNonExistingSurroundingTiles(
                        ref this.updatingTiles,
                        ref this.tiles,
                        tile.CellPosition,
                        this.gridSize.x
                    );
                }
            }
        }

        private bool LowerEntropy(TileData tileData) {
            int entropyLevelBefore = tileData.EntropyLevel;
            UpdateTileBasedOnRule(tileData);

            if (tileData.EntropyLevel != entropyLevelBefore) {
                if (entropyLevelBefore == 1) {
                    tileData.MarkAsCollapsed();
                    this.tilesInSuperPostion--;
                }
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
    }
}
