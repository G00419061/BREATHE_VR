using UnityEngine;
using System.Collections;

public class CrescentLungeSequence : MonoBehaviour
{
    [Header("Local Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f; // NEVER ZERO
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public float returnDuration = 0.4f;

    [Header("Animation")]
    public Animator animator; // MUST be Armature Simple Human
    public string triggerName = "PlayCrescentLunge";
    public string animationStateName = "CrescentLunge";

    bool hasStarted = false;

    void Awake()
    {
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;

        Debug.Log("CrescentLunge Awake()");
        Debug.Log($"Animator assigned? {animator != null}");
    }

    void OnEnable()
    {
        Debug.Log("CrescentLunge OnEnable() — waiting for ArmStretch");
        ArmStretchSequence.OnArmStretchFinished += StartSequence;
    }

    void OnDisable()
    {
        ArmStretchSequence.OnArmStretchFinished -= StartSequence;
    }

    void StartSequence()
    {
        Debug.Log("CrescentLunge StartSequence() CALLED");

        if (hasStarted)
        {
            Debug.Log("CrescentLunge already started — ignoring");
            return;
        }

        hasStarted = true;
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        Debug.Log("CrescentLunge PlaySequence STARTED");

        // Move + scale in
        yield return MoveAndScale(
            startLocalPosition,
            endLocalPosition,
            startScale,
            endScale,
            moveDuration,
            scaleDuration
        );

        Debug.Log("CrescentLunge MOVE+SCALE COMPLETE");

        // Trigger animation
        animator.SetTrigger(triggerName);
        Debug.Log($"Trigger SENT: {triggerName}");

        yield return null; // wait 1 frame

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        Debug.Log($"After trigger — InState({animationStateName}) = {state.IsName(animationStateName)}");
        Debug.Log($"State normalizedTime = {state.normalizedTime}");

        // Wait for animation
        yield return WaitForAnimation();

        Debug.Log("CrescentLunge ANIMATION FINISHED");

        // Move + scale back
        yield return MoveAndScale(
            endLocalPosition,
            startLocalPosition,
            endScale,
            startScale,
            returnDuration,
            returnDuration
        );

        Debug.Log("CrescentLunge RETURN COMPLETE");
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
        Debug.Log("Waiting for CrescentLunge state...");

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
            yield return null;

        Debug.Log("CrescentLunge STATE ENTERED");

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
