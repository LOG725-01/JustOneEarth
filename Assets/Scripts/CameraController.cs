using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public float moveSpeed = 5f; // Vitesse de deplacement avec WASD
    public float panSpeed = 5f; // Vitesse du drag avec la souris
    public float zoomSpeed = 5f; // Vitesse du zoom
    public float minZoom = 2f; // Zoom minimum
    public float maxZoom = 7f; // Zoom maximum

    private Vector3 dragOrigin;

    private void Start()
    {
        cam.transform.position = new Vector3(-6.5f, 7, -6f);
        cam.transform.rotation = Quaternion.Euler(45, 45, 0);
    }

    private void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 difference = cam.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
            Vector3 move = new Vector3(difference.x * panSpeed, 0, difference.y * panSpeed);

            cam.transform.position += move;
            dragOrigin = Input.mousePosition;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0; // Empeche la caméra de se deplacer en hauteur
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 movement = (right * moveX + forward * moveZ).normalized * moveSpeed * Time.deltaTime;
        cam.transform.position += movement;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            Vector3 zoomDirection = cam.transform.forward;
            Vector3 targetPosition;

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                targetPosition = hit.point;
            }
            else
            {
                targetPosition = cam.transform.position + zoomDirection * 10f;
            }

            float newZoom = Mathf.Clamp(cam.transform.position.y - scroll * zoomSpeed, minZoom, maxZoom);


            if ((scroll > 0 && cam.transform.position.y <= minZoom) ||
                (scroll < 0 && cam.transform.position.y >= maxZoom))
            {
                return; 
            }

            // Applique le zoom
            Vector3 direction = (targetPosition - cam.transform.position).normalized;
            cam.transform.position += direction * scroll * zoomSpeed;
            cam.transform.position = new Vector3(cam.transform.position.x, newZoom, cam.transform.position.z);
        }
    }

}
