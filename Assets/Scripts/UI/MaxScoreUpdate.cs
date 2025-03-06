using TMPro;
using UnityEngine;
using UnityEngine.UI;

// @brief Met à jour la valeur du score à atteindre
public class MaxScoreUpdate : MonoBehaviour
{
    [SerializeField] private Slider slider;

    [SerializeField] private TextMeshProUGUI maxScoreText;

    public void SetTotalScore(int value)
    {
        slider.maxValue = value;
        maxScoreText.text = value.ToString();
    }

}
