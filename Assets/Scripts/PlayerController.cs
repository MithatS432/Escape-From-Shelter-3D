using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    public float speed = 6f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 200f;
    public Transform cameraTransform;

    private bool isGrounded;
    private float verticalRotation = 0f;
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null && GetComponentInChildren<Camera>() != null)
            cameraTransform = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        // Fare hareketi
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
        }
    }

    void FixedUpdate()
    {
        Vector3 move = transform.TransformDirection(inputDirection) * speed;
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);

        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.1f);
    }
}
