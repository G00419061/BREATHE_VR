using UnityEngine;
using System.Collections;

public class CrescentLungeSequence : MonoBehaviour
{
    // 🔔 EVENT: Fired when CrescentLunge is 100% finished
    public static System.Action OnCrescentLungeFinished;

    [Header("Local Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f; // never zero
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public float returnDuration = 0.4f;

    [Header("Animation")]
    public Animator animator; // Generic → must be on CrescentLunge root
    public string triggerName = "PlayCrescentLunge";
    public string animationStateName = "CrescentLunge";

    private bool hasStarted = false;

    void Awake()
    {
        // Force hidden start state
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void OnEnable()
    {
        // 🔒 Wait for ArmStretch to finish
        ArmStretchSequence.OnArmStretchFinished += StartSequence;
    }

    void OnDisable()
    {
        ArmStretchSequence.OnArmStretchFinished -= StartSequence;
    }

    void StartSequence()
    {
        if (hasStarted) return;
        hasStarted = true;

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Move + scale in
        yield return MoveAndScale(
            startLocalPosition,
            endLocalPosition,
            startScale,
            endScale,
            moveDuration,
            scaleDuration
        );

        // Play animation
        animator.SetTrigger(triggerName);

        // Wait for animation to complete
        yield return WaitForAnimation();

        // Move + scale back
        yield return MoveAndScale(
            endLocalPosition,
            startLocalPosition,
            endScale,
            startScale,
            returnDuration,
            returnDuration
        );

        // 🔔 BROADCAST FINISH
        OnCrescentLungeFinished?.Invoke();
    }

    IEnumerator MoveAndScale(
        Vector3 posFrom,
        Vector3 posTo,
        Vector3 scaleFrom,
        Vector3 scaleTo,
        float posDur,
        float scaleDur
    )
    {
        float duration = Mathf.Max(posDur, scaleDur);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(posFrom, posTo, Mathf.Clamp01(t / posDur));
            transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, Mathf.Clamp01(t / scaleDur));

            yield return null;
        }

        transform.localPosition = posTo;
        transform.localScale = scaleTo;
    }

    IEnumerator WaitForAnimation()
    {
        // Wait until Animator enters CrescentLunge
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
            yield return null;

        // Wait until animation completes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
