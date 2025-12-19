using UnityEngine;
using System.Collections;

public class LungeSequence : MonoBehaviour
{
    // 🔔 Fired when Lunge fully finishes
    public static System.Action OnLungeFinished;

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
    public string triggerName = "PlayLunge";
    public string stateName = "Lunge";

    private bool started = false;

    void Awake()
    {
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void OnEnable()
    {
        InhaleExhaleSequence.OnInhaleExhaleFinished += StartSequence;
    }

    void OnDisable()
    {
        InhaleExhaleSequence.OnInhaleExhaleFinished -= StartSequence;
    }

    void StartSequence()
    {
        if (started) return;
        started = true;

        Debug.Log("Lunge STARTED");
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

        // Wait until animation state entered
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait full animation
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

        Debug.Log("Lunge FINISHED");
        OnLungeFinished?.Invoke();
    }

    // ✅ ONLY ONE MoveAndScale EXISTS
    IEnumerator MoveAndScale(
        Vector3 pFrom,
        Vector3 pTo,
        Vector3 sFrom,
        Vector3 sTo,
        float pDur,
        float sDur
    )
    {
        float duration = Mathf.Max(pDur, sDur);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float pt = Mathf.Clamp01(t / pDur);
            float st = Mathf.Clamp01(t / sDur);

            transform.localPosition = Vector3.Lerp(pFrom, pTo, pt);
            transform.localScale = Vector3.Lerp(sFrom, sTo, st);

            yield return null;
        }

        transform.localPosition = pTo;
        transform.localScale = sTo;
    }
}
