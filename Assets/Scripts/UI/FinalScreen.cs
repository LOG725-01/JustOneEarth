using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinalScreenUI : MonoBehaviour
{
    public static FinalScreenUI Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text endMessage;
    [SerializeField] private Button toMenuButton;
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    private void Start()
    {
        panel.SetActive(false);
    }

    public void Show(string winnerName)
    {
        panel.SetActive(true);
        endMessage.text = $"{winnerName} won !";
    }
}
