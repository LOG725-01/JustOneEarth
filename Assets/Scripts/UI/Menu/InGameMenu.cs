using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (inGameMenu.activeSelf)
                AudioManager.Instance.UiClose();
            else 
                AudioManager.Instance.UiOpen();
            inGameMenu.SetActive(!inGameMenu.activeSelf);
        }
            
    }
}
