using UnityEngine;

public class SecondaryButtonsActivation : MonoBehaviour
{
    public static void SetActiveObject(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
