using System.Collections.Generic;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject hexTilePrefab;
    public float hexRadius = 1f;

    // Resource types enum
    public enum ResourceType { Wood, Sheep, Wheat, Brick, Ore, Desert }

    [System.Serializable]
    public class HexTileData
    {
        public ResourceType resource;
        public int numberToken;
    }

    private List<Vector2Int> boardCoordinates = new()
    {
        new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0),
        new(0, 1), new(1, 1), new(2, 1), new(3, 1),
        new(0, 2), new(1, 2), new(2, 2),
        new(1, -1), new(2, -1), new(3, -1),
        new(2, -2), new(3, -2)
    };

    void Start()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        List<ResourceType> resources = new()
        {
            ResourceType.Wood, ResourceType.Wood, ResourceType.Wood, ResourceType.Wood,
            ResourceType.Sheep, ResourceType.Sheep, ResourceType.Sheep, ResourceType.Sheep,
            ResourceType.Wheat, ResourceType.Wheat, ResourceType.Wheat, ResourceType.Wheat,
            ResourceType.Brick, ResourceType.Brick, ResourceType.Brick,
            ResourceType.Ore, ResourceType.Ore, ResourceType.Ore,
            ResourceType.Desert
        };

        List<int> tokens = new() { 2, 3, 3, 4, 4, 5, 5, 6, 6, 8, 8, 9, 9, 10, 10, 11, 11, 12 };

        Shuffle(resources);
        Shuffle(tokens);

        for (int i = 0; i < boardCoordinates.Count; i++)
        {
            Vector2Int axial = boardCoordinates[i];
            Vector3 worldPos = HexToWorld(axial);

            GameObject tile = Instantiate(hexTilePrefab, worldPos, Quaternion.identity, transform);
            ResourceType resource = resources[i];

            int token = -1;
            if (resource != ResourceType.Desert && tokens.Count > 0)
            {
                token = tokens[0];
                tokens.RemoveAt(0);
            }

            // Optional: visualize or assign to tile
            var info = tile.AddComponent<HexTileInfo>();
            info.resource = resource;
            info.numberToken = token;

            // You could also update text/UI elements here
        }
    }

    Vector3 HexToWorld(Vector2Int axial)
    {
        float x = hexRadius * 1.5f * axial.x;
        float z = hexRadius * Mathf.Sqrt(3f) * (axial.y + axial.x * 0.5f);
        return new Vector3(x, 0, z);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            (list[i], list[randIndex]) = (list[randIndex], list[i]);
        }
    }
}
