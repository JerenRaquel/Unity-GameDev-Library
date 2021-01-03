using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ContainerBars
{
  public delegate void CallBack();

  public enum TYPE { STATIC, COUNT_UP, COUNT_DOWN }

  [System.Serializable]
  public struct ProgressColor
  {
    public Color color;
    public float percentageMin;
    public float percentageMax;
  }

  public class ProgressSystem : MonoBehaviour
  {
    #region Data Fields
    // Data
    [SerializeField]
    private Slider bar;
    [SerializeField]
    private TMPro.TMP_Text text;
    [SerializeField]
    private Image forground;
    [SerializeField]
    private Image background;
    [SerializeField]
    private ProgressColor[] progressColors;
    [SerializeField]
    private Color backgroundColor;
    [SerializeField]
    private string barName;
    [SerializeField]
    private float initialValue;
    [SerializeField]
    private float finalValue;
    [SerializeField]
    private float rate = 1f;
    [SerializeField]
    private float updateDelay;
    [SerializeField]
    private TYPE type;
    private CallBack callback;

    // Internal
    private float startTime;
    private float percentage;
    private float totalValue;
    private float currentValue;
    private float internalDelayScore;
    private bool isComplete;
    private bool isInitialized = false;
    #endregion

    private void Update()
    {
      if (this.type != TYPE.STATIC)
      {
        if (!this.isComplete && this.isInitialized && this.internalDelayScore <= Time.time)
        {
          this.internalDelayScore = Time.time + updateDelay;

          this.currentValue += rate;
          this.percentage = this.currentValue / this.totalValue * 100f;

          SetColorBars();

          switch (this.type)
          {
            case TYPE.COUNT_UP:
              this.bar.value = NormalizeValue();
              break;
            case TYPE.COUNT_DOWN:
              this.bar.value = 1f - NormalizeValue();
              break;
          }

          if (percentage >= 100)
            this.isComplete = true;
        }
      }
      if (this.isInitialized && this.isComplete)
        Message();
    }

    #region Methods
    public void Initialize(CallBack callback = null)
    {
      this.text.text = this.barName;
      this.callback = callback;
      this.startTime = Time.time;
      this.internalDelayScore = this.startTime;
      this.percentage = 0f;
      this.currentValue = 0f;
      this.totalValue = this.finalValue - this.initialValue;
      if (this.totalValue < 0)
        this.totalValue *= -1;
      this.isComplete = false;
      this.isInitialized = true;
      this.background.color = this.backgroundColor;
      SetColorBars();
    }

    public void AddToProgressAsPercentage(float percentageValue)
    {
      this.percentage += percentageValue;
      this.percentage = Mathf.Clamp(this.percentage, 0, 100);
    }

    public void AddToProgressAsValue(float value)
    {
      this.currentValue += value;
      this.currentValue = Mathf.Clamp(this.currentValue, this.initialValue, this.finalValue);
    }

    public void SetRateAsAPercentage(float percentageRate) => this.rate = (percentageRate / 100) * this.totalValue;

    public void CompleteNow() => this.isComplete = true;

    public void SetProgressValue(float value)
    {
      this.currentValue = value;
      this.percentage = this.currentValue / this.totalValue * 100f;
      this.bar.value = NormalizeValue();
    }

    public void DebugValues()
    {
      if (!this.IsComplete)
        Debug.Log("Value: " + this.CurrentValue + " | " + this.Percentage + "%");
    }

    public void SetColorBars()
    {
      float percent = this.type != TYPE.COUNT_DOWN ? this.percentage : 100f - this.percentage;
      for (int i = 0; i < this.progressColors.Length; i++)
      {
        if (percent >= this.progressColors[i].percentageMin && percent <= this.progressColors[i].percentageMax)
        {
          this.forground.color = this.progressColors[i].color;
          break;
        }
      }
    }

    private float NormalizeValue() => this.percentage / 100f;

    private void Message()
    {
      this.isInitialized = false;
      if (this.callback != null)
        this.callback();
    }
    #endregion

    #region GettersSetters
    public TYPE Type => this.type;
    public float Percentage
    {
      get => this.percentage;
      set => this.percentage = Mathf.Clamp(value, 0, 100);
    }
    public float Rate { get => this.rate; set => this.rate = value; }
    public float CurrentValue
    {
      get
      {
        if (this.type != TYPE.COUNT_DOWN)
          return this.currentValue;
        else
          return this.totalValue - this.currentValue;
      }
    }
    public float TimeTaken => Time.time - this.startTime;
    public bool IsInitialized => this.isInitialized;
    public bool IsComplete => this.isComplete;
    #endregion
  }
}
