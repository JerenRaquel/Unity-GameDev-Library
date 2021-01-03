using UnityEngine;

namespace TextSystem
{
  public static class TextParser
  {
    private enum TEXT_OPTION { BOLD, ITALICIZED, SIZE, COLOR }

    public struct TextParts
    {
      public string before;
      public string after;
      public string text;
    }

    public static TextParts FormatText(TextData data)
    {
      TextParts parts;
      string result = "";

      if (data.isBold)
        result += "<b>";
      if (data.isItalicized)
        result += "<i>";
      result += "<color=#" + ColorUtility.ToHtmlStringRGB(data.color) + ">";
      result += "<size=" + data.size + ">";

      parts.before = result;
      parts.text = data.text;

      result = "";

      result += "</size></color>";
      if (data.isItalicized)
        result += "</i>";
      if (data.isBold)
        result += "</b>";

      parts.after = result;

      return parts;
    }
  }
}