using UnityEngine;

namespace WaveFunctionCollapse {
    [CreateAssetMenu(fileName = "RuleData", menuName = "Game Dev Library/RuleData", order = 0)]
    public class RuleData : ScriptableObject {
        [System.Serializable]
        public class RuleDirection {
            [SerializeField] public string[] data;
            public int Length { get { return data.Length; } }
            public bool isValid { get { return (this.data != null); } }
        }
        public enum Directions {
            north = 0b00000001,
            northEast = 0b00000010,
            east = 0b00000100,
            southEast = 0b00001000,
            south = 0b00010000,
            southWest = 0b00100000,
            west = 0b01000000,
            northWest = 0b10000000
        }

        public string CellName;
        [Header("Allowable Adjacent Cells")]
        [SerializeField] private RuleDirection allDirections;
        [HideInInspector] public Directions overridedDirections;
        [HideInInspector] public RuleDirection north;
        [HideInInspector] public RuleDirection northEast;
        [HideInInspector] public RuleDirection east;
        [HideInInspector] public RuleDirection southEast;
        [HideInInspector] public RuleDirection south;
        [HideInInspector] public RuleDirection southWest;
        [HideInInspector] public RuleDirection west;
        [HideInInspector] public RuleDirection northWest;

        public bool CanBeNeighbor(string neighbor, int direction) {
            RuleDirection ruleDirection = GetRuleDirection(direction);
            if (neighbor == this.CellName) {
                return true;
            }

            foreach (string possibleNeighbor in ruleDirection.data) {
                if (possibleNeighbor == neighbor) return true;
            }
            return false;
        }

        private RuleDirection GetRuleDirection(int direction) {
            if (this.overridedDirections == 0) return this.allDirections;

            switch (direction) {
                case 0:
                    if (this.north.isValid || this.north.Length == 0)
                        return this.allDirections;
                    return this.north;
                case 1:
                    if (this.northEast.isValid || this.northEast.Length == 0)
                        return this.allDirections;
                    return this.northEast;
                case 2:
                    if (this.east.isValid || this.east.Length == 0)
                        return this.allDirections;
                    return this.east;
                case 3:
                    if (this.southEast.isValid || this.southEast.Length == 0)
                        return this.allDirections;
                    return this.southEast;
                case 4:
                    if (this.south.isValid || this.south.Length == 0)
                        return this.allDirections;
                    return this.south;
                case 5:
                    if (this.southWest.isValid || this.southWest.Length == 0)
                        return this.allDirections;
                    return this.southWest;
                case 6:
                    if (this.west.isValid || this.west.Length == 0)
                        return this.allDirections;
                    return this.west;
                case 7:
                    if (this.northWest.isValid || this.northWest.Length == 0)
                        return this.allDirections;
                    return this.northWest;
                default:
                    return null;
            }
        }
    }
}