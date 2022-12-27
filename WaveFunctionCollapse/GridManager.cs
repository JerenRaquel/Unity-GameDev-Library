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
        [SerializeField] private bool createOnStartUp = false;
        [SerializeField] private bool nextTile = false;
        [SerializeField] private float delay;
        private float current;
        private int count = 0;
        private int max;

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
            this.max = this.gridSize.x * gridSize.y;
        }

        private void Update() {
            if (this.count < this.max
                && this.nextTile && this.current <= Time.time + this.delay) {
                this.nextTile = false;
                this.current = Time.time;
                Generate(this.count);
                this.count++;
            }
        }

        private void Generate(int index) {
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
    }
}
