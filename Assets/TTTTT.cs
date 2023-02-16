using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TTTTT : MonoBehaviour
{
    public Material material;
    public int planeNum;
    int  lastPlaneNum;
    public float height;
    public float width;
    public bool  IsmoreThanHalfCir;
    public float _Radius;
    public Vector3 dir;
    Mesh newMesh;
    Vector3[] vertexs;
    int[] triangles;
    Vector3[] newVertexs;
    public float f;
    // Start is called before the first frame update
    void Start()
    {
        newMesh = new Mesh();
        newMesh.name = "test";
        //  vertexs = GetComponent<MeshFilter>().mesh.vertices;
        // triangles = GetComponent<MeshFilter>().mesh.triangles;
        // newVertexs = new Vector3[vertexs.Length];
        GetComponent<MeshRenderer>().material = material;
    }
    public  void  SetVertex(Vector3 leftPoint,Vector3 rightPoint)
    {

    }
    Vector3 point = Vector3.zero;
    Vector3 cylinderCenter;
    // Update is called once per frame

    List<Vector3> VertexList = new List<Vector3>();
    List<int> TriangleList = new List<int>();
    List<Vector2> uvList = new List<Vector2>();
    List<Color> clorList = new List<Color>();
    void GenratePlane()
    {
        if (planeNum != lastPlaneNum)
        {
            Vector3 startPoint = transform.InverseTransformPoint(transform.position);
            Vector3 rightDir = transform.InverseTransformDirection( transform.right.normalized);
            Vector3 upDir = transform.InverseTransformDirection(transform.up.normalized);
            VertexList.Clear();
            TriangleList.Clear();
            uvList.Clear();
            clorList.Clear();
            for (int i = 0; i < planeNum; i++)
            {
                Vector3 L = startPoint - rightDir * width + upDir * height * i;
                Vector3 R = startPoint + rightDir * width + upDir * height * i;
                VertexList.Add(L);
                VertexList.Add(R);
                uvList.Add(Vector2.one);
                uvList.Add(Vector2.one);
                clorList.Add(Color.white);
                clorList.Add(Color.white);
            }


            if (VertexList.Count > 2)
            {
                int i = 0;
                while (i + 2 < VertexList.Count)
                {
                    //äÖÈ¾ÕýÃæ
                    TriangleList.Add(i);
                    TriangleList.Add(i + 2);
                    TriangleList.Add(i + 1);
                    TriangleList.Add(i + 1);
                    TriangleList.Add(i + 2);
                    TriangleList.Add(i + 3);
                    //äÖÈ¾±³Ãæ
                    TriangleList.Add(i);
                    TriangleList.Add(i + 1);
                    TriangleList.Add(i + 2);
                    TriangleList.Add(i + 1);
                    TriangleList.Add(i + 3);
                    TriangleList.Add(i + 2);
                    i += 2;
                }
            }
            vertexs = VertexList.ToArray();
        }
    }
    void winding()
    {
        VertexList.Clear();
        Vector3 moveDir = dir.normalized;
        cylinderCenter = transform.localToWorldMatrix.MultiplyPoint(moveDir * f);
        Matrix4x4 l2w = transform.localToWorldMatrix;
        Matrix4x4 w2l = transform.worldToLocalMatrix;
        for (int i = 0; i < vertexs.Length; i++)
        {

            //Vector3 v = l2w.MultiplyPoint(vertexs[i]);

            Vector3 v = vertexs[i];

            float dis = Vector3.Dot((v - moveDir * f), moveDir);

            if (dis <= 0)
            {

                VertexList.Add( vertexs[i]);
                continue;
            }


            Vector3 newV;
            float moreThanHalfCir = (dis - Mathf.PI * _Radius);
         
            if (moreThanHalfCir >= 0 && IsmoreThanHalfCir)
            {
                newV = new Vector3(v.x - dis * dir.x, v.y - dis * dir.y, v.z + 2 * _Radius) - moreThanHalfCir * dir;
            }
            else
            {
                float angle = Mathf.PI - dis / _Radius;
                float xy = dis - Mathf.Sin(angle) * _Radius;
                float z = _Radius + Mathf.Cos(angle) * _Radius;
                Vector3 vD = v - xy * new Vector3(moveDir.x, moveDir.y, 0);
                newV = new Vector3(vD.x, vD.y, v.z + z);

            }
          //  newV = w2l.MultiplyPoint(newV);
            VertexList.Add(newV);
        }
      //  newMesh.Clear();
        newMesh.vertices = VertexList.ToArray();
        newMesh.triangles = TriangleList.ToArray();
     //   newMesh.uv = uvList.ToArray();
     //   newMesh.colors = clorList.ToArray();
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
   //     newMesh.RecalculateTangents();
       
        GetComponent<MeshFilter>().mesh = newMesh;

    }
    void Update()
    {
        GenratePlane();
        winding();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(point, 0.15f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(cylinderCenter, 0.15f);

    }
}
