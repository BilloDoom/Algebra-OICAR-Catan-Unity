using UnityEngine;

public class TrackTo : MonoBehaviour
{
    [Header("Tracking Settings")]
    public Transform target;
    public bool trackMainCamera = false;
    public bool smooth = true;
    public float rotationSpeed = 5f;
    public Vector3 axisConstraint = new Vector3(1, 1, 1);

    void Start()
    {
        if (trackMainCamera)
        {
            target = Camera.main?.transform;
        }
        if (target == null)
        {
            Debug.LogWarning("No target assigned for TrackTo script. Please assign a target or enable tracking of the main camera.");
        }
    }

    void Update()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;

        // Apply axis constraints
        direction = Vector3.Scale(direction, axisConstraint);
        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        if (smooth)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
        else
        {
            transform.rotation = targetRotation;
        }
    }
}
