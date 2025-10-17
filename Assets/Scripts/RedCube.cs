using UnityEngine;

public class RedCube : MonoBehaviour, IClickable
{
    private int clickCount = 0;
    private int maxClicks = 3;
    private float moveAmount = 2f;
    private Vector3 startPos;
    private bool isMoving = false;

    void Start()
    {
        startPos = transform.position;
    }

    public void OnClick()
    {
        // Sol tıklama ile çağrılır, artan hareket
        if (clickCount < maxClicks && !isMoving)
        {
            clickCount++;
            Vector3 targetPos = startPos + Vector3.up * moveAmount * clickCount;
            StartCoroutine(MoveSmooth(targetPos));
        }
    }

    public void OnRightClick()
    {
        if (clickCount > 0 && !isMoving)
        {
            clickCount--;
            Vector3 targetPos = startPos + Vector3.up * moveAmount * clickCount;
            StartCoroutine(MoveSmooth(targetPos));
        }
    }

    private System.Collections.IEnumerator MoveSmooth(Vector3 targetPos)
    {
        isMoving = true;
        float t = 0f;
        Vector3 initial = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * 3f;
            transform.position = Vector3.Lerp(initial, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }
}
