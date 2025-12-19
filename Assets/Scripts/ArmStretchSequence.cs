using UnityEngine;
using System.Collections;

public class ArmStretchSequence : MonoBehaviour
{
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
    public string triggerName = "PlayArmStretch";
    public string animationStateName = "ArmStretch";

    void Awake()
    {
        // Force start state
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void OnEnable()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // Move + scale to end
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

        // Return to start position + scale
        yield return MoveAndScale(
            endLocalPosition,
            startLocalPosition,
            endScale,
            startScale,
            returnDuration,
            returnDuration
        );
    }

    IEnumerator MoveAndScale(
        Vector3 posFrom,
        Vector3 posTo,
        Vector3 scaleFrom,
        Vector3 scaleTo,
        float posDuration,
        float scaleDuration
    )
    {
        float duration = Mathf.Max(posDuration, scaleDuration);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float posT = Mathf.Clamp01(t / posDuration);
            float scaleT = Mathf.Clamp01(t / scaleDuration);

            transform.localPosition = Vector3.Lerp(posFrom, posTo, posT);
            transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, scaleT);

            yield return null;
        }

        transform.localPosition = posTo;
        transform.localScale = scaleTo;
    }

    IEnumerator WaitForAnimation()
    {
        // Wait for correct state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
            yield return null;

        // Wait until animation finishes
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
    }
}
