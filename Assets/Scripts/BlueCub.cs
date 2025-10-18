using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlueCub : MonoBehaviour, IClickable
{
    private Vector3 originalScale;
    public float jumpForce = 5f;
    public float scaleDown = 0.5f;
    private bool isActive = false;

    void Start()
    {
        originalScale = transform.localScale;
        GetComponent<Collider>().isTrigger = true;
    }

    public void OnClick()
    {
        if (isActive) return;

        Vector3 scaled = originalScale;
        scaled.y = scaleDown;
        transform.localScale = scaled;
        isActive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        transform.localScale = originalScale;
        isActive = false;
    }
}
