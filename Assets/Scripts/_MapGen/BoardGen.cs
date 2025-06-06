
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Port;

public class BoardGen : MonoBehaviour
{
    public static BoardGen Instance { get; private set; }

    public GameObject hexTilePrefab;
    public GameObject edgePointPrefab;

    public GameObject thiefPrefab;
    private GameObject thiefInstance;
    private HexTile currentThiefTile;

    public float hexSize = 1f;

    private const int radius = 2;

    public GameObject vertexPrefab;
    private Dictionary<Vector3, VertexPoint> vertexMap = new();

    public bool generateBoardOnStart = false;

    public GameObject portPrefab;
    public List<PortType> portTypes;

    /* = new()
    {
        PortType.Generic3To1, PortType.Wood2To1, PortType.Brick2To1,
        PortType.Generic3To1, PortType.Wheat2To1, PortType.Sheep2To1,
        PortType.Generic3To1, PortType.Ore2To1, PortType.Generic3To1
    }; */

    Vector3 GetVertexPosition(Vector3 center, int cornerIndex, float radius)
    {
        float angle = Mathf.Deg2Rad * (60 * cornerIndex + 30);
        float x = center.x + radius * Mathf.Cos(angle);
        float z = center.z + radius * Mathf.Sin(angle);
        return new Vector3(x, 0.1f, z);
    }

    void SpawnVertexIfNotExists(Vector3 position)
    {
        position = new Vector3(Mathf.Round(position.x * 10) / 10f, 0.1f, Mathf.Round(position.z * 10) / 10f);

        if (vertexMap.ContainsKey(position)) return;

        GameObject v = Instantiate(vertexPrefab, position, Quaternion.identity);

        var vertexComp = v.GetComponent<VertexPoint>();
        vertexMap[position] = vertexComp;
    }

    private static readonly string[] resources = {
        "wood", "wood", "wood", "wood",
        "brick", "brick", "brick",
        "wheat", "wheat", "wheat", "wheat",
        "sheep", "sheep", "sheep", "sheep",
        "ore", "ore", "ore",
        "desert"
    };

