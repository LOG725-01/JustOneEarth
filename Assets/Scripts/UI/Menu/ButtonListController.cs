using UnityEngine;

public class ButtonListController : MonoBehaviour
{
    [SerializeField] private ButtonController[] buttonControllers = null;
    [SerializeField] private GameObject[] panels = null;

    private void OnValidate()
    {
        if (buttonControllers != null)
        {
            if (buttonControllers.Length != panels.Length)
                print("error each button should have a panel");
        }
        else if (panels != null) {
            print("error each panels should have a button");
        }
    }

    private void Awake()
    {
        for (int i = 0; i < buttonControllers.Length; i++)
        {
            buttonControllers[i].SetListController(this, i);
        }
    }

    public void AllNormal()
    {
        foreach (var buttonController in buttonControllers)
        {
            buttonController.NormalAnimation(true);
        }
    }

    public void PressButton(int button)
    {
        for (int i = 0; i < buttonControllers.Length; i++)
        {
            if (i == button) { 
                buttonControllers[i].PressAnimation();
                SecondaryButtonsActivation.SetActiveObject(panels[i]);
            }
            else { 
                buttonControllers[i].NormalAnimation(true);
                panels[i].SetActive(false);
            }
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < buttonControllers.Length; i++)
        {
            panels[i].SetActive(false);
        }
    }
}
