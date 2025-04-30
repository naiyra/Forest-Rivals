using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the offset relative to the target's rotation
            Vector3 rotatedOffset = target.rotation * offset;

            transform.position = target.position + rotatedOffset;

            // Make the camera look at the car
            transform.LookAt(target);
        }
    }
}