    private static readonly int[] numberTokens = {
        5, 2, 6, 3, 8, 10, 9, 12, 11, 4,
        8, 10, 9, 4, 5, 6, 3, 11
    };
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (generateBoardOnStart)
        {
            GenerateBoard();
            List<VertexPoint> allPoints = FindObjectsByType<VertexPoint>(FindObjectsSortMode.None).ToList();
            GenerateEdgePoints(allPoints);
            PlacePorts();
        }
    }


    public void GenerateBoard()
    {
        List<string> shuffledResources = new(resources);
        List<int> shuffledNumbers = new(numberTokens);

        Shuffle(shuffledResources);
        Shuffle(shuffledNumbers);

        int index = 0;
        int numberIndex = 0;

        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);

            for (int r = r1; r <= r2; r++)
            {
                Vector3 pos = HexToWorld(q, r);

                for (int i = 0; i < 6; i++)
                {
                    Vector3 vertexPos = GetVertexPosition(pos, i, hexSize);
                    SpawnVertexIfNotExists(vertexPos);
                }

                GameObject tileObj = Instantiate(hexTilePrefab, pos, Quaternion.identity);

                if (tileObj.GetComponent<MeshCollider>() == null)
                {
                    MeshCollider meshCollider = tileObj.AddComponent<MeshCollider>();
                    MeshFilter mf = tileObj.GetComponent<MeshFilter>();
                    if (mf != null)
                        meshCollider.sharedMesh = mf.sharedMesh;
                }

                string resource = shuffledResources[index++];
                int number = resource == "desert" ? 0 : shuffledNumbers[numberIndex++];

                HexTile tile = tileObj.GetComponent<HexTile>();
                tile.Initialize(resource, number, q, r);

                if (resource == "desert")
                {
                    thiefInstance = Instantiate(thiefPrefab, tileObj.transform);
                    thiefInstance.transform.localPosition = Vector3.up * 0.1f;
                    currentThiefTile = tile;
                }
            }
        }

        foreach (var vertex in vertexMap.Values)
        {
            foreach (var tile in FindObjectsByType<HexTile>(FindObjectsSortMode.None))
            {
                if (Vector3.Distance(tile.transform.position, vertex.Position) < hexSize + .1f)
                {
                    vertex.nearbyTiles.Add(tile);
                }
            }
        }
    }

    public void GenerateEdgePoints(List<VertexPoint> allVertexPoints)
    {
        HashSet<(VertexPoint, VertexPoint)> createdEdges = new();

        for (int i = 0; i < allVertexPoints.Count; i++)
        {
            VertexPoint a = allVertexPoints[i];

            for (int j = i + 1; j < allVertexPoints.Count; j++)
            {
                VertexPoint b = allVertexPoints[j];

                float dist = Vector3.Distance(a.Position, b.Position);

                float minDist = hexSize * 0.8f;
                float maxDist = hexSize * 1.3f;

                if (dist >= minDist && dist <= maxDist)
                {
                    if (!createdEdges.Contains((a, b)) && !createdEdges.Contains((b, a)))
                    {
                        GameObject edgeGO = Instantiate(edgePointPrefab, (a.Position + b.Position) / 2, Quaternion.identity, transform);
                        EdgePoint ep = edgeGO.GetComponent<EdgePoint>();

                        ep.pointA = a;
                        ep.pointB = b;

                        a.edgePoints.Add(ep);
                        b.edgePoints.Add(ep);

                        createdEdges.Add((a, b));
                    }
                }
            }
        }
    }

    void PlacePorts()
    {
        Shuffle(portTypes);
        var portPairs = FindValidPortPairs();

        Vector3 center = CalculateBoardCenter();
        var sorted = portPairs.OrderBy(pair =>
        {
            Vector3 mid = (pair.Item1.Position + pair.Item2.Position) / 2f;
            Vector3 dir = (mid - center).normalized;
            return Mathf.Atan2(dir.z, dir.x);
        }).ToList();

        int count = Mathf.Min(portTypes.Count, sorted.Count);
        int spacing = sorted.Count / count;

        for (int i = 0; i < count; i++)
        {
            int index = i * spacing;
            var (a, b) = sorted[index];
            PortType type = portTypes[i];

            List<HexTile> sharedHexes = a.nearbyTiles.Intersect(b.nearbyTiles).ToList();
            if (sharedHexes.Count == 0)
                continue;
            
            Vector3 hexCenter = sharedHexes[0].transform.position;

            Vector3 midpoint = (a.Position + b.Position) / 2f;
            Vector3 outwardDirection = (midpoint - hexCenter).normalized;

            Vector3 portPosition = midpoint + outwardDirection * hexSize;

            GameObject portObj = Instantiate(portPrefab, portPosition, Quaternion.identity, transform);
            Port port = portObj.GetComponent<Port>();
            port.Initialize(type, a, b);
        }
    }

    List<(VertexPoint, VertexPoint)> FindValidPortPairs()
    {
        var portPairs = new List<(VertexPoint, VertexPoint)>();

        foreach (var vertex in vertexMap.Values)
        {
            if (vertex.nearbyTiles.Count > 2) continue;

            foreach (var edge in vertex.edgePoints)
            {
                VertexPoint other = edge.pointA == vertex ? edge.pointB : edge.pointA;

                if (other == null || other == vertex || other.nearbyTiles.Count > 2)
                    continue;

                var pair = (vertex, other);

                if (!portPairs.Any(p => (p.Item1 == pair.Item2 && p.Item2 == pair.Item1)))
                    portPairs.Add(pair);
            }
        }

        return portPairs;
    }

    Vector3 CalculateBoardCenter()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (var tile in FindObjectsByType<HexTile>(FindObjectsSortMode.None))
        {
            sum += tile.transform.position;
            count++;
        }

        return count > 0 ? sum / count : Vector3.zero;
    }

    Vector3 HexToWorld(int q, int r)
    {
        float x = Mathf.Sqrt(3f) * (q + r / 2f);
        float z = 1.5f * r;
        return new Vector3(x * hexSize, 0, z * hexSize);
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void MoveThiefTo(HexTile newTile)
    {
        if (thiefInstance == null || newTile == currentThiefTile)
            return;

        thiefInstance.transform.SetParent(newTile.transform);
        thiefInstance.transform.localPosition = Vector3.up * 0.1f;
        currentThiefTile = newTile;
    }

    public HexTile GetCurrentThiefTile()
    {
        return currentThiefTile;
    }
}
