using TMPro;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public static TileInfo Instance;

    [SerializeField] private GameObject tileInfoObject;

    [SerializeField] private TextMeshProUGUI tileTypeText;
    [SerializeField] private TextMeshProUGUI ressourceText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI ownerText;

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
        Clear();
    }

    public void ChangeInfo(GameObject tileObject)
    {
        if (tileObject.TryGetComponent<Tile>(out var tile))
        {
            string resourceType = "";
            int resources = 0;
            foreach (var item in tile.producedRessources)
            {
                if (item.Value > 0)
                {
                    resourceType = item.Key.ToString();
                    resources = item.Value;
                    break;
                }
            }

            string ownerName = tile.owner != null ? tile.owner.PlayerType.ToString() : "Aucun";
            bool isOwner = tile.owner != null;

            SetTexts(tile.tileType.ToString(), resources, resourceType, isOwner, "", ownerName);
        }
    }

    private void SetTexts(
        string tileType, 
        int resources, 
        string resourceType, 
        bool isOwner = true, 
        string condition = "", 
        string owner = "Civilisation")
    {
        tileTypeText.text = tileType;
        ressourceText.text = "+ " + resources.ToString() + " " + resourceType;
        if (isOwner) conditionText.text = string.Empty;
        else conditionText.text = condition;
        ownerText.text = "Propri�taire : " + owner;
        tileInfoObject.SetActive(true);
    }

    public void Clear()
    {
        tileInfoObject.SetActive(false);
        tileTypeText.text = "No tile selected";
        ressourceText.text = string.Empty;
        conditionText.text = string.Empty;
        ownerText.text = string.Empty;
    }
}
