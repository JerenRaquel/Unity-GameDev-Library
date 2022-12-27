using UnityEngine;
using UnityEditor;

namespace WaveFunctionCollapse {
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor {
        private int progressCount = 0;

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GridManager gm = (GridManager)target;

            gm.enableDebugging
                = EditorGUILayout.Toggle("Enable Debugging", gm.enableDebugging);

            if (gm.enableDebugging) {
                EditorGUI.indentLevel++;
                gm.tilePlacementDelayDebug
                    = EditorGUILayout.FloatField("Delay per Tile", gm.tilePlacementDelayDebug);
                if (gm.disableDebugButtons) {
                    EditorGUILayout.LabelField(ProgressTick());
                } else if (!gm.Complete) {
                    if (GUILayout.Button("Generate Next Tile")) {
                        gm.PlaceNextTile();
                    }
                    if (GUILayout.Button("Generate Remaining Tiles")) {
                        gm.PlaceRest();
                    }
                } else {
                    EditorGUILayout.LabelField("Generation Complete");
                }
                EditorGUI.indentLevel--;
            }
        }

        private string ProgressTick() {
            string result = "Generation Working";
            for (int i = 0; i < progressCount; i++) {
                result += ".";
            }
            progressCount++;
            if (progressCount >= 5) {
                progressCount = 0;
            }
            return result;
        }
    }
}