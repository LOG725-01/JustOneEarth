using TMPro;
using UnityEngine;
using UnityEngine.UI;

// @brief Met � jour la valeur du score � atteindre
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
