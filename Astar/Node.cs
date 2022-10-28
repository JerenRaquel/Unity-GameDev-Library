using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    [System.Serializable]
    public class NodeMap {
        public Node[,] nodes { get; } = null;
        public Vector2Int gridSize { get; }
        public Vector2 origin { get; }
        public int x { get { return this.nodes.GetLength(0); } }
        public int y { get { return this.nodes.GetLength(1); } }
        public Node start = null;
        public Node goal = null;

        public NodeMap(Vector2 origin, Vector2Int gridSize) {
            this.gridSize = gridSize;
            this.nodes = new Node[gridSize.x, gridSize.y];
        }

        public void Reset() {
            foreach (var node in this.nodes) {
                node.Reset();
            }
        }

        public Node this[int x, int y] { get { return this.nodes[x, y]; } }
    }

    [System.Serializable]
    public class NodeData {
        public string tag;
        public Color gizmoColor;
    }

    public class Node : MonoBehaviour {
        [HideInInspector]
        public Vector3 position {
            get { return this.transform.position; }
            set { this.transform.position = value; }
        }
        [HideInInspector] public float fCost = float.MaxValue;
        [HideInInspector] public float gcost = float.MaxValue;
        [HideInInspector] public Vector2Int index;
        private Rigidbody2D rb = null;
        private BoxCollider2D bc = null;
        private NodeData nodeData = null;
        private NodeMap map = null;
        private string startTag;
        private string goalTag;

        public void Generate(
            float size, Vector2Int index, string startTag, string goalTag, ref NodeMap map) {
            this.rb = this.gameObject.AddComponent<Rigidbody2D>();
            this.rb.bodyType = RigidbodyType2D.Kinematic;
            this.rb.freezeRotation = true;
            this.bc = this.gameObject.AddComponent<BoxCollider2D>();
            this.bc.size = new Vector2(size, size);
            this.bc.isTrigger = true;
            this.index = index;
            this.map = map;
            this.startTag = startTag;
            this.goalTag = goalTag;
        }

        public void Reset() {
            this.fCost = float.MaxValue;
            this.gcost = float.MaxValue;
        }

        private void OnDrawGizmos() {
            if (AStar.instance.debug && !AStar.instance.displayOnSelected) {
                DrawGizmos();
            }
        }

        private void OnDrawGizmosSelected() {
            if (AStar.instance.debug && AStar.instance.displayOnSelected) {
                DrawGizmos();
            }
        }

        private void DrawGizmos() {
            Color storedColor = Gizmos.color;
            if (nodeData == null) {
                Gizmos.color = AStar.instance.defaultColor;
            } else {
                Gizmos.color = nodeData.gizmoColor;
            }
            if (AStar.instance.displayAsWire) {
                Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 0));
            } else {
                Gizmos.DrawCube(transform.position, new Vector3(1, 1, 0));
            }
            Gizmos.color = storedColor;
        }

        private void SetNode(string tag) {
            if (tag == startTag) {
                this.map.start = this;
            }
            if (tag == goalTag) {
                this.map.goal = this;
            }
            this.nodeData = AStar.instance.RetrieveNodeData(tag);
        }

        private void UnassignNode() {
            if (this.map.start == this) {
                this.map.start = null;
            }
            if (this.map.goal == this) {
                this.map.goal = null;
            }
            this.nodeData = null;
        }

        private void OnTriggerEnter2D(Collider2D other) {
            SetNode(other.tag);
        }

        private void OnTriggerStay2D(Collider2D other) {
            if (nodeData != null && other.tag == nodeData.tag) return;
            SetNode(other.tag);
        }

        private void OnTriggerExit2D(Collider2D other) {
            UnassignNode();
        }
    }
}
