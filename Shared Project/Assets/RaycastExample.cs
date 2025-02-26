using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RaycastExample : MonoBehaviour
{
    public float rayDistance = 10f;
    public float moveSpeed = 5f;
    public float lookSensitivity = 2f;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;
    public static GameObject[] bridges;
    private int collectedObjects = 0;
    public Text objectName;
    public LayerMask viewableLayer;

    public Vector3 jump;
    public float jumpForce = 2.0f;
    public bool isGrounded;

    public Light flashlight;
    public float batteryLevel = 100f;
    public Slider batteryLevelSlide;

    private Rigidbody rb;
    private Transform playerBody;

    private GameObject lastHitObject;
    private bool canChangeColor = true;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        playerBody = transform;

        StartCoroutine(FlashlightSim());

        jump = new Vector3(0.0f, 2.0f, 0.0f);
    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    void Update()
    {
        batteryLevelSlide.value = batteryLevel / 100;
        flashlight.range = rayDistance;
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.enabled = !flashlight.enabled;
        }

        RotatePlayer();

        PerformRaycast();

       
    }

    void FixedUpdate()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {

            rb.AddForce(jump * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
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
            else if (hit.collider.CompareTag("RedBox") && Input.GetKey(KeyCode.E))
            {
                MoveObject(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("BlueBox") && Input.GetKey(KeyCode.E))
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

    public static void MoveBridge1()
    {
        bridges[0].transform.position = new Vector3(-4, 1, 24);
    }

    public static void MoveBridge2()
    {
       // bridges[1].transform.position = new Vector3();
    }

    IEnumerator FlashlightSim()
    {
        while(true)
        {
            if (flashlight.enabled)
            {
                yield return new WaitForSeconds(10f);

                if(flashlight.enabled)
                {
                    batteryLevel -= 5;
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator FlickerManager()
    {
        while (true)
        {
            if (flashlight.enabled && batteryLevel < 50)
            {
                yield return new WaitForSeconds(Random.Range(2f, 6f)); //flickers between 2 and 5
                StartCoroutine(FlickerEffect());
            }
            else
            {
                yield return null;
            }
        }    
    }

    IEnumerator FlickerEffect()
    {
        int flickerCount = Random.Range(1, 4);
        for (int i = 0; i < flickerCount; i++)
        {
            flashlight.enabled = false;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            flashlight.enabled = true;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        }
    }
}