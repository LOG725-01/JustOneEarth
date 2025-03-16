using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ResourcesPanel : AnimationController
{
    [SerializeField] private TextMeshProUGUI buttonText;
    private ResourceAnimationController[] resources = null;

    private bool isOpened;
    private void Awake()
    {
        resources = FindObjectsOfType<ResourceAnimationController>();
        Open();
    }

    public void OpenClose()
    {
        if (isOpened) Close();
        else Open();
    }

    private void Open()
    {
        isOpened = true;
        buttonText.text = "Close";
        ChangeAnimation("Opened");
        foreach (var resource in resources)
        {
            resource.Open();
        }
    }

    private void Close()
    {
        isOpened = false;
        buttonText.text = "Open";
        ChangeAnimation("Closed");
        foreach (var resource in resources)
        {
            resource.Close();
        }
    }


}
