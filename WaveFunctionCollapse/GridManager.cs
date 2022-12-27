using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse {
    [RequireComponent(typeof(WaveFunctionCollapse))]
    public class GridManager : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2Int gridOffset;
        [SerializeField] private float cellWidth;
        [SerializeField] private GameObject cellPrefab;

        [Header("Debugging Options")]
        [HideInInspector] public bool enableDebugging = false;
        [HideInInspector] public bool createOnStartUp = false;
        [HideInInspector] public bool createAllAtOnce = false;
        [HideInInspector] public bool nextTile = false;
        [HideInInspector] public int count { get; private set; } = 0;
        [HideInInspector] public int max { get; private set; }

        // Internals //
        private WaveFunctionCollapse wfc;
        private GameObject[] cells;

        private void Start() {
            this.wfc = GetComponent<WaveFunctionCollapse>();
            this.max = this.gridSize.x * gridSize.y;
            if (this.createOnStartUp) Initialize();
        }

        public void Initialize() {
            this.cells = new GameObject[this.gridSize.x * this.gridSize.y];
            this.wfc.Initialize(this.gridSize);
            this.wfc.Generate();
            if (this.createAllAtOnce) {
                for (int i = 0; i < this.max; i++) {
                    PlaceNextTile();
                }
            }
        }

        public void PlaceNextTile() {
            this.nextTile = false;
            int index = this.count;
            this.count++;
            Sprite tileSprite = this.wfc.GetNextTile();
                if (tileSprite == null) {
                    Debug.LogException(new System.Exception("No Sprite Found"));
                    return;
                }

            Grid<TileData>.ConvertIndexToCoordinate(index, this.gridSize.x, out int x, out int y);
            this.cells[index] = Instantiate(
                cellPrefab,
                new Vector3(x + this.gridOffset.x, y + this.gridOffset.y, 0) * cellWidth,
                Quaternion.identity,
                transform
            );
            this.cells[index].GetComponent<SpriteRenderer>().sprite = tileSprite;
            this.cells[index].name = tileSprite.name;
        }

        public void PlaceRest() {
            while (this.count < this.max) {
                PlaceNextTile();
            }
        }
    }
}
