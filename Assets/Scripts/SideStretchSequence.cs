using UnityEngine;
using System;
using System.Collections;

public class SideStretchSequence : MonoBehaviour
{
    public static event Action OnSideStretchFinished;

    [Header("Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f;
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.4f;
    public float scaleDuration = 0.4f;
    public float returnDuration = 0.4f;

    [Header("Animation")]
    public Animator animator;
    public string triggerName = "PlaySideStretch";
    public string animationStateName = "SideStretch";

    private bool started = false;

    void Awake()
    {
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void OnEnable()
    {
        if (started) return;
        started = true;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return MoveAndScale(startLocalPosition, endLocalPosition, startScale, endScale, moveDuration, scaleDuration);

        animator.SetTrigger(triggerName);

        yield return WaitForAnimation();

        yield return MoveAndScale(endLocalPosition, startLocalPosition, endScale, startScale, returnDuration, returnDuration);

        Debug.Log("SideStretch FINISHED");
        OnSideStretchFinished?.Invoke();
    }

    IEnumerator MoveAndScale(Vector3 pFrom, Vector3 pTo, Vector3 sFrom, Vector3 sTo, float pDur, float sDur)
    {
        float dur = Mathf.Max(pDur, sDur);
        float t = 0f;

        while (t < dur)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(pFrom, pTo, Mathf.Clamp01(t / pDur));
            transform.localScale = Vector3.Lerp(sFrom, sTo, Mathf.Clamp01(t / sDur));
            yield return null;
        }

        transform.localPosition = pTo;
        transform.localScale = sTo;
    }

    IEnumerator WaitForAnimation()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
            yield return null;

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
