using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;

    public void ToggleMenu()
    {
        if (inGameMenu.activeSelf)
            AudioManager.Instance.UiClose();
        else
            AudioManager.Instance.UiOpen();

        inGameMenu.SetActive(!inGameMenu.activeSelf);
    }

    public bool IsMenuOpen()
    {
        return inGameMenu.activeSelf;
    }
}