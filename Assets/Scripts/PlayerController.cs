using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 10f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;

    private bool isGrounded;
    private float verticalRotation = 0f;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null && GetComponentInChildren<Camera>() != null)
            cameraTransform = GetComponentInChildren<Camera>().transform;

        rb.freezeRotation = true;
    }

    void Update()
    {
        // Fare ile bakış
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);
        if (cameraTransform != null)
            cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);


        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        inputDirection = new Vector3(x, 0f, z).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector3 move = transform.TransformDirection(inputDirection) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
