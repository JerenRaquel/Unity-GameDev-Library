using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class Pathfinder : MonoBehaviour {
        [Header("Settings")]
        public Transform nodeParent;
        public Vector2Int gridSize;
        public float cellSize;
        public string startingTag;
        public string goalTag;
        public string obstacleTag;
        [Header("Debug")]
        public bool enableDebugLine;

        private NodeMap map;

        private void Start() {
            this.map = AStar.instance.GenerateNodeMap(
                transform.position,
                this.gridSize,
                this.cellSize,
                this.nodeParent,
                this.startingTag,
                this.goalTag,
                this.obstacleTag
            );

            if (this.map == null) {
                Debug.LogError("Node map not created!!!");
            }
        }

        private void Update() {
            if (enableDebugLine) {
                List<Vector2> path = AStar.instance.FindPath(
                    ref map, ref map.start, ref map.goal
                );
                if (path == null) return;
                AStar.instance.DisplayLine(path);
            }
        }

        public List<Vector2> GetPath() {
            return AStar.instance.FindPath(
                ref map, ref map.start, ref map.goal
            );
        }

        public Vector3? GetNextPosition() {
            List<Vector2> path = AStar.instance.FindPath(
                ref map, ref map.start, ref map.goal
            );
            if (path == null || path.Count == 0) return null;
            if (path.Count == 1) {
                return map.goal.position;
            } else {
                return path[0];
            }
        }
    }
}
