using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInputDetection : MonoBehaviour
{
    public static PlayerInputDetection Instance;
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private LayerMask raycastLayerMask;

    private GameState gameState;
    public GameState GameState { set => gameState = value; }

    private Camera mainCamera;

    [SerializeField] private GraphicRaycaster[] raycasters;
    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        
        if (gameState == null)
        {
            Debug.LogError("[PlayerInputDetection] gameState is null");
            return;
        }

        // check if UI is on top first
        PointerEventData m_PointerEventData = new(eventSystem)
        {
            position = Mouse.current.position.ReadValue()
        };
        List<RaycastResult> results = new();

        foreach(GraphicRaycaster raycaster in raycasters)
        {
            raycaster.Raycast(m_PointerEventData, results);
            if (results.Count > 0) return;
        }
        

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, raycastLayerMask);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.TryGetComponent<IClickable>(out var clickedObject))
                clickedObject.OnClick(gameState);
        }
        else
            DeselectCurrentTile(); 
    }

    private void DeselectCurrentTile()
    {
        gameState.currentInstancePlayer.DeselectTile();
        TileInfo.Instance.Clear();
    }
}
