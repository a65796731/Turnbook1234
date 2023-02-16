using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGenratePlane : MonoBehaviour
{
    public float minDis=0.5f;
    public float NormalScale;
    public float planeWidth=1.0f;
    public Transform sphere;
    public RayTest rayTest;
    public Material material = null;
    Mesh mesh;

    private Vector3[] Vertexs;
    private Vector3[] Normals;
    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        gameObject.AddComponent<MeshRenderer>().material = material;
        Mesh spheremesh = sphere.GetComponent<MeshFilter>().mesh;
        Vertexs = spheremesh.vertices;
        Normals = spheremesh.normals;
        mesh.name = "11";
    }
    Vector3 curPos;
    Vector3 oldPos;
    Vector3 lastLeftPoint;
    Vector3 lastRightPoint;
    float curDis;
    float oldDis;
    List<Vector3> planeVertexList=new  List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    private List<int> planeTriangleList = new List<int>();
    private void Update()
    {
        //if (CollisionDetectionTest.test&&(sphere.position - oldPos).magnitude >= 0.05f)
        //{
            oldPos = curPos = sphere.position;
            Vector3 leftPoint = Vector3.zero;
            Vector3 rightPoint = Vector3.zero;
            if (rayTest.CutButton(ref leftPoint, ref rightPoint))
            {

            leftPoint = transform.worldToLocalMatrix.MultiplyPoint(leftPoint);
            rightPoint = transform.worldToLocalMatrix.MultiplyPoint(rightPoint);
         
            if (planeVertexList.Count>2)
            {
                // float minDisTemp = float.MaxValue;
                //for(int i= planeVertexList.Count-4; i< planeVertexList.Count;i++)
                //{
                //     int index=(i + 1) % planeVertexList.Count == 0 ? planeVertexList.Count - 4 : (i + 1);
                //    float dis=(planeVertexList[i] - planeVertexList[index]).magnitude;

                //    if(dis< minDisTemp)
                //    {
                //        minDisTemp = dis;
                //    }
                //}
             
                if (((planeVertexList[planeVertexList.Count - 2] - leftPoint).magnitude < 0.001f)&&
                   ((planeVertexList[planeVertexList.Count - 1] - rightPoint).magnitude < 0.001f))
                {
                    planeVertexList.RemoveAt(planeVertexList.Count-1);
                    planeVertexList.RemoveAt(planeVertexList.Count - 1);
                    return;
                }
                  
            }


            planeVertexList.Add(leftPoint);
            planeVertexList.Add(rightPoint);

            uvs.Add(Vector2.one);
            uvs.Add(Vector2.one);
            planeTriangleList.Clear();
            if (planeVertexList.Count > 2)
            {
                int i = 0;
                while (i + 2 < planeVertexList.Count)
                {
                    //äÖÈ¾ÕýÃæ
                    planeTriangleList.Add(i);
                    planeTriangleList.Add(i + 2);
                    planeTriangleList.Add(i + 1);
                    planeTriangleList.Add(i + 1);
                    planeTriangleList.Add(i + 2);
                    planeTriangleList.Add(i + 3);
                    //äÖÈ¾±³Ãæ
                    //planeTriangleList.Add(i);
                    //planeTriangleList.Add(i + 1);
                    //planeTriangleList.Add(i + 2);
                    //planeTriangleList.Add(i + 1);
                    //planeTriangleList.Add(i + 3);
                    //planeTriangleList.Add(i + 2);
                    i += 2;
                }
            }


            mesh.vertices = planeVertexList.ToArray();
            mesh.triangles = planeTriangleList.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();


        }
       // }
       
    }

    //    Vector3 collDir = (rayPoint - hitPoint).normalized;
    //  //  Debug.DrawLine(hitPoint, hitPoint + collDir * NormalScale, Color.red);

    //    float[] f = new float[3];
    //    f[0] = Mathf.Abs( Vector3.Dot(collDir, Vector3.up));
    //    f[1] = Mathf.Abs(Vector3.Dot(collDir, Vector3.forward));
    //   f[2] = Mathf.Abs(Vector3.Dot(collDir, Vector3.right));

    //    float tempF = float.MinValue;
    //    int index = -1;
    //    for (int i = 0; i < f.Length; i++)
    //    {
    //        if (f[i] > tempF)
    //        {
    //            tempF = f[i];
    //            index = i;
    //        }
    //    }
    //    Vector3 up = Vector3.zero;
    //    Vector3 right = Vector3.zero;
    //    Vector3 forward = Vector3.zero;
    //    if (index == 0)
    //    {
    //        up = collDir;
    //        right = Vector3.Cross(Vector3.forward.normalized, up);
    //        forward = Vector3.Cross(right, up);

    //    }
    //    else if (index == 2)
    //    {
    //        right = collDir;
    //        up = Vector3.Cross(Vector3.forward.normalized, right);
    //        forward = Vector3.Cross(up, right);
    //    }
    //    else if (index == 1)
    //    {
    //        forward = collDir;
    //        right = Vector3.Cross(Vector3.up.normalized, forward);
    //        up = Vector3.Cross(right, forward);
    //    }

    //Vector3 dir;
    //if (index == 0 || index == 1)
    //{
    //    dir = right;

    //}
    //else
    //{
    //    dir = forward;
    //}
    //dir *= planeWidth;
    //Vector3 pointL = hitPoint + dir;
    //Vector3 pointR = hitPoint - dir;
    //Debug.DrawLine(hitPoint, pointL, Color.blue);
    //Debug.DrawLine(hitPoint, pointR, Color.red);
    List<RaycastHit> raycastsList = new List<RaycastHit>();
    List<Vector3> vertexsList = new List<Vector3>();
    private bool CutButton(ref Vector3 hitPoint,ref Vector3 rayPoint)
    {
        raycastsList.Clear();
        vertexsList.Clear();
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("hotSpot");
        for (int j = 0; j < Vertexs.Length; j++)
        {
            Matrix4x4 l2w = sphere.localToWorldMatrix;
            Vector3 from = l2w.MultiplyPoint(Vertexs[j]);
            Vector3 to = l2w.MultiplyPoint(Vertexs[j] + Normals[j] * NormalScale);


            RaycastHit[] raycastHits = new RaycastHit[1];

            int hitCount = Physics.RaycastNonAlloc(from, (to - from).normalized, raycastHits, (to - from).magnitude, layerMask);
            if (hitCount > 0)
            {
                raycastsList.Add(raycastHits[0]);
                vertexsList.Add(from);


            }

        }
        if (raycastsList.Count == 0)
            return false;
        float dis = float.MaxValue;
        int index = -1;

        for (int i = 0; i < raycastsList.Count; i++)
        {
            if (dis > raycastsList[i].distance)
            {
                dis = raycastsList[i].distance;
                index = i;
            }
        }

     //   RaycastHit hit = raycastsList[index];
        //Vector3 collDir = (vertexsList[index] - hit.point).normalized;
        //Debug.DrawLine(hit.point, hit.point + collDir * NormalScale, Color.red);
        hitPoint = raycastsList[index].point;
        rayPoint = vertexsList[index];
        return true;
    }
}
