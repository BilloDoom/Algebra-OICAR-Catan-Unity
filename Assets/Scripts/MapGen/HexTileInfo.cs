using UnityEngine;
using static BoardGenerator;

public class HexTileInfo : MonoBehaviour
{
    public ResourceType resource;
    public int numberToken;

    void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.2f, $"{resource} {numberToken}");
    }
}
