using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private bool copyRotation;
    [SerializeField] Vector3 offset = Vector3.zero;

    void Start()
    {
        if (target == null)
            Debug.LogWarning($"No target for {this.name} set!");
    }

    void Update()
    {
        transform.position = target.transform.position + offset;
        if (copyRotation)
            transform.rotation = target.transform.rotation;
    }
}
