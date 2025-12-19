using UnityEngine;

public class WayPointFollow : MonoBehaviour
{
    public GameObject[] waypoints;
    public float speed = 10f;
    public float rotSpeed = 10f;
    public float lookAhead = 10f;

    private int currentWP = 0;
    private GameObject tracker;

    private Quaternion originalRotation;
    private bool returningToOriginalRotation = false;
    private bool canMove = false;

    void Awake()
    {
        originalRotation = transform.rotation;
    }

    void OnEnable()
    {
        CrescentLungeSequence.OnCrescentLungeFinished += EnableMovement;
        TwistLungeSequence.OnTwistLungeFinished += EnableMovement;
        LungeSequence.OnLungeFinished += EnableMovement;
    }

    void OnDisable()
    {
        CrescentLungeSequence.OnCrescentLungeFinished -= EnableMovement;
        TwistLungeSequence.OnTwistLungeFinished -= EnableMovement;
        LungeSequence.OnLungeFinished -= EnableMovement;
    }

    void Start()
    {
        tracker = new GameObject("Tracker");
        tracker.transform.position = transform.position;
    }

    void EnableMovement()
    {
        if (canMove) return;
        Debug.Log("WayPointFollow ENABLED");
        canMove = true;
    }

    void Update()
    {
        if (!canMove)
            return;

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

        Quaternion look = Quaternion.LookRotation(
            tracker.transform.position - transform.position
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            look,
            rotSpeed * Time.deltaTime
        );

        transform.Translate(0, 0, speed * Time.deltaTime);
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

        tracker.transform.LookAt(targetPos);
        tracker.transform.position += tracker.transform.forward * lookAhead * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPos) < 1f)
        {
            currentWP++;
            if (currentWP >= waypoints.Length)
                returningToOriginalRotation = true;
        }
    }
}
