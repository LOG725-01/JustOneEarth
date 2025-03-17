using System;
using UnityEngine;

public class PlayerInputNotifier : MonoBehaviour
{
    // Define an event using Action
    public event Action<GameObject> OnGameObjectClicked;

    // Called when the GameObject is clicked.
    public void OnMouseDown()
    {
        // Notify all observers that this GameObject was clicked.
        OnGameObjectClicked?.Invoke(gameObject);
    }
}
