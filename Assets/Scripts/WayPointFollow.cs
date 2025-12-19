using UnityEngine;

public class WayPointFollow : MonoBehaviour
{
    public GameObject[] waypoints;
    public float speed = 10.0f;
    public float rotSpeed = 10.0f;
    public float lookAhead = 10.0f;

    private int currentWP = 0;
    private GameObject tracker;

    private Quaternion originalRotation;
    private bool returningToOriginalRotation = false;

    // 🔒 LOCK MOVEMENT UNTIL CRESCENT LUNGE FINISHES
    private bool canMove = false;

    void Awake()
    {
        originalRotation = transform.rotation;
    }

    void OnEnable()
    {
        CrescentLungeSequence.OnCrescentLungeFinished += EnableMovement;
    }

    void OnDisable()
    {
        CrescentLungeSequence.OnCrescentLungeFinished -= EnableMovement;
    }

    void Start()
    {
        tracker = new GameObject("Tracker");
        tracker.transform.position = transform.position;
    }

    void EnableMovement()
    {
        Debug.Log("WayPointFollow ENABLED");
        canMove = true;
    }

    void ProgressTracker()
    {
        if (currentWP >= waypoints.Length)
            return;

        Vector3 targetPos = new Vector3(
            waypoints[currentWP].transform.position.x,
            transform.position.y,
            waypoints[currentWP].transform.position.z
        );

        float distToWP = Vector3.Distance(transform.position, targetPos);

        tracker.transform.LookAt(targetPos);
        tracker.transform.position += tracker.transform.forward * lookAhead * Time.deltaTime;

        if (distToWP < 1f)
        {
            currentWP++;

            if (currentWP >= waypoints.Length)
            {
                returningToOriginalRotation = true;
            }
        }
    }

    void Update()
    {
        // 🚫 DO NOTHING until CrescentLunge finishes
        if (!canMove)
            return;

        // Rotate back to original facing when done
        if (returningToOriginalRotation)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                originalRotation,
                rotSpeed * Time.deltaTime
            );
            return;
        }

        if (currentWP >= waypoints.Length)
            return;

        ProgressTracker();

        if (currentWP >= waypoints.Length)
            return;

        Quaternion look = Quaternion.LookRotation(tracker.transform.position - transform.position);
        float angle = Quaternion.Angle(transform.rotation, look);

        float dynamicRotSpeed = Mathf.Lerp(rotSpeed * 0.5f, rotSpeed, angle / 45f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            look,
            dynamicRotSpeed * Time.deltaTime
        );

        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
