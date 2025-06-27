using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public float smoothTime = 0.2f; // Camera movement smoothness
    public float zoomLimiter = 10f; // Lower = more zoom-out on distance
    public float minZoom = 5f;      // Closest zoom level
    public float maxZoom = 15f;     // Farthest zoom level
    public Vector3 offset = new Vector3(0, 0, -10f); // Keeps camera behind

    private Vector3 velocity;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player1 == null || player2 == null)
            return;

        Move();
        Zoom();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float distance = GetGreatestDistance();

        // Inverse the distance with limiter to control zoom effect
        float newZoom = Mathf.Lerp(maxZoom, minZoom, distance / zoomLimiter);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
    }

    float GetGreatestDistance()
    {
        return Vector3.Distance(player1.position, player2.position);
    }

    Vector3 GetCenterPoint()
    {
        return (player1.position + player2.position) / 2f;
    }
}
