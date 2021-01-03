using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CutSceneSystem
{
  public class CutSceneManager : MonoBehaviour
  {
    public SpriteRenderer sceneImageHolder;
    public CutsceneData[] cutsceneData;
    public TextSystem.TextReader reader;

    private int index = 0;

    public void Read()
    {
      if (index != cutsceneData.Length)
      {
        SceneSkimmer();
        index++;
      }
    }

    public void SceneSkimmer()
    {
      TextSystem.TextData[] data = cutsceneData[index].texts;
      if (cutsceneData[index].sceneImage != null)
        sceneImageHolder.sprite = cutsceneData[index].sceneImage;

      reader.Read(data, "Someone's name");
    }
  }
}
