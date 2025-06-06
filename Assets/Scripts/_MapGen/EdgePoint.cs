using System.Collections.Generic;
using UnityEngine;

public class EdgePoint : MonoBehaviour
{
    public VertexPoint pointA;
    public VertexPoint pointB;
    public Material roadMaterial;

    public object owner;
    public GameObject roadVisual;

    [Header("Visual Feedback")]
    public GameObject highlightVisual;

    public List<EdgePoint> connectedEdges = new();

    // Static flag set from UI (when road is being placed)
    public static bool ShowPlacementHighlights = false;

    void Start()
    {
        SetHighlightVisible(false);
    }

    void Update()
    {
        if (highlightVisual == null) return;

        // Update highlight only if road is not yet built and we're placing a road
        if (owner == null && ShowPlacementHighlights && IsConnectedToPlayer("debug")) // Replace with real player ID
        {
            SetHighlightVisible(true);
        }
        else
        {
            SetHighlightVisible(false);
        }
    }

    public void SetHighlightVisible(bool isVisible)
    {
        if (highlightVisual != null)
            highlightVisual.SetActive(isVisible);
    }

    public bool IsPlaceable()
    {
        return owner == null && (pointA.owner != null || pointB.owner != null);
    }

    public bool IsConnectedToPlayer(string player)
    {
        if ((pointA.owner != null && pointA.owner.Equals(player)) ||
            (pointB.owner != null && pointB.owner.Equals(player)))
        {
            return true;
        }

        foreach (EdgePoint neighbor in pointA.edgePoints)
        {
            if (neighbor != this && neighbor.owner != null && neighbor.owner.Equals(player))
                return true;
        }

        foreach (EdgePoint neighbor in pointB.edgePoints)
        {
            if (neighbor != this && neighbor.owner != null && neighbor.owner.Equals(player))
                return true;
        }

        return false;
    }

    public void Build(string player)
    {
        if (owner != null) return;

        owner = player;
        SetHighlightVisible(false); // Hide visual once built
        RoadGenerator.AlignRoad(this.gameObject, pointA.transform.position, pointB.transform.position, roadMaterial);
    }
}
