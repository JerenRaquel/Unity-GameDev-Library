using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextSystem {
    public class TextParser {
        public struct TextParts {
            public string before;
            public string text;
            public string after;
        }

        public static TextParts ApplyEffects(TextData.TextPart part, FontData defaultData) {
            TextParts result;
            FontData fontData = part.fontData;

            if (part.fontData == null) {
                fontData = defaultData;
            }

            string bBuffer = "";
            string aBuffer = "";

            if (fontData.isBold) {
                bBuffer += "<b>";
            }
            if (fontData.isItalicized) {
                bBuffer += "<i>";
            }
            bBuffer += "<color=#" + ColorUtility.ToHtmlStringRGB(fontData.color) + ">";
            bBuffer += "<size=" + fontData.size + ">";
            result.before = bBuffer;

            result.text = part.text;

            aBuffer += "</size></color>";
            if (fontData.isItalicized) {
                aBuffer += "</i>";
            }
            if (fontData.isBold) {
                aBuffer += "</b>";
            }
            result.after = aBuffer;

            return result;
        }

        public static string ApplyEffectsAsString(TextData.TextPart part, FontData defaultFont) {
            TextParts parts = ApplyEffects(part, defaultFont);
            return parts.before + parts.text + parts.after;
        }
    }
}
