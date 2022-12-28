using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace WaveFunctionCollapse {
    [CustomEditor(typeof(RuleData))]
    public class RuleDataEditor : Editor {

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            RuleData rd = (RuleData)target;

            rd.overridedDirections
                = (RuleData.Directions)EditorGUILayout.EnumFlagsField(
                    "Override Directions", rd.overridedDirections);
            if (rd.overridedDirections != 0) {
                EditorGUI.indentLevel++;
                RuleDirectionField("north", rd.overridedDirections);
                RuleDirectionField("northEast", rd.overridedDirections);
                RuleDirectionField("east", rd.overridedDirections);
                RuleDirectionField("southEast", rd.overridedDirections);
                RuleDirectionField("south", rd.overridedDirections);
                RuleDirectionField("southWest", rd.overridedDirections);
                RuleDirectionField("west", rd.overridedDirections);
                RuleDirectionField("northWest", rd.overridedDirections);
                EditorGUI.indentLevel--;
            }
        }

        private void RuleDirectionField(string name, RuleData.Directions flag) {
            int val = (int)System.Enum.Parse(typeof(RuleData.Directions), name);
            if ((val & (int)flag) == 0) return;

            SerializedProperty baseObj = serializedObject.FindProperty(name);
            EditorGUILayout.LabelField(Fancify(name));
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(baseObj.FindPropertyRelative("data"), true);
            EditorGUI.indentLevel--;
        }

        private string Fancify(string text) {
            var r = new Regex(@"(?<=[A-Z])(?=[A-Z][a-z]) |
                                (?<=[^A-Z])(?=[A-Z]) |
                                (?<=[A-Za-z])(?=[^A-Za-z])",
                                RegexOptions.IgnorePatternWhitespace);

            string[] words = r.Replace(text, " ").Split(" ");
            string result = "";
            for (int i = 0; i < words.Length; i++) {
                result += char.ToUpper(words[i][0]);
                result += words[i].Substring(1);
                if (i != words.Length) result += " ";
            }
            return result;
        }
    }
}