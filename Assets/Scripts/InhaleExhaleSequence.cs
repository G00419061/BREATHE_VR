using UnityEngine;
using System;
using System.Collections;

public class InhaleExhaleSequence : MonoBehaviour
{
    public static event Action OnInhaleExhaleFinished;
    public static bool HasFinished = false;

    [Header("Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f;
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public float returnDuration = 0.4f;

    [Header("Animation")]
    public Animator animator;
    public string animationStateName = "InhaleExhale";

    void Awake()
    {
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
        animator.enabled = false;
        HasFinished = false;
    }

    void OnEnable()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return MoveAndScale(startLocalPosition, endLocalPosition, startScale, endScale, moveDuration, scaleDuration);

        animator.enabled = true;
        yield return null;
        animator.Play(animationStateName, 0, 0f);

        yield return WaitForAnimation();

        yield return MoveAndScale(endLocalPosition, startLocalPosition, endScale, startScale, returnDuration, returnDuration);

        animator.enabled = false;

        HasFinished = true;
        OnInhaleExhaleFinished?.Invoke();
    }

    IEnumerator MoveAndScale(Vector3 pFrom, Vector3 pTo, Vector3 sFrom, Vector3 sTo, float pDur, float sDur)
    {
        float t = 0f;
        float d = Mathf.Max(pDur, sDur);

        while (t < d)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(pFrom, pTo, t / pDur);
            transform.localScale = Vector3.Lerp(sFrom, sTo, t / sDur);
            yield return null;
        }

        transform.localPosition = pTo;
        transform.localScale = sTo;
    }

    IEnumerator WaitForAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
