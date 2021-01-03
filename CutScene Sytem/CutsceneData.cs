using UnityEngine;

namespace CutSceneSystem
{
  [CreateAssetMenu(fileName = "CutsceneData", menuName = "Dev Tools/CutsceneData", order = 0)]
  public class CutsceneData : ScriptableObject
  {
    public Sprite sceneImage;
    public TextSystem.TextData[] texts;
  }
}