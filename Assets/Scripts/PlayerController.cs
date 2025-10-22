using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider boxCollider;

    [Header("Hareket Ayarları")]
    public float speed = 6f;
    public float jumpForce = 7f;
    private float zRange = 700f;

    [Header("Kamera ve Dönüş Ayarları")]
    public Transform cameraTransform;
    public float mouseSensitivity = 100f;
    public float lookLimit = 85f;

    [Header("Zemin Kontrol Ayarları")]
    public LayerMask groundLayer = -1; // Tüm layer'lar
    public float groundCheckCooldownDuration = 0.3f;
    public float checkSphereRadius = 0.3f;

    private bool isGrounded;
    private float verticalRotation = 0f;
    private Vector3 inputDirection;
    private float groundCheckCooldown;
    private bool jumpInputUsed;

    [Header("UI Text Settings")]
    public TextMeshProUGUI firstText, secondText, thirdText, fourthText, fifthText;
    public GameObject firstTextObject, secondTextObject, thirdTextObject, fourthTextObject, fifthTextObject;
    public GameObject wall1, wall2, wall3;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        
        // Rigidbody ayarları
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = true;
        
        // Collider ayarı
        boxCollider.isTrigger = false;

        // Layer ayarı
        if (groundLayer.value == 0)
        {
            groundLayer = LayerMask.GetMask("Default");
        }

        // Fare ayarları
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Kamera kontrolü
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
            else
                Debug.LogError("PlayerController: cameraTransform bulunamadı!");
        }

        Debug.Log("PlayerController başlatıldı. Ground Layer: " + groundLayer.value);
    }

    void Update()
    {
        HandleRotation();
        HandleInput();
        HandleJump();

        if (transform.position.z >= zRange)
            SceneManager.LoadScene(0);
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded && !jumpInputUsed)
            {
                // Dikey hızı sıfırla ve zıpla
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                isGrounded = false;
                jumpInputUsed = true;
                groundCheckCooldown = groundCheckCooldownDuration;
                
                Debug.Log("Zıplama gerçekleşti!");
            }
        }
        
        // Space tuşu bırakıldığında resetle
        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpInputUsed = false;
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

        float halfHeight = boxCollider.size.y * 0.5f * transform.lossyScale.y;
        Vector3 checkPosition = transform.position + Vector3.down * (halfHeight + 0.15f);
        
        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(checkPosition, checkSphereRadius, groundLayer, QueryTriggerInteraction.Ignore);
        
        // Eğer yukarı doğru hareket ediyorsa, grounded olmamalı
        if (rb.linearVelocity.y > 0.1f)
        {
            isGrounded = false;
        }
        
        // Yere yeni temas ettiysek
        if (!wasGrounded && isGrounded)
        {
            Debug.Log("Yere temas edildi!");
            jumpInputUsed = false;
        }
    }

    void OnDrawGizmos()
    {
        if (boxCollider == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        float halfHeight = boxCollider.size.y * 0.5f * transform.lossyScale.y;
        Vector3 checkPosition = transform.position + Vector3.down * (halfHeight + 0.15f);
        Gizmos.DrawWireSphere(checkPosition, checkSphereRadius);
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