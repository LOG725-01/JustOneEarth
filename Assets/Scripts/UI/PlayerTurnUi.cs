using TMPro;
using UnityEngine;

public class PlayerTurnUi : MonoBehaviour
{

    public static PlayerTurnUi Instance;

    [SerializeField] private TextMeshProUGUI turnText;

    private PlayerType currentPlayerTurn;
    
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
    }

    public void SetTurn(PlayerType currentPlayer)
    {
        switch (currentPlayer)
        {
            case PlayerType.Civilisation:
                Civilisation();
                break;
            case PlayerType.World:
                World();
                break;
        }
    }

    public PlayerType NextTurn()
    {
        switch (currentPlayerTurn)
        {
            case PlayerType.Civilisation:
                World();
                break;
            case PlayerType.World:
                Civilisation();
                break;
        }
        return currentPlayerTurn;
    }

    private void Civilisation()
    {
        currentPlayerTurn = PlayerType.Civilisation;
        turnText.text = "Tour de la Civilisation";
    }

    private void World()
    {
        currentPlayerTurn = PlayerType.World;
        turnText.text = "Tour du Monde";
    }

}
