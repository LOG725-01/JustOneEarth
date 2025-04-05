using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreObserver : Observer
{
    [SerializeField] private Slider slider;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PlayerType playerType;

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
        if (subject.TryGetComponent<Player>(out var player))
        {
            if (player.PlayerType != playerType) return;
            if (slider.maxValue < player.Points) { Display((int)slider.maxValue); }
            else if (player.Points <= 0) { Display(0); }
            else { Display(player.Points); }
        }
        
    }
}
