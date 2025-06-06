using System.Collections.Generic;
using UnityEngine;

public class VertexPoint : MonoBehaviour
{
    public enum StructureType
    {
        
    }

    public Vector3 Position => transform.position;

    public StructureType? structure = null;

    public object owner;
    public List<HexTile> nearbyTiles = new();
    public List<EdgePoint> edgePoints = new();

    private void Update()
    {
        OrientStructureToTileHeights();
    }

    public void Build(StructureType structureType)
    {
        owner = "debug"; //temp

        structure = structureType;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(child.name.ToUpper() == structureType.ToString());
        }

        Debug.Log($"Built: {structureType} at {name}");
    }

    public void OrientStructureToTileHeights()
    {
        if (nearbyTiles == null || nearbyTiles.Count == 0)
        {
            Debug.LogWarning($"[VertexPoint] No nearby tiles to orient {name}");
            return;
        }

        List<Vector3> positions = new();

        foreach (var tile in nearbyTiles)
        {
            if (tile != null)
                positions.Add(tile.transform.position);
        }

        if (positions.Count <= 1) return;

        Vector3 normal;

        if (positions.Count == 2)
        {
            Vector3 a = positions[0] - transform.position;
            Vector3 b = positions[1] - transform.position;
            normal = Vector3.Cross(a, b).normalized;
        }
        else
        {
            Vector3 a = positions[1] - positions[0];
            Vector3 b = positions[2] - positions[0];
            normal = Vector3.Cross(a, b).normalized;
        }

        if (normal.y < 0) normal = -normal;

        transform.up = normal;
    }
}
