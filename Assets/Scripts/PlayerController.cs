using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    
    [Header("Hareket Ayarları")]
    public float speed = 6f;
    public float jumpForce = 5f;
    
    [Header("Kamera ve Dönüş Ayarları")]
    [Tooltip("Karakterin alt objesi olan kamera Transform'u")]
    public Transform cameraTransform; 
    public float mouseSensitivity = 100f; // Genellikle Time.deltaTime olmadan kullanılır
    public float lookLimit = 85f; // Yukarı/aşağı bakış sınırı (derece)

    private bool isGrounded;
    private float verticalRotation = 0f; // Sadece kameranın dikey açısı
    private Vector3 inputDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Fizik motorunun karakteri döndürmesini engeller
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Fareyi kilitler ve gizler
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Eğer Inspector'da atanmamışsa, alt objelerdeki ilk Kamerayı bul
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogError("Hata: PlayerController, bir cameraTransform nesnesi bulamıyor!");
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
        // Fare Hareketi
        // Time.deltaTime olmadan daha doğal bir hassasiyet yönetimi sağlarız
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // 1. Karakterin Yatay Dönüşü (Y Ekseninde Dönüş)
        // Karakterin kendisini (kök objesini) yatay olarak döndürürüz.
        transform.Rotate(Vector3.up * mouseX * Time.deltaTime);

        verticalRotation -= mouseY * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -lookLimit, lookLimit);
        
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = new Vector3(verticalRotation, 0f, 0f);
        }
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        HandleMovement();
        CheckGround();
    }

    private void HandleMovement()
    {
        Vector3 localMove = transform.TransformDirection(inputDirection) * speed;
        
        rb.linearVelocity = new Vector3(localMove.x, rb.linearVelocity.y, localMove.z);
    }

    private void CheckGround()
    {
        float rayDistance = 0.6f; 
        isGrounded = Physics.Raycast(transform.position, Vector3.down, rayDistance);

    }
    
    private void OnDrawGizmos()
    {
        if (rb == null) return;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        
        float rayDistance = 0.6f;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * rayDistance);
    }
}