using UnityEngine;
using System.Collections;

public class InhaleExhaleSequence : MonoBehaviour
{
    // 🔔 Fired when the whole sequence is finished
    public static System.Action OnInhaleExhaleFinished;

    [Header("Local Position")]
    public Vector3 startLocalPosition;
    public Vector3 endLocalPosition;

    [Header("Scale")]
    public Vector3 startScale = Vector3.one * 0.01f;
    public Vector3 endScale = Vector3.one;

    [Header("Timing")]
    public float moveDuration = 0.5f;
    public float scaleDuration = 0.5f;
    public float returnDuration = 0.5f;

    [Header("Animation")]
    public Animator animator;
    public string triggerName = "PlayInhaleExhale";
    public string stateName = "InhaleExhale"; // must match Animator state name exactly

    private bool started = false;

    void Awake()
    {
        // Force hidden start state
        transform.localPosition = startLocalPosition;
        transform.localScale = startScale;
    }

    void Start()
    {
        // ▶ Play automatically on scene start
        if (started) return;
        started = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Move + scale in
        yield return MoveAndScale(startLocalPosition, endLocalPosition, startScale, endScale, moveDuration, scaleDuration);

        // Trigger animation
        animator.SetTrigger(triggerName);

        // Wait until animation state is entered
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            yield return null;

        // Wait full animation length
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Move + scale back
        yield return MoveAndScale(endLocalPosition, startLocalPosition, endScale, startScale, returnDuration, returnDuration);

        // 🔔 Broadcast finished
        Debug.Log("InhaleExhale FINISHED");
        OnInhaleExhaleFinished?.Invoke();
    }

    IEnumerator MoveAndScale(Vector3 pFrom, Vector3 pTo, Vector3 sFrom, Vector3 sTo, float pDur, float sDur)
    {
        float duration = Mathf.Max(pDur, sDur);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float pt = pDur <= 0f ? 1f : Mathf.Clamp01(t / pDur);
            float st = sDur <= 0f ? 1f : Mathf.Clamp01(t / sDur);

            transform.localPosition = Vector3.Lerp(pFrom, pTo, pt);
            transform.localScale = Vector3.Lerp(sFrom, sTo, st);

            yield return null;
        }

        transform.localPosition = pTo;
        transform.localScale = sTo;
    }
}
