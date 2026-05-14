using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class PolygonFromVertices : MonoBehaviour
{
    [Header("Vertex Objects")]
    public Transform[] vertices; // Drag your vertex GameObjects here

    [Header("Options")]
    public bool closedLoop = true;
    public bool fillPolygon = true;
    public Color outlineColor = Color.white;
    public Color fillColor = Color.cyan;
    public float lineWidth = 0.05f;

    private LineRenderer _lr;
    private MeshFilter _mf;
    private MeshRenderer _mr;

    void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _mf = GetComponent<MeshFilter>();
        _mr = GetComponent<MeshRenderer>();

        // Setup line renderer
        _lr.useWorldSpace = true;
        _lr.loop = closedLoop;
        _lr.startWidth = lineWidth;
        _lr.endWidth = lineWidth;
        _lr.startColor = outlineColor;
        _lr.endColor = outlineColor;

        // Setup fill material
        _mr.material = new Material(Shader.Find("Sprites/Default"));
        _mr.material.color = fillColor;
    }

    void Update()
    {
        if (vertices == null || vertices.Length < 2) return;

        DrawOutline();
        if (fillPolygon) DrawFill();
    }

    void DrawOutline()
    {
        _lr.positionCount = vertices.Length;
        for (int i = 0; i < vertices.Length; i++)
            _lr.SetPosition(i, vertices[i].position);
    }

    void DrawFill()
    {
        // Build a 2D polygon mesh using fan triangulation
        Mesh mesh = new Mesh();

        Vector3[] verts = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            // Convert to local space for the mesh
            verts[i] = transform.InverseTransformPoint(vertices[i].position);

        // Fan triangulation (works for convex polygons)
        // For concave polygons, use a proper triangulator (see below)
        int[] triangles = new int[(vertices.Length - 2) * 3];
        for (int i = 0; i < vertices.Length - 2; i++)
        {
            triangles[i * 3]     = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        _mf.mesh = mesh;
    }
}
