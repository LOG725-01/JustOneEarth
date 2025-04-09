using TMPro;
using UnityEngine;

public class ResourceObserver : Observer
{
    [SerializeField] private TextMeshProUGUI resourcesNumber;
    [SerializeField] private RessourceTypes resourceType; 


    public override void ObserverUpdate(GameObject subject)
    {
        if (subject.TryGetComponent<Player>(out var player))
        {
            int amount = player.currentRessources[resourceType];
            resourcesNumber.text = amount.ToString();
            //Debug.Log($"[ResourceObserver] {resourceType} mis � jour : {amount}");
        }
    }
}