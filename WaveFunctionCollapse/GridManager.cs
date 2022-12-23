using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    [RequireComponent(typeof(WaveFunctionCollapse))]
    public class GridManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private float cellWidth;
        [SerializeField] private GameObject cellPrefab;

        [Header("Debugging Options")]
        [SerializeField] private bool createOnStartUp = false;

        // Internals //
        private WaveFunctionCollapse wfc;
        private GameObject[] cells;

        private void Start() {
            this.wfc = GetComponent<WaveFunctionCollapse>();
            if (this.createOnStartUp) Initialize();
        }

        public void Initialize() {
            this.cells = new GameObject[this.gridSize.x * this.gridSize.y];
            this.wfc.Initialize(this.gridSize);
            this.wfc.Generate();
            for (int i = 0; i < this.gridSize.x * this.gridSize.y; i++) {
                Sprite tileSprite = this.wfc.GetNextTile();
                if (tileSprite == null) {
                    Debug.LogException(new System.Exception("No Sprite Found"));
                    return;
                }

                Grid<TileData>.ConvertIndexToCoordinate(i, this.gridSize.x, out int x, out int y);
                this.cells[i] = Instantiate(
                    cellPrefab,
                    new Vector3(x, y, 0) * cellWidth,
                    Quaternion.identity,
                    transform
                );
                this.cells[i].GetComponent<SpriteRenderer>().sprite = tileSprite;
                this.cells[i].name = tileSprite.name;
            }
        }
    }
}
