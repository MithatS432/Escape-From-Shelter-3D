using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider boxCollider;

    [Header("Hareket Ayarları")]
    public float speed = 6f;
    public float jumpForce = 7f;

    [Header("Kamera ve Dönüş Ayarları")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float lookLimit = 85f;

    private bool isGrounded;
    private float verticalRotation = 0f;
    private Vector3 inputDirection;

    [Header("UI Text Settings")]
    public TextMeshProUGUI firstText, secondText, thirdText, fourthText, fifthText;
    public GameObject firstTextObject, secondTextObject, thirdTextObject, fourthTextObject, fifthTextObject;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        boxCollider = GetComponent<BoxCollider>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogError("PlayerController: cameraTransform bulunamadı!");
        }
    }

    void Update()
    {
        HandleRotation();
        HandleInput();
        HandleJump();
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX * Time.deltaTime);

        verticalRotation -= mouseY * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -lookLimit, lookLimit);

        if (cameraTransform != null)
            cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
    }

    private void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(x, 0f, z).normalized;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }

    private void HandleMovement()
    {
        Vector3 move = transform.TransformDirection(inputDirection) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void CheckGround()
    {
        float rayDistance = 5f;
        Vector3 rayOrigin = transform.position + Vector3.up * 0.05f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
        {
            isGrounded = true;

            float targetY = hit.point.y + boxCollider.size.y / 2f;
            if (rb.position.y < targetY)
            {
                Vector3 pos = rb.position;
                pos.y = targetY;
                rb.position = pos;
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.2f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TextTrigger1"))
        {
            firstText.gameObject.SetActive(true);
            firstTextObject.SetActive(false);
            Invoke(nameof(CloseTexts), 3f);
        }
    }
    void CloseTexts()
    {
        firstText.gameObject.SetActive(false);
    }
}
