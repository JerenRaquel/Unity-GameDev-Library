using UnityEngine;
using UnityEditor;

namespace WaveFunctionCollapse {
    [CustomEditor(typeof(GridManager))]
    public class GridManagerEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GridManager gm = (GridManager)target;

            gm.enableDebugging
                = EditorGUILayout.Toggle("Enable Debugging", gm.enableDebugging);

            if (gm.enableDebugging) {
                EditorGUI.indentLevel++;
                gm.createOnStartUp = EditorGUILayout.Toggle("Create On Start Up", gm.createOnStartUp);
                gm.createAllAtOnce = EditorGUILayout.Toggle("Create All", gm.createAllAtOnce);

                if (gm.count < gm.max) {
                    if (GUILayout.Button("Generate Next Tile")) {
                        gm.PlaceNextTile();
                    }
                    if (GUILayout.Button("Generate Remaining Tiles")) {
                        gm.PlaceRest();
                    }
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}