using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMeshGenerator : MonoBehaviour
{
    [Header("Hex Settings")]
    public float radius = 1f;
    [Range(0f, 360f)] public float rotationY = 0f;
    public float height = 0.2f;

    [Header("Options")]
    public bool addCollider = true;

    void Awake()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Procedural Hex";

        Vector3[] vertices = new Vector3[14];  // 7 top (0-6) + 7 bottom (7-13)
        int[] triangles = new int[6 * 3 + 6 * 6]; // 6 top + 6 sides (2 triangles each)

        Vector2[] uvs = new Vector2[14];

        float angleOffsetRad = Mathf.Deg2Rad * rotationY;

        // Top center
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        // Generate top ring and bottom ring
        for (int i = 0; i < 6; i++)
        {
            float angle = angleOffsetRad + Mathf.Deg2Rad * (60 * i);
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            // Top ring vertex (1 to 6)
            vertices[i + 1] = new Vector3(x, 0f, z);
            uvs[i + 1] = new Vector2((x / (radius * 2f)) + 0.5f, (z / (radius * 2f)) + 0.5f);

            // Bottom ring vertex (7 to 12)
            vertices[i + 7] = new Vector3(x, -height, z);
            uvs[i + 7] = uvs[i + 1]; // reuse UVs
        }

        // Bottom center (not used, bottom cap is open)
        vertices[13] = new Vector3(0, -height, 0);
        uvs[13] = new Vector2(0.5f, 0.5f);

        int t = 0;

        // Top cap triangles (fan)
        for (int i = 0; i < 6; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = (i == 5) ? 1 : i + 2;
            triangles[t++] = i + 1;
        }

        // Side quads (as two triangles each)
        for (int i = 0; i < 6; i++)
        {
            int topCurrent = i + 1;
            int topNext = (i == 5) ? 1 : i + 2;
            int bottomCurrent = topCurrent + 6;
            int bottomNext = topNext + 6;

            // First triangle
            triangles[t++] = topCurrent;
            triangles[t++] = bottomNext;
            triangles[t++] = bottomCurrent;

            // Second triangle
            triangles[t++] = topCurrent;
            triangles[t++] = topNext;
            triangles[t++] = bottomNext;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        if (addCollider)
        {
            var collider = GetComponent<MeshCollider>();
            if (!collider)
                collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
    }
}
