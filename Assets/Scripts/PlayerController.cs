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
    private float groundCheckCooldown;

    [Header("UI Text Settings")]
    public TextMeshProUGUI firstText, secondText, thirdText, fourthText, fifthText;
    public GameObject firstTextObject, secondTextObject, thirdTextObject, fourthTextObject, fifthTextObject;
    public GameObject wall1, wall2, wall3;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null)
            boxCollider.isTrigger = false;

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
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
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
            groundCheckCooldown = 0.1f;
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }

    private void HandleMovement()
    {
        Vector3 targetVelocity = transform.TransformDirection(inputDirection) * speed;
        Vector3 newVelocity = new Vector3(targetVelocity.x, rb.linearVelocity.y, targetVelocity.z);
        rb.linearVelocity = newVelocity;
    }

    private void CheckGround()
    {
        if (groundCheckCooldown > 0)
        {
            groundCheckCooldown -= Time.fixedDeltaTime;
            return;
        }

        float rayDistance = (boxCollider != null ? (boxCollider.size.y / 2f) : 0.9f) + 0.2f;
        Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, rayDistance))
            isGrounded = true;
        else
            isGrounded = false;
    }

    private void OnDrawGizmos()
    {
        if (boxCollider == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.down * 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TextTrigger1"))
        {
            firstText.gameObject.SetActive(true);
            firstTextObject.SetActive(false);
            Invoke(nameof(CloseText1), 3f);
        }
        else if (other.CompareTag("TextTrigger2"))
        {
            secondText.gameObject.SetActive(true);
            secondTextObject.SetActive(false);
            Invoke(nameof(CloseText2), 2f);
        }
        else if (other.CompareTag("TextTrigger3"))
        {
            thirdText.gameObject.SetActive(true);
            thirdTextObject.SetActive(false);
            Invoke(nameof(CloseText3), 2f);
        }
        else if (other.CompareTag("TextTrigger4"))
        {
            fourthText.gameObject.SetActive(true);
            fourthTextObject.SetActive(false);
            Invoke(nameof(CloseText4), 2f);
        }
        else if (other.CompareTag("TextTrigger5"))
        {
            fifthText.gameObject.SetActive(true);
            fifthTextObject.SetActive(false);
            Invoke(nameof(CloseText5), 2f);
        }
        else if (other.CompareTag("WallTrigger"))
        {
            wall1.SetActive(true);
            wall2.SetActive(true);
            wall3.SetActive(true);
        }
    }

    void CloseText1() => firstText.gameObject.SetActive(false);
    void CloseText2() => secondText.gameObject.SetActive(false);
    void CloseText3() => thirdText.gameObject.SetActive(false);
    void CloseText4() => fourthText.gameObject.SetActive(false);
    void CloseText5() => fifthText.gameObject.SetActive(false);
}
