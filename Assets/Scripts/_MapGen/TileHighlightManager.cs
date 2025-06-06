using UnityEngine;
using System.Collections.Generic;

public class TileHighlightManager : MonoBehaviour
{
    public float maxLift = 0.5f;
    public float maxDistance = 3f;
    public float smoothSpeed = 5f;

    private Plane groundPlane;
    private List<FancyHighlightTile> tiles = new List<FancyHighlightTile>();

    void Awake()
    {
        groundPlane = new Plane(Vector3.up, Vector3.zero); // Plane at y=0
    }

    void Update()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        foreach (var tile in tiles)
        {
            float distance = Vector3.Distance(mouseWorldPos, tile.WorldPosition);
            float t = Mathf.Clamp01(1f - (distance / maxDistance));
            float targetLift = maxLift * t;
            tile.SetLiftTarget(targetLift, smoothSpeed);
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;

        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }

        return Vector3.zero; // Fallback
    }

    public void RegisterTile(FancyHighlightTile tile)
    {
        if (!tiles.Contains(tile))
            tiles.Add(tile);
    }
}
