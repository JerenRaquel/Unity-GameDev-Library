using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStar {
    public class AStar : MonoBehaviour {
        #region Class Instance
        public static AStar instance = null;
        private void CreateInstance() {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }
        #endregion
        private void Awake() {
            CreateInstance();
            nodeDataMap = new Dictionary<string, NodeData>();
            foreach (NodeData data in this.nodeData) {
                this.nodeDataMap.Add(data.tag, data);
            }
        }

        public NodeData[] nodeData;
        [Header("Settings")]
        public bool debug = false;
        public bool displayAsWire = false;
        public bool displayOnSelected = false;
        public Color defaultColor;
        public Vector2 lineWidthEnds;
        public Gradient lineColor;

        private Dictionary<string, NodeData> nodeDataMap;
        private LineRenderer lineRenderer;
        private Vector2Int[] neighbors = {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        private void Start() {
            GameObject go = new GameObject();
            go.transform.parent = this.transform;
            this.lineRenderer = go.AddComponent<LineRenderer>();
            this.lineRenderer.colorGradient = this.lineColor;
            this.lineRenderer.startWidth = lineWidthEnds.x;
            this.lineRenderer.endWidth = lineWidthEnds.y;
            go.SetActive(false);
        }

        public NodeMap GenerateNodeMap(
            Vector2 origin, Vector2Int size, float spacing, Transform parent,
            string startTag, string goalTag, string obstacleTag) {
            NodeMap map = new NodeMap(origin, size);

            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    GameObject go = new GameObject();
                    Node node = go.AddComponent<Node>();
                    node.Generate(
                        spacing,
                        new Vector2Int(x, y),
                        startTag,
                        goalTag,
                        obstacleTag,
                        ref map
                    );
                    map.nodes[x, y] = node;
                    go.transform.position = new Vector3(
                        x + origin.x - (size.x / 2),
                        y + origin.y - (size.y / 2),
                        0
                    );
                    go.transform.parent = parent;
                }
            }

            return map;
        }

        public List<Vector2> FindPath(ref NodeMap map, ref Node origin, ref Node goal) {
            if (origin == null || goal == null || map == null) return null;
            // Reset all nodes
            map.Reset();

            // Create the data structures
            MinHeap<Node> openSet = new MinHeap<Node>(
                map.gridSize.x * map.gridSize.y,
                (Node lhs, Node rhs) => lhs.fCost < rhs.fCost,
                (Node lhs, Node rhs) => lhs == rhs
            );
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

            // Setup inital node
            origin.gcost = 0;
            origin.fCost = Heuristic(origin, goal);
            openSet.Add(origin);

            // Start searching
            while (!openSet.IsEmpty()) {
                Node current = openSet.Pop();
                // Goal found!
                if (current.index == goal.index) return RecontructPath(ref cameFrom, ref current);

                // Search
                foreach (Vector2Int neighborPosition in this.neighbors) {
                    // Get x, y coords relative to grid
                    int x = neighborPosition.x + current.index.x;
                    int y = neighborPosition.y + current.index.y;

                    // Skip out of bounds
                    if (x < 0 || y < 0 || x >= map.x || y >= map.y) continue;

                    // Get the neighbor and calculate new g and f scores
                    Node neighbor = map[x, y];
                    // Check if one can go there
                    if (neighbor.IsObstacle) continue;
                    CalculateNeighborWeights(
                        ref cameFrom, ref openSet, ref current, ref neighbor, ref goal);
                }
            }
            Debug.LogError("AStar::FindPath: AStar Failure");
            return null;
        }

        public NodeData RetrieveNodeData(string tag) {
            if (this.nodeDataMap.ContainsKey(tag)) {
                return this.nodeDataMap[tag];
            } else {
                return null;
            }
        }

        public void DisplayLine(List<Vector2> path) {
            this.lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++) {
                Vector2 position = path[path.Count - i - 1];
                this.lineRenderer.SetPosition(i, new Vector3(position.x, position.y, 0));
            }
            this.lineRenderer.gameObject.SetActive(true);
        }

        private List<Vector2> RecontructPath(
            ref Dictionary<Node, Node> cameFromMap, ref Node current) {
            List<Vector2> path = new List<Vector2>();

            Node c = current;
            foreach (Node node in cameFromMap.Keys) {
                if (cameFromMap.ContainsKey(c)) {
                    Vector2 position = cameFromMap[c].position;
                    c = cameFromMap[c];
                    path.Add(position);
                } else { break; } //! FAILURE
            }

            return path;
        }

        private void CalculateNeighborWeights(
            ref Dictionary<Node, Node> cameFrom, ref MinHeap<Node> openSet, ref Node current,
            ref Node neighbor, ref Node goal
        ) {
            float score = current.gcost + Heuristic(current, neighbor);
            if (score < neighbor.gcost) {
                if (!cameFrom.ContainsKey(neighbor)) {
                    cameFrom.Add(neighbor, current);
                } else {
                    cameFrom[neighbor] = current;
                }
                neighbor.gcost = score;
                neighbor.fCost = score + Heuristic(neighbor, goal);
                if (!openSet.Find(neighbor)) {
                    openSet.Add(neighbor);
                }
            }
        }

        private float Heuristic(Node a, Node b) {
            return Vector2.Distance(a.position, b.position);
        }
    }
}
