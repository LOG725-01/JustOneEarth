using TMPro;
using UnityEngine;

public class PlayerTurnUi : MonoBehaviour
{

    public static PlayerTurnUi Instance;

    [SerializeField] private TextMeshProUGUI turnText;
    
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    public void Civilisation()
    {
        turnText.text = "Tour de la Civilisation";
    }

    public void World()
    {
        turnText.text = "Tour du Monde";
    }
}
