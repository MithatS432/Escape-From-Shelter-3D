using UnityEngine;
using System.Collections;

public class YellowCube : MonoBehaviour, IClickable
{
    private float stepHeight = 10f;
    private Vector3 startScale;
    private Vector3 startPos;
    private bool isScaling = false;
    private bool isRaised = false;

    void Start()
    {
        startScale = transform.localScale;
        startPos = transform.position;
    }

    public void OnClick()
    {
        if (isScaling || isRaised) return;

        float newHeight = startScale.y + stepHeight;
        StartCoroutine(ScaleSmooth(newHeight));
        isRaised = true;
    }

    public void OnRightClick()
    {
        if (isScaling || !isRaised) return;

        float newHeight = startScale.y;
        StartCoroutine(ScaleSmooth(newHeight));
        isRaised = false;
    }

    private IEnumerator ScaleSmooth(float targetHeight)
    {
        isScaling = true;

        Vector3 initialScale = transform.localScale;
        Vector3 targetScale = new Vector3(startScale.x, targetHeight, startScale.z);

        Vector3 initialPos = transform.position;
        float heightDifference = targetHeight - startScale.y;
        Vector3 targetPos = new Vector3(
            startPos.x,
            startPos.y + (heightDifference / 2f),
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
