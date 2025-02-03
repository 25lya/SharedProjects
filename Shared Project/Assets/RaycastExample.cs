using UnityEngine;
using UnityEngine.UI;

public class RaycastExample : MonoBehaviour
{
    public float rayDistance = 10f;
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    private int collectedObjects = 0;
    public Text objectName;
    public LayerMask viewableLayer;

    private Rigidbody rb;
    private Transform playerBody;

    private GameObject lastHitObject;
    private bool canChangeColor = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform;
    }

    void Update()
    {
        RotatePlayer();

        PerformRaycast();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = playerBody.right * moveX + playerBody.forward * moveZ;
        move.y = 0;

        rb.velocity = move.normalized * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    void RotatePlayer()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;


        playerBody.Rotate(Vector3.up * mouseX);


        verticalRotation -= mouseY;
        horizontalRotation -= mouseX;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, -horizontalRotation, 0f); // Apply the vertical rotation to the camera
    }

    void PerformRaycast()
    {
        Vector3 rayOrigin = Camera.main.transform.position;
        Vector3 rayDirection = Camera.main.transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, viewableLayer))
        {
            objectName.text = hit.collider.name;

            if (hit.collider.CompareTag("ChangeColor"))
            {
                if (lastHitObject != hit.collider.gameObject && canChangeColor)
                {
                    ChangeColor(hit.collider.gameObject);
                    lastHitObject = hit.collider.gameObject;
                    canChangeColor = false;
                }
            }
            else if (hit.collider.CompareTag("Moveable"))
            {
                MoveObject(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("Destroyable"))
            {
                objectName.text = "Press E to collect " + hit.collider.name;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DestroyObject(hit.collider.gameObject);
                }
            }
        }
        else
        {
            objectName.text = " ";
            canChangeColor = true;
            lastHitObject = null;
        }
    }

    void ChangeColor(GameObject obj)
    {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        if (objRenderer != null)
        {
            objRenderer.material.color = new Color(Random.value, Random.value, Random.value); // Change color
        }
    }

    void MoveObject(GameObject obj)
    {
        obj.transform.position += Camera.main.transform.forward * 0.1f;
    }

    void DestroyObject(GameObject obj)
    {
        Destroy(obj);
    }

    void PickUpObject(GameObject obj)
    {
        Destroy(obj);
        collectedObjects++;
    }
}