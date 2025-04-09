using TMPro;
using UnityEngine;

public class ResourceIncomeObserver : Observer
{
    [SerializeField] private TextMeshProUGUI resourceIncome;
    [SerializeField] private string suffix = " every round";
    [SerializeField] private RessourceTypes resourceType;

    public override void ObserverUpdate(GameObject subject)
    {
        if (subject.TryGetComponent<Player>(out var player))
        {
            int gain = 0;

            foreach (var tile in player.ownedTiles)
            {
                if (tile.producedRessources.ContainsKey(resourceType))
                    gain += tile.producedRessources[resourceType];
            }

            resourceIncome.text = "+" + gain.ToString() + " " + suffix;
        }
    }
}
