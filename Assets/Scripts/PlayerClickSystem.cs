using UnityEngine;

// Objelerin etkileşim için implement edeceği arayüz
public interface IClickable
{
    void OnClick();
}

public class PlayerClickSystem : MonoBehaviour
{
    [Header("Camera & Settings")]
    public Camera playerCamera;
    public float interactRange = 5f;
    public LayerMask interactMask;

    [Header("Click Cursor")]
    public GameObject cursorPrefab;
    public float cursorLifetime = 1f;

    private GameObject currentCursor;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange, interactMask))
            {
                if (currentCursor != null)
                    Destroy(currentCursor);

                currentCursor = Instantiate(cursorPrefab, hit.point + hit.normal * 0.01f, Quaternion.identity);
                currentCursor.transform.forward = hit.normal;

                Destroy(currentCursor, cursorLifetime);

                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}
