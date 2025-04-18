using UnityEngine;
using System.Collections;

public class Blob : MonoBehaviour
{
    private class PropagateCollisions : MonoBehaviour   //çarpışmaları yayma
    {
        void OnCollisionEnter2D(Collision2D collision) //referans noktalarındaki collisionlar parent objesine iletiliyor
        {
            transform.parent.SendMessage("OnCollisionEnter2D", collision, SendMessageOptions.DontRequireReceiver);
        }
    }

    public int width = 5;
    public int height = 5;
    public int referencePointsCount = 12;
    public float referencePointRadius = 0.25f;
    public float mappingDetail = 10;
    public float springDampingRatio = 0;
    public float springFrequency = 2;
    public PhysicsMaterial2D surfaceMaterial;

    GameObject[] referencePoints;
    int vertexCount;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uv;
    Vector3[,] offsets; // 2 indeksli dizi olduğundan arada virgül var
    float[,] weights;

    void Start()
    {
        CreateReferencePoints();
        CreateMesh();
        MapVerticesToReferencePoints();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.sortingLayerName = "Default"; // veya "TopLayer"
        mr.sortingOrder = 10; // diğer objelerden yüksek olsun
    }

    void CreateReferencePoints()
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        referencePoints = new GameObject[referencePointsCount]; //referans noktalarını saklayacak dizi
        Vector3 offsetFromCenter = ((0.5f - referencePointRadius) * Vector3.up);
        float angle = 360.0f / referencePointsCount;

        for (int i = 0; i < referencePointsCount; i++)
        {
            referencePoints[i] = new GameObject();
            referencePoints[i].tag = gameObject.tag;
            referencePoints[i].AddComponent<PropagateCollisions>();
            referencePoints[i].transform.parent = transform; //burayı anlamadım
            Quaternion rotation = Quaternion.AngleAxis(angle * (i - 1), Vector3.back); //her nokta için dönüş açısı hesaplanır
            referencePoints[i].transform.localPosition = rotation * offsetFromCenter; //dönüş açısıyla birlikte noktayı hesaplanan noktaya yerleştir

            Rigidbody2D body = referencePoints[i].AddComponent<Rigidbody2D>();
            //body.fixedAngle = true; //o noktanın rotasyonunu kilitler
            body.interpolation = rigidbody.interpolation; //2 satırda ana toptan alınan özellikler referans noktalarına aktarılır
            body.collisionDetectionMode = rigidbody.collisionDetectionMode;

            CircleCollider2D collider = referencePoints[i].AddComponent<CircleCollider2D>();
            collider.radius = referencePointRadius * transform.localScale.x; //colliderın boyutu objenin boyutuna göre belirlenir
            if (surfaceMaterial != null)
            {
                collider.sharedMaterial = surfaceMaterial; //collidera fizik matereryali uygulanır
            }

            AttachWithSpringJoint(referencePoints[i], gameObject); //referans noktası spring joint ile ana objeye bağlanır
            if (i > 0)
            {
                AttachWithSpringJoint(referencePoints[i], referencePoints[i - 1]); // nokta bir önceki nokta ile de bağlanır
            }
        }
        AttachWithSpringJoint(referencePoints[0], referencePoints[referencePointsCount - 1]); //son nokta ilk nokta ile de bağlanır

        IgnoreCollisionsBetweenReferencePoints(); //noktaların çarpışmaları engellenir
    }

    void AttachWithSpringJoint(GameObject referencePoint, GameObject connected) // 2 nesne arasında fiziksel yay kurulur
    {
        SpringJoint2D springJoint = referencePoint.AddComponent<SpringJoint2D>();
        springJoint.connectedBody = connected.GetComponent<Rigidbody2D>();
        springJoint.connectedAnchor = LocalPosition(referencePoint) - LocalPosition(connected); //LocalPosition noktaların top nesnesine göre yerini gösterir
        springJoint.distance = 0;
        springJoint.dampingRatio = springDampingRatio;
        springJoint.frequency = springFrequency;
    }

    void IgnoreCollisionsBetweenReferencePoints()
    {
        int i;
        int j;
        CircleCollider2D a;
        CircleCollider2D b;

        for (i = 0; i < referencePointsCount; i++)
        {
            for (j = i; j < referencePointsCount; j++) //baştaki eşitenme ile aynı ikili iki kere kontrol edilmez
            {
                a = referencePoints[i].GetComponent<CircleCollider2D>();
                b = referencePoints[j].GetComponent<CircleCollider2D>();
                Physics2D.IgnoreCollision(a, b, true);
            }
        }
    }

    void CreateMesh() //topun görsel temsili oluşturulur
    {
        vertexCount = (width + 1) * (height + 1); //vertex sayısı hesaplanır

        int trianglesCount = width * height * 6; //üçgen sayısı hesaplanır
        vertices = new Vector3[vertexCount]; //nokta konumları
        triangles = new int[trianglesCount]; //mesh üçgen yapısı
        uv = new Vector2[vertexCount]; //kaplama koordinatları
        int t;

        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                int v = (width + 1) * y + x; // her nokta için index hesaplanır ama tam anlamadım?
                vertices[v] = new Vector3 (x / (float)width - 0.5f, y / (float)height - 0.5f, 0); //vertexlerin pozisyonu belirleniyor
                uv[v] = new Vector2(x / (float)width, y / (float)height); //uv koordinatları hesaplanır

                if (x < width && y < height)  //köşe noktalarındaysak üçgen çizmesin
                {
                    t = 3 * (2 * width * y + 2 * x);

                    triangles[t] = v;  //kareyi oluşturan iki üçgenin vertex indexleri atanır ama tam mantığını anlamadım
                    triangles[++t] = v + width + 1;
                    triangles[++t] = v + width + 2;
                    triangles[++t] = v;
                    triangles[++t] = v + width + 2;
                    triangles[++t] = v + 1;
                }
            }
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    void MapVerticesToReferencePoints() //Mesh’in her bir vertex’ini çevresindeki referans noktalara göre bağlar
    {
        offsets = new Vector3[vertexCount, referencePointsCount]; //Vertex i, referans noktası j'ye göre ne kadar uzakta?
        weights = new float[vertexCount, referencePointsCount]; //Referans noktası j, vertex i'yi ne kadar etkiliyor?

        for (int i = 0; i < vertexCount; i++)
        {
            float totalWeight = 0;

            for (int j = 0; j < referencePointsCount; j++)
            {
                offsets[i, j] = vertices[i] - LocalPosition(referencePoints[j]); //Bu vertex, referans noktasına göre nerede?
                weights[i, j] = 1 / Mathf.Pow(offsets[i, j].magnitude, mappingDetail); //Uzaklık arttıkça etki azalsın istiyoruz , mappingDetail ne kadar büyükse, etki daha lokal olur
                totalWeight += weights[i, j];
            }

            for (int j = 0; j < referencePointsCount; j++)
            {
                weights[i, j] /= totalWeight;
            }
        }
    }

    void Update()
    {
        UpdateVertexPositions();
    }

    void UpdateVertexPositions()
    {
        Vector3[] vertices = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = Vector3.zero; //İlk olarak sıfırlanır, sonra katkılar eklenir

            for (int j = 0; j < referencePointsCount; j++)
            {
                vertices[i] += weights[i, j] * (LocalPosition(referencePoints[j]) + offsets[i, j]);
            }
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    Vector3 LocalPosition(GameObject obj) //dünya konumunu alıp top objesinin lokal konumuna dönüştürür
    {
        return transform.InverseTransformPoint(obj.transform.position);
    }
}
