using UnityEngine;

public class WayPointFollow : MonoBehaviour
{
    public GameObject[] waypoints;
    public float speed = 10.0f;
    public float rotSpeed = 10.0f;
    public float lookAhead = 10.0f;

    private int currentWP = 0;
    private GameObject tracker;

    private Quaternion originalRotation;          // store original rotation
    private bool returningToOriginalRotation = false; // flag for final rotation

    void Start()
    {
        // Save the original facing direction of RobotRoot
        originalRotation = transform.rotation;

        // Create tracker object
        tracker = new GameObject("Tracker");
        tracker.transform.position = transform.position;
    }

    void ProgressTracker()
    {
        if (currentWP >= waypoints.Length)
            return;

        Vector3 targetPos = new Vector3(
            waypoints[currentWP].transform.position.x,
            transform.position.y,  // lock to robot’s height
            waypoints[currentWP].transform.position.z
        );

        float distToWP = Vector3.Distance(transform.position, targetPos);

        // Move tracker toward next waypoint
        tracker.transform.LookAt(targetPos);
        tracker.transform.position += tracker.transform.forward * lookAhead * Time.deltaTime;

        // Advance to next waypoint
        if (distToWP < 1f)
        {
            currentWP++;

            // If we have reached the last waypoint
            if (currentWP >= waypoints.Length)
            {
                returningToOriginalRotation = true; // begin rotation back to start
            }
        }
    }

    void Update()
    {
        // If we reached last waypoint → rotate back to original facing direction
        if (returningToOriginalRotation)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                originalRotation,
                rotSpeed * Time.deltaTime
            );

            return; // stop all movement
        }

        if (currentWP >= waypoints.Length)
            return;

        ProgressTracker();

        if (currentWP >= waypoints.Length)
            return;

        // Smooth rotate toward tracker
        Quaternion look = Quaternion.LookRotation(tracker.transform.position - transform.position);
        float angle = Quaternion.Angle(transform.rotation, look);

        float dynamicRotSpeed = Mathf.Lerp(rotSpeed * 0.5f, rotSpeed, angle / 45f);

        transform.rotation = Quaternion.Slerp(transform.rotation, look, dynamicRotSpeed * Time.deltaTime);

        // Movement forward
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
