using UnityEngine;
using System.Collections;

public class TwistLungeSequence : MonoBehaviour
{
    // 🔔 Fired when the entire sequence is complete
    public static System.Action OnTwistLungeFinished;

    [Header("Local Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f;
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.6f;
    public float scaleDuration = 0.6f;
    public float returnDuration = 0.5f;

    [Header("Animation")]
    public Animator animator;
    public string triggerName = "PlayTwistedLunge";
    public string stateName = "TwistLunge";

    private bool started = false;

    void Awake()
    {
        // Force hidden start state
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void OnEnable()
    {
        SideStretchSequence.OnSideStretchFinished += StartSequence;
    }

    void OnDisable()
    {
        SideStretchSequence.OnSideStretchFinished -= StartSequence;
    }

    void StartSequence()
    {
        if (started) return;
        started = true;

        Debug.Log("TwistLunge STARTED");
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
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

        // Wait until animation state is entered
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait full animation length
        yield return new WaitForSeconds(
            animator.GetCurrentAnimatorStateInfo(0).length
        );

        // Move + scale back
        yield return MoveAndScale(
            endLocalPosition,
            startLocalPosition,
            endScale,
            startScale,
            returnDuration,
            returnDuration
        );

        Debug.Log("TwistLunge FINISHED");
        OnTwistLungeFinished?.Invoke();
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

            float p = posDur <= 0 ? 1f : Mathf.Clamp01(t / posDur);
            float s = scaleDur <= 0 ? 1f : Mathf.Clamp01(t / scaleDur);

            transform.localPosition = Vector3.Lerp(posFrom, posTo, p);
            transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, s);

            yield return null;
        }

        transform.localPosition = posTo;
        transform.localScale = scaleTo;
    }
}
