using UnityEngine;

public class YellowCube : MonoBehaviour, IClickable
{
    private int clickCount = 0;
    private int maxClicks = 3;
    private float stepHeight = 1f;
    private Vector3 startScale;
    private Vector3 startPos;
    private bool isScaling = false;

    void Start()
    {
        startScale = transform.localScale;
        startPos = transform.position;
    }

    public void OnClick()
    {
        if (clickCount < maxClicks && !isScaling)
        {
            clickCount++;
            float newHeight = stepHeight * clickCount;
            StartCoroutine(ScaleSmooth(newHeight));
        }
    }

    public void OnRightClick()
    {
        if (clickCount > 0 && !isScaling)
        {
            clickCount--;
            float newHeight = stepHeight * clickCount;
            StartCoroutine(ScaleSmooth(newHeight));
        }
    }

    private System.Collections.IEnumerator ScaleSmooth(float newHeight)
    {
        isScaling = true;
        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x, newHeight, startScale.z);

        Vector3 initialPos = transform.position;
        Vector3 targetPos = new Vector3(
            startPos.x,
            startPos.y + (newHeight - startScale.y) / 2f,
            startPos.z
        );

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 3f; 
            transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            transform.position = Vector3.Lerp(initialPos, targetPos, t);
            yield return null;
        }

        transform.localScale = targetScale;
        transform.position = targetPos;
        isScaling = false;
    }
}
