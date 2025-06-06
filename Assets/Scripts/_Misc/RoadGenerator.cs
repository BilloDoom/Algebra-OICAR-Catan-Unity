using UnityEngine;

public static class RoadGenerator
{
    public static void AlignRoad(GameObject roadObject, Vector3 pointA, Vector3 pointB, Material material)
    {
        Vector3 direction = pointB - pointA;
        float length = direction.magnitude;
        Vector3 midPoint = (pointA + pointB) / 2f;

        MeshFilter mf = roadObject.GetComponent<MeshFilter>();
        if (mf == null) mf = roadObject.AddComponent<MeshFilter>();

        MeshRenderer mr = roadObject.GetComponent<MeshRenderer>();
        if (mr == null) mr = roadObject.AddComponent<MeshRenderer>();

        MeshCollider mc = roadObject.GetComponent<MeshCollider>();
        if (mc == null) mc = roadObject.AddComponent<MeshCollider>();

        Mesh mesh = new Mesh();

        float width = .5f; // Adjustable width
        float height = .75f; // Thickness of the road

        // Create a box mesh centered at origin, length = Z axis
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-width, -height, -length / 2), // Bottom Left Back
            new Vector3(width, -height, -length / 2),  // Bottom Right Back
            new Vector3(width, -height, length / 2),   // Bottom Right Front
            new Vector3(-width, -height, length / 2),  // Bottom Left Front

            new Vector3(-width, height, -length / 2), // Top Left Back
            new Vector3(width, height, -length / 2),  // Top Right Back
            new Vector3(width, height, length / 2),   // Top Right Front
            new Vector3(-width, height, length / 2),  // Top Left Front
        };

        int[] triangles = new int[]
        {
            // Bottom
            0, 1, 2, 0, 2, 3,
            // Top
            4, 6, 5, 4, 7, 6,
            // Front
            3, 2, 6, 3, 6, 7,
            // Back
            0, 5, 1, 0, 4, 5,
            // Left
            0, 3, 7, 0, 7, 4,
            // Right
            1, 5, 6, 1, 6, 2
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;

        roadObject.transform.position = midPoint;
        roadObject.transform.rotation = Quaternion.LookRotation(direction);

        if (material != null)
        {
            mr.material = material;
        }
        else
        {
            Debug.LogWarning("No road material assigned. Using default.");
            mr.material = new Material(Shader.Find("Standard"));
        }
    }
}
