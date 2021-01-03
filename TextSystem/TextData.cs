using UnityEngine;

namespace TextSystem
{
  [CreateAssetMenu(fileName = "TextData", menuName = "Dev Tools/TextData", order = 0)]
  public class TextData : ScriptableObject
  {
    [Header("Options")]
    public bool isBold;
    public bool isItalicized;
    public Color color = Color.white;
    public float size = 20f;
    public float charDelay = 0.15f;

    [Header("Text")]
    [TextArea(3, 10)]
    public string text;
  }
}