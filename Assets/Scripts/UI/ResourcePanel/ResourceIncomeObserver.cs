using TMPro;
using UnityEngine;

public class ResourceIncomeObserver : Observer
{
    [SerializeField] private TextMeshProUGUI resourceIncome;

    [SerializeField] private string suffix = "par tour";
    public override void ObserverUpdate(GameObject subject)
    {
        throw new System.NotImplementedException();
        resourceIncome.text = "Quantity" + " " + suffix;
    }

}
