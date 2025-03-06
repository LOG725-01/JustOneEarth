using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    [SerializeField] private GameObject inGameMenu;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            inGameMenu.SetActive(!inGameMenu.activeSelf);
    }
}
