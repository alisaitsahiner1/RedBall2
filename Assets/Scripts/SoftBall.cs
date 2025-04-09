using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(Rigidbody2D))]
public class SoftBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public int pointCount = 12; // Kaç dış nokta olacak
    public float radius = 1f; // Topun yarıçapı
    public float vertexDensity = 1f; // Ne kadar detaylı mesh
    public float pointMass = 0.1f;

    [Header("Physics Settings")]
    public float springFrequency = 5f;
    public float springDamping = 0.6f;
    public PhysicsMaterial2D material2D;

    private List<GameObject> referencePoints = new List<GameObject>();
    private Vector3[] baseVertices;
    private Vector2[] uv;
    private int[] triangles;

    void Start()
    {
        GenerateReferencePoints();
        GenerateMesh();
    }

    void Update()
    {
        UpdateMeshDeformation();
    }

    void GenerateReferencePoints()
    {
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pointCount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            GameObject point = new GameObject("Point_" + i);
            point.transform.parent = transform;
            point.transform.localPosition = pos;

            Rigidbody2D rb = point.AddComponent<Rigidbody2D>();
            rb.mass = pointMass;
            rb.gravityScale = 1f;

            CircleCollider2D col = point.AddComponent<CircleCollider2D>();
            col.radius = 0.1f;
            col.sharedMaterial = material2D;

            SpringJoint2D spring = point.AddComponent<SpringJoint2D>();
            spring.connectedBody = GetComponent<Rigidbody2D>();
            spring.autoConfigureDistance = false;
            spring.distance = 0f;
            spring.dampingRatio = springDamping;
            spring.frequency = springFrequency;

            referencePoints.Add(point);
        }

        // Dış noktalar arası bağlantı da ekleyebilirsin istersen
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int vertCount = referencePoints.Count + 1;
        baseVertices = new Vector3[vertCount];
        uv = new Vector2[vertCount];
        triangles = new int[(referencePoints.Count) * 3];

        // Merkez vertex
        baseVertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        for (int i = 0; i < referencePoints.Count; i++)
        {
            float angle = i * Mathf.PI * 2f / pointCount;
            baseVertices[i + 1] = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            uv[i + 1] = new Vector2((Mathf.Cos(angle) + 1) / 2f, (Mathf.Sin(angle) + 1) / 2f);

            // Triangle indices
            int tri = i * 3;
            triangles[tri] = 0;
            triangles[tri + 1] = i + 1;
            triangles[tri + 2] = (i + 2 > referencePoints.Count) ? 1 : i + 2;
        }

        mesh.vertices = baseVertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    void UpdateMeshDeformation()
    {
        Vector3[] deformedVertices = new Vector3[baseVertices.Length];
        deformedVertices[0] = Vector3.zero; // merkez sabit

        for (int i = 0; i < referencePoints.Count; i++)
        {
            Vector3 localPos = transform.InverseTransformPoint(referencePoints[i].transform.position);
            deformedVertices[i + 1] = localPos;
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = deformedVertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
