using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenratePlaneTest : MonoBehaviour
{

    public bool IsDrawPoint;
    public Material material;

    private static int Index = 0;
    public bool IsmoreThanHalfCir;
    public float _Radius;
    //  public Vector3 dir;
    Mesh newMesh;
    Vector3[] vertexs;

    public float f;
    public float speed;
    public float Interval;
    public Vector3 dir;


    public Vector2 originLeftUV;
    public Vector2 originRightUV;
    public Vector3 originLeftPoint;
    public Vector3 originRightPoint;
    public Matrix4x4 originW2L;
    public Matrix4x4 originL2W;
    public void Init(bool ismoreThanHalfCir,bool isDrawDebugPoints,float redius,float speed,float interval,Material material,Texture2D tex)
    {
        _Radius = redius;
        IsmoreThanHalfCir = ismoreThanHalfCir;
        IsDrawPoint = isDrawDebugPoints;
        this.speed = speed;
        Interval = interval;
        this.material = material;
        material.SetTexture("_MainTex", tex);
        newMesh = new Mesh();
        newMesh.name = "organizationstructure " + Index;
        Index++;
        GetComponent<MeshRenderer>().material = material;

    }
    public void recordInitPoint(Vector3 leftPoint, Vector3 rightPoint)
    {
        originW2L = transform.worldToLocalMatrix;
        originL2W = transform.localToWorldMatrix;
        originLeftPoint = originW2L.MultiplyPoint(leftPoint);
        originRightPoint = originW2L.MultiplyPoint(rightPoint);
       
    }
   
    public void correctionPosition(Vector3 inPointLeft, Vector3 inPointRight, out Vector3 outPointLeft, out Vector3 outPointRight)
    {
        Vector3 curLeftPoint = originW2L.MultiplyPoint(inPointLeft);
        Vector3 curRightPoint = originW2L.MultiplyPoint(inPointRight);
        curLeftPoint.x = originLeftPoint.x;
        curRightPoint.x = originRightPoint.x;
        outPointLeft = originL2W.MultiplyPoint(curLeftPoint);
        outPointRight = originL2W.MultiplyPoint(curRightPoint);

    }
    public void correctionPositionAndUV(Vector3 inPointLeft, Vector3 inPointRight,Vector2 inUVLeft,Vector2 inUVRight,
               out Vector3 outPointLeft, out Vector3 outPointRight, out Vector2 outUVLeft,out Vector2 outUVRight)
    {
        Vector3 localLeftPoint = originW2L.MultiplyPoint(inPointLeft);
        Vector3 localRightPoint = originW2L.MultiplyPoint(inPointRight);
        localLeftPoint.x = originLeftPoint.x;
        localRightPoint.x = originRightPoint.x;
        outPointLeft = originL2W.MultiplyPoint(localLeftPoint);
        outPointRight = originL2W.MultiplyPoint(localRightPoint);
        Vector3 dir=  (inPointLeft - originLeftPoint).normalized;
        Vector3 dir1 = (outPointLeft - originLeftPoint).normalized;
        float f = Vector3.Dot(inPointLeft, originLeftPoint)/Vector3.Dot(dir, dir1);
        outUVLeft= originLeftUV + ( inUVLeft- originLeftUV) * f;
         dir = (inPointRight - originRightPoint).normalized;
         dir1 = (outPointRight - originRightPoint).normalized;
         f = Vector3.Dot(inPointRight, originRightPoint) / Vector3.Dot(dir, dir1);
        outUVRight = originRightUV + ( inUVRight-originRightUV) * f;
    }

    public void SetVertex(Vector3 leftPoint, Vector3 rightPoint, Vector2 leftUV, Vector2 rightUV)
    {

     

       
        GenratePlane(leftPoint, rightPoint, leftUV, rightUV);
        winding();
      
  
            

    }
    //bool DistanceBetweenTheLastFourPoints(Vector3 leftPoint, Vector3 rightPoint)
    //{
    //    if (VertexList.Count < 2)
    //        return true;
    //    Matrix4x4 w2l = transform.worldToLocalMatrix;
    //    Vector3 leftPointTemp = w2l.MultiplyPoint(leftPoint);
    //    Vector3 rightPointTemp = w2l.MultiplyPoint(rightPoint);
    //    if (((VertexList[VertexList.Count - 2] - leftPointTemp).magnitude < 0.001f) &&
    //              ((VertexList[VertexList.Count - 1] - rightPointTemp).magnitude < 0.001f))
    //    {

    //        return false;
    //    }
    //    return true;
    //}




    void GenrateGameObject(Vector3 leftPoint, Vector3 rightPoint, Vector3 angle)
    {
        Vector3 centerPoint = rightPoint + (rightPoint - leftPoint) * 0.5f;
        GameObject go = new GameObject();
        go.transform.position = centerPoint;
        go.transform.eulerAngles = angle;
    }
    Vector3 point = Vector3.zero;
    Vector3 cylinderCenter;
    // Update is called once per frame

    List<Vector3> VertexList = new List<Vector3>();
    List<int> TriangleList = new List<int>();
    List<Vector2> uvList = new List<Vector2>();
    List<Color> colorList = new List<Color>();
    List<Vector3> worldPointList = new List<Vector3>();
    void GenratePlane(Vector3 leftPoint, Vector3 rightPoint, Vector2 leftUV, Vector2 rightUV)
    {

        SetupVertexs(leftPoint, rightPoint, leftUV, rightUV);
      
       SetupTriangles();
        
        vertexs = VertexList.ToArray();
        // newMesh.vertices = vertexs;
        // newMesh.triangles = TriangleList.ToArray();
        // newMesh.uv = uvList.ToArray();
        // newMesh.colors = clorList.ToArray();
        // newMesh.RecalculateBounds();
        // newMesh.RecalculateNormals();
        // newMesh.RecalculateTangents();

        // GetComponent<MeshFilter>().mesh = newMesh;
    }
    List<Vector3> windingVertexList = new List<Vector3>();
    
    void winding()
    {
        if (vertexs.Length <= 2)
            return;

        windingVertexList.Clear();
        int totalOuterCircle = 0;
        float dis = Vector3.Dot((vertexs[0] - dir * f), dir);
        totalOuterCircle = (int)(dis - Mathf.PI * _Radius * 2 + 1);
        totalOuterCircle = totalOuterCircle < 0 ? 0 : totalOuterCircle;
    //    Debug.Log("dis :" + dis + " , " + (dis - Mathf.PI * _Radius * 2));
        for (int i = 0; i < vertexs.Length-2; i += 2)
        {
          
            cylinderCenter = transform.localToWorldMatrix.MultiplyPoint(dir * f);

            //Vector3 lastLeftPoint = VertexList[0];
            //Vector3 lastRightPoint = VertexList[1];
            //Vector3 curLeftPoint = VertexList[2];
            //Vector3 curRightPoint = VertexList[3];
            //Vector3 lastMidPoint = lastLeftPoint + (lastRightPoint - lastLeftPoint) * 0.5f;
            //Vector3 curMidPoint = curLeftPoint + (curRightPoint - curLeftPoint) * 0.5f;
            //dir = (curMidPoint - lastMidPoint).normalized;
            for (int j = 0; j < 2; j++)
            {


                Vector3 thisPoint = VertexList[i + j];
                Vector3 target = VertexList[i + j + 2];
                dir = -(target - thisPoint).normalized;
              dir = Vector3.up;
                Vector3 v = vertexs[i + j];

                 dis = Vector3.Dot((v - dir * f), dir);
              
                // float dis = Vector3.Dot((v - dir * f), dir);
                if (dis <= 0)
                {

                    windingVertexList.Add(vertexs[i + j]);
                    continue;
                }


                Vector3 newV;
                float moreThanHalfCir = (dis - Mathf.PI * _Radius);
             
              
                if (moreThanHalfCir >= 0 && IsmoreThanHalfCir)
                {
                    newV = new Vector3(v.x - dis * dir.x, v.y - dis * dir.y, v.z - 2 * _Radius) - moreThanHalfCir * dir;
                }
                else
                {
                    int curOuterCircle = (int)(dis - Mathf.PI * _Radius * 2 + 1);
                    curOuterCircle = curOuterCircle < 0 ? 0 : curOuterCircle;
                    int count= (totalOuterCircle - curOuterCircle);
                    float radius = _Radius + 0.005f* count;
                    float angle = Mathf.PI - dis / radius;
                    float xy = dis - Mathf.Sin(angle) * radius;
                    float z = radius + Mathf.Cos(angle) * radius;
                    Vector3 vD = v - xy * new Vector3(dir.x, dir.y, 0);
                    newV = new Vector3(vD.x, vD.y, v.z - z);

                }

                windingVertexList.Add(newV);
            }
        }

        windingVertexList.Add(vertexs[vertexs.Length-2]);
        windingVertexList.Add(vertexs[vertexs.Length - 1]);
        newMesh.Clear();
        newMesh.vertices = windingVertexList.ToArray();
        newMesh.triangles = TriangleList.ToArray();
        newMesh.uv = uvList.ToArray();
        newMesh.colors = colorList.ToArray();
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();

        GetComponent<MeshFilter>().mesh = newMesh;

    }
    void SetupVertexs(Vector3 leftPoint, Vector3 rightPoint, Vector2 leftUV, Vector2 rightUV)
    {
        Matrix4x4 w2l = transform.worldToLocalMatrix;
        //计算LastPoint和CurPoint的距离是否过大,过大需要进行补点
        if (VertexList.Count >= 2)
        {
            Vector3 lastLeftPoint = worldPointList[worldPointList.Count - 2];
            Vector3 lastRightPoint = worldPointList[worldPointList.Count - 1];
            float length = (leftPoint - lastLeftPoint).magnitude / Interval;
            if (length >= 3.0f)
            {
                Vector2 lastLeftUV = uvList[uvList.Count - 2];
                Vector2 lastRightUV = uvList[uvList.Count - 1];
                int count = Mathf.RoundToInt(length - 1.0f);
                float percentage = 1.0f / count;
                for (int i = 1; i < count; i++)
                {
                    float t = i * percentage;
                    Vector3 midLeftPoint = Vector3.Lerp(lastLeftPoint, leftPoint, t);
                    Vector3 midRightPoint = Vector3.Lerp(lastRightPoint, rightPoint, t);
                    Vector2 midLeftUV = Vector2.Lerp(lastLeftUV, leftUV, t);
                    Vector2 midRightUV = Vector2.Lerp(lastRightUV, rightUV, t);
                    AddMeshData(w2l, new Vector3[] { midLeftPoint, midRightPoint },
                                new Vector2[] { midLeftUV, midRightUV }, Color.white);
                   
                }

            }
          
        }
        AddMeshData(w2l, new Vector3[] { leftPoint, rightPoint },
                     new Vector2[] { leftUV, rightUV }, Color.white);
       
    }
    void SetupTriangles()
    {
        if (VertexList.Count <= 2)
            return;
        TriangleList.Clear();
        int i = 0;
        while (i + 2 < VertexList.Count)
        {
            //渲染正面
            TriangleList.Add(i);
            TriangleList.Add(i + 2);
            TriangleList.Add(i + 1);
            TriangleList.Add(i + 1);
            TriangleList.Add(i + 2);
            TriangleList.Add(i + 3);
            ////渲染背面
            //TriangleList.Add(i);
            //TriangleList.Add(i + 1);
            //TriangleList.Add(i + 2);
            //TriangleList.Add(i + 1);
            //TriangleList.Add(i + 3);
            //TriangleList.Add(i + 2);
            i += 2;
        }
    }
    void CalculationDistance()
    {
        if (VertexList.Count < 4)
            return;
        Vector3 lastPointL = VertexList[VertexList.Count - 4];
        Vector3 lastPointR = VertexList[VertexList.Count - 3];
        Vector3 curPointL = VertexList[VertexList.Count - 2];
        Vector3 curPointR = VertexList[VertexList.Count - 1];
        Vector3 curCenterPoint = curPointL + (curPointR - curPointL) * 0.5f;
        Vector3 lastCenterPoint = lastPointL + (lastPointR - lastPointL) * 0.5f;
        float dis = (curCenterPoint - lastCenterPoint).magnitude;
        f -= dis * speed;
    }
    void AddMeshData(Matrix4x4 w2l, Vector3[] worldPoints, Vector2[] uvs, Color color)
    {
        worldPointList.Add(worldPoints[0]);
        worldPointList.Add(worldPoints[1]);
        VertexList.Add(w2l.MultiplyPoint(worldPoints[0]));
        VertexList.Add(w2l.MultiplyPoint(worldPoints[1]));
        uvList.Add(uvs[0]);
        uvList.Add(uvs[1]);
        colorList.Add(color);
        colorList.Add(color);
        CalculationDistance();
    }
  public  List<Vector2> getUVs()
    {
        return uvList;
    }
    private void OnDrawGizmos()
    {
        //  Gizmos.DrawSphere(point, 0.15f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(cylinderCenter, 0.03f);
        if (IsDrawPoint)
        {
            Gizmos.color = Color.blue;
        for (int i = 0; i < worldPointList.Count; i++)
        {
            Gizmos.DrawSphere(worldPointList[i], 0.01f);
        }
        }
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        //    Gizmos.DrawLine(transform.position,transform.position+dir*10);
    }
}
