using UnityEngine;

public class CameraController : MonoBehaviour
{    
    [SerializeField] private Camera cam;
    public float zoomSpeed = 5f; // Vitesse de zoom
    public float minZoom = 2f; // Zoom minimum
    public float maxZoom = 15f; // Zoom maximum

    private Vector3 dragOrigin; // Position d'origine du drag


    private void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;
        }

    }



    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); 
        if (scroll != 0)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }
}
