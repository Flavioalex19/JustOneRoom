using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;                    

    [Header("Follow Settings")]
    public bool followTarget = true;            
    public bool useSmoothing = true;            

    [Header("Smooth Settings")]
    public float smoothSpeed = 5f;              
    public float smoothDamping = 0.25f;         

    private Vector3 velocity = Vector3.zero;    

    void LateUpdate()
    {
        if (!followTarget || target == null) return;

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        if (useSmoothing)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothDamping
            );
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    public void SetFollow(bool enabled)
    {
        followTarget = enabled;
    }
    public void SetSmoothing(bool enabled)
    {
        useSmoothing = enabled;
    }
}
