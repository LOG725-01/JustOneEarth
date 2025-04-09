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
            Vector3 currentMousePos = Input.mousePosition;
            Vector3 delta = dragOrigin - currentMousePos;

            Vector3 right = cam.transform.right;
            Vector3 forward = cam.transform.forward;

            right.y = 0f;
            forward.y = 0f;

            right.Normalize();
            forward.Normalize();


            Vector3 move = (right * delta.x + forward * delta.y) * panSpeed * Time.deltaTime;

            cam.transform.position += move;
            dragOrigin = currentMousePos;
        }

        if (Chat.Instance != null && Chat.Instance.IsChatInputSelected())
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 camRight = cam.transform.right;
        Vector3 camForward = cam.transform.forward;

        camRight.y = 0f;
        camForward.y = 0f;

        camRight.Normalize();
        camForward.Normalize();

        Vector3 movement = (camRight * moveX + camForward * moveZ).normalized * moveSpeed * Time.deltaTime;
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
