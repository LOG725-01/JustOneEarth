using TMPro;
using UnityEngine;

public class ResourceObserver : Observer
{
    [SerializeField] private TextMeshProUGUI resourcesNumber;
    public override void ObserverUpdate(GameObject subject)
    {
        throw new System.NotImplementedException();
        resourcesNumber.text = "Quantity";
    }

}
