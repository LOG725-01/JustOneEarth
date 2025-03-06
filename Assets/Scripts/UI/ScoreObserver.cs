using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreObserver : Observer
{
    [SerializeField] private Slider slider;

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        Display((int) slider.value);
    }

    private void Display(int score)
    {
        slider.value = score;
        scoreText.text = score.ToString();
    }

    public override void ObserverUpdate(GameObject subject)
    {
        throw new System.NotImplementedException();
        int score = 0;
        if (slider.maxValue < score) { Display((int)slider.maxValue); }
        else if (score <= 0) { Display(0); }
        else { Display(score); }
    }
}
