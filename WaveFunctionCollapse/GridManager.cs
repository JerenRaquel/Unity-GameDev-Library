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
        [HideInInspector]
        public bool Complete {
            get {
                if (this.cells == null) {
                    return false;
                } else {
                    return this.count >= this.cells.Length;
                }
            }
        }

        [Header("Debugging Options")]
        [HideInInspector] public bool enableDebugging = false;
        [HideInInspector] public bool disableDebugButtons = false;
        [HideInInspector] public float tilePlacementDelayDebug;

        private int count = 0;

        // Internals //
        private WaveFunctionCollapse wfc;
        private Grid<GameObject> cells;

        private void Start() {
            this.wfc = GetComponent<WaveFunctionCollapse>();
            Initialize();
        }

        public void Initialize() {
            this.cells = new Grid<GameObject>(this.gridSize.x, this.gridSize.y);
            this.wfc.Initialize(this.gridSize);
            this.wfc.Generate();
        }

        public void PlaceNextTile() {
            if (this.Complete) return;
            int index = this.count;
            this.count++;
            WFCResultData tileData = this.wfc.GetNextTile();
            if (tileData == null) {
                Debug.LogException(new System.Exception("No Data Found"));
                    return;
                }

            Vector3 pos = new Vector3(
                    tileData.gridPosition.x + this.gridOffset.x,
                    tileData.gridPosition.y + this.gridOffset.y, 0
                ) * cellWidth;
            this.cells[index] = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
            this.cells[index].GetComponent<SpriteRenderer>().sprite = tileData.sprite;
            this.cells[index].name = tileData.sprite.name + "( " + pos.x + ", " + pos.y + " )";
        }

        public void PlaceRest() {
            StartCoroutine(_PlaceRest());
        }

        private IEnumerator _PlaceRest() {
            this.disableDebugButtons = true;
            while (!this.Complete) {
                PlaceNextTile();
                yield return new WaitForSeconds(this.tilePlacementDelayDebug);
            }
            this.disableDebugButtons = false;
        }
    }
}
