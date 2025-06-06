using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BoundingVolume3D : MonoBehaviour
{
    public enum BoundShape { Box, Sphere, ConvexHull }

    [Header("Target to constrain")]
    public Transform target;

    [Header("Shape Settings")]
    public BoundShape shape = BoundShape.Box;

    [Tooltip("For Box")]
    public Vector3 boxSize = new Vector3(10, 10, 10);

    [Tooltip("For Sphere")]
    public float radius = 5f;

    [Tooltip("For Convex Hull - define planes in local space")]
    public List<PlaneDefinition> convexPlanes = new();

    [Header("Behavior")]
    public bool constrainInPlayMode = true;
    public bool drawGizmos = true;

    [System.Serializable]
    public class PlaneDefinition
    {
        public Vector3 normal = Vector3.forward;
        public float distance = 1f;

        public Plane ToWorldPlane(Transform t)
        {
            Vector3 worldNormal = t.TransformDirection(normal).normalized;
            Vector3 point = t.position + t.TransformDirection(normal.normalized) * distance;
            return new Plane(worldNormal, point);
        }
    }

    void Update()
    {
        if (!Application.isPlaying || constrainInPlayMode)
        {
            if (target != null)
                ConstrainTarget();
        }
    }

    void ConstrainTarget()
    {
        Vector3 localPos = transform.InverseTransformPoint(target.position);

        switch (shape)
        {
            case BoundShape.Box:
                localPos = ClampInBox(localPos);
                break;

            case BoundShape.Sphere:
                localPos = ClampInSphere(localPos);
                break;

            case BoundShape.ConvexHull:
                Vector3 worldPos = ClampInConvex(transform.InverseTransformPoint(target.position));
                localPos = worldPos;
                break;
        }

        target.position = transform.TransformPoint(localPos);
    }

    Vector3 ClampInBox(Vector3 local)
    {
        Vector3 half = boxSize * 0.5f;
        return new Vector3(
            Mathf.Clamp(local.x, -half.x, half.x),
            Mathf.Clamp(local.y, -half.y, half.y),
            Mathf.Clamp(local.z, -half.z, half.z)
        );
    }

    Vector3 ClampInSphere(Vector3 local)
    {
        float dist = local.magnitude;
        if (dist > radius)
            local = local.normalized * radius;
        return local;
    }

    Vector3 ClampInConvex(Vector3 local)
    {
        Vector3 world = transform.TransformPoint(local);
        foreach (var pd in convexPlanes)
        {
            Plane p = pd.ToWorldPlane(transform);
            if (!p.GetSide(world))
            {
                float distance = p.GetDistanceToPoint(world);
                world -= p.normal * distance;
            }
        }
        return transform.InverseTransformPoint(world);
    }

    void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.green;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        switch (shape)
        {
            case BoundShape.Box:
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
                break;

            case BoundShape.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, radius);
                break;

            case BoundShape.ConvexHull:
                Gizmos.matrix = Matrix4x4.identity;
                foreach (var pd in convexPlanes)
                {
                    Plane p = pd.ToWorldPlane(transform);
                    Vector3 center = p.normal * p.distance;
                    Vector3 tangent = Vector3.Cross(p.normal, Vector3.up).normalized;
                    Vector3 bitangent = Vector3.Cross(p.normal, tangent).normalized;

                    float scale = 1f;
                    Vector3 a = center + tangent * scale;
                    Vector3 b = center - tangent * scale;
                    Vector3 c = center + bitangent * scale;
                    Vector3 d = center - bitangent * scale;

                    Gizmos.DrawLine(a, b);
                    Gizmos.DrawLine(c, d);
                    Gizmos.DrawLine(a, c);
                    Gizmos.DrawLine(b, d);
                }
                break;
        }

        Gizmos.matrix = oldMatrix;
    }
}
