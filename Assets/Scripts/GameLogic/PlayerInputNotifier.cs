using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputNotifier : MonoBehaviour
{
    // Define an event using Action
    public static event Action<GameObject> OnGameObjectClicked;

    // Called when the GameObject is clicked.
    private void OnMouseDown()
    {
        // Notify all observers that this GameObject was clicked.
        OnGameObjectClicked?.Invoke(gameObject);
    }
}
