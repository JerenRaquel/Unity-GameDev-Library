using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TextSystem
{
  public class TextReader : MonoBehaviour
  {
    public TMPro.TMP_Text textBox;
    public TMPro.TMP_Text titleBox;

    private void Start()
    {
      this.titleBox.text = "";
      this.textBox.text = "";
    }

    public void Read(TextData[] data, string title = null)
    {
      if (title != null)
        this.titleBox.text = "<b>" + title + "</b>";
      StartCoroutine(TextRead(data));
    }

    private IEnumerator TextRead(TextData[] data)
    {
      string buffer = "";
      for (int i = 0; i < data.Length; i++)
      {
        TextSystem.TextParser.TextParts text = TextSystem.TextParser.FormatText(data[i]);

        string result = "";
        for (int j = 0; j < text.text.Length + 1; j++)
        {
          result = text.before;
          for (int k = 0; k < j; k++)
          {
            result += text.text[k];
          }
          result += text.after;

          this.textBox.text = buffer + result;
          yield return new WaitForSeconds(data[i].charDelay);
        }
        buffer += result + " ";
      }
    }
  }
}
