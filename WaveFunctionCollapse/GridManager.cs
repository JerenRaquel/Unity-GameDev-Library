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
