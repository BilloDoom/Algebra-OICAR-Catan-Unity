
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Port : MonoBehaviour
{
    public enum PortType
    {
        None,
        Trade,
        Fishing,
        Military
    }

    public PortType portType;
    public VertexPoint connectedA;
    public VertexPoint connectedB;

    /// <summary>
    /// Sets up the port to face away from the shared hex between vertex A and B.
    /// </summary>
    public void Initialize(PortType type, VertexPoint a, VertexPoint b)
    {
        portType = type;
        connectedA = a;
        connectedB = b;

        // Optional: face toward the midpoint of the two points (i.e., back toward island)
        Vector3 lookDirection = ((a.Position + b.Position) / 2f - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }
}
