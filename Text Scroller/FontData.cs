using UnityEngine;

namespace TextSystem {
    [CreateAssetMenu(fileName = "FontData", menuName = "Game Dev Library/FontData", order = 0)]
    public class FontData : ScriptableObject {
        public bool isBold;
        public bool isItalicized;
        public Color color;
        public int size;
    }
}