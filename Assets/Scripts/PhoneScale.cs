using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PhoneScale : MonoBehaviour
{
    public RectTransform rect;

    public float scaleTime = 0.35f;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // 👉 Your target “open” size
    public Vector3 openScale = new Vector3(2.481574f, 4.70171f, 1f);

    private bool isVisible = false;
    private bool isAnimating = false;

    private void Start()
    {
        // Start hidden
        rect.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.spaceKey.wasPressedThisFrame &&
            !isAnimating)
        {
            if (!isVisible)
                StartCoroutine(ScaleUp());
            else
                StartCoroutine(ScaleDown());
        }
    }

    IEnumerator ScaleUp()
    {
        isAnimating = true;
        float t = 0f;

        while (t < scaleTime)
        {
            t += Time.deltaTime;

            float p = curve.Evaluate(t / scaleTime);
            rect.localScale = Vector3.Lerp(Vector3.zero, openScale, p);

            yield return null;
        }

        rect.localScale = openScale;
        isVisible = true;
        isAnimating = false;
    }

    IEnumerator ScaleDown()
    {
        isAnimating = true;
        float t = 0f;

        while (t < scaleTime)
        {
            t += Time.deltaTime;

            float p = curve.Evaluate(t / scaleTime);
            rect.localScale = Vector3.Lerp(openScale, Vector3.zero, p);

            yield return null;
        }

        rect.localScale = Vector3.zero;
        isVisible = false;
        isAnimating = false;
    }
}
