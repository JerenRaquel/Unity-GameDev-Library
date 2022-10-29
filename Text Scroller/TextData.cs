using UnityEngine;

namespace TextSystem {

    [CreateAssetMenu(fileName = "TextData", menuName = "Game Dev Library/TextData", order = 0)]
    public class TextData : ScriptableObject {
        [System.Serializable]
        public class TextPart {
            public FontData fontData;
            [Multiline]
            public string text;
            public float charDelay;
        }

        public TextPart[] parts;
    }
}

