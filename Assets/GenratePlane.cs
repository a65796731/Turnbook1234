using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenratePlane : MonoBehaviour
{
    public Graffiti graffiti = null;
    public bool IsCurve;
    public float radiusScale;
    public float width;
    public float PointYScale = 2;
    public float InitPointY = -0.5f;
    public Material material = null;
    public RollProgressedCPU RollProgressedCPU = null;
    private List<int> triangles = new List<int>();
    private List<Vector3> vertexs = new List<Vector3>();
    private List<Vector2> uvs = new List<Vector2>();
    public Transform Brush;

    bool IsBrush;
    int  first;

    Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>().mesh= mesh;
        gameObject.AddComponent<MeshRenderer>().material= material;
       
        RollProgressedCPU.Init1(new Vector3(0, 1f, 0), new Vector3(0, 0, -1f), 0, 1f, 0, 0, 0);
    }
    Vector3 curPos;
    Vector3 oldPos;
    Vector3 lastLeftPoint;
    Vector3 lastRightPoint;
    float curDis;
    float oldDis;
    public void  beginBrush(Transform brush)
    {
        Brush = brush;
        IsBrush = true;
        curPos = Brush.position;
        oldPos = curPos;
        first = 0;
        curDis = 0;
        oldDis = 0;
        vertexs.Clear();
        triangles.Clear();
        uvs.Clear();
        mesh.Clear();
        

    }
    public void endBrush()
    {
      
    IsBrush = false;
        graffiti.reset();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            IsBrush = false;
       

        }
 
        if (IsBrush && (Brush.position - oldPos).magnitude >= 0.5f && first==0)
        {

            first++;
            curPos = Brush.position;
            Vector3 z = (curPos - oldPos).normalized;
            Vector3 x = Vector3.Cross(Vector3.forward.normalized, z);
            Vector3 upDir = z.y > 0 ? -Vector3.up : Vector3.up;
            RollProgressedCPU.SetUpDir(upDir.normalized);
            Vector3 V0 = curPos - (x * width);
            Vector3 v1 = curPos + (x * width);
            lastLeftPoint = transform.worldToLocalMatrix.MultiplyPoint(V0);
            lastRightPoint = transform.worldToLocalMatrix.MultiplyPoint(v1);
            vertexs.Add(lastLeftPoint);
            vertexs.Add(lastRightPoint);
            uvs.Add(Vector2.one);
            uvs.Add(Vector2.one);
            
           
        }
        else if(first==1)
        {
            first++;
        }
        else if (IsBrush && (Brush.position - oldPos).magnitude >= 0.05f && first==2)
        {

            curPos = Brush.position;
            Vector3 z = (curPos - oldPos).normalized;
            Vector3 x = Vector3.Cross(Vector3.forward.normalized, z);
       
      
            Vector3 V0 = curPos - (x * width);
            Vector3 v1 = curPos + (x * width);
            Vector3 newLeftPoint = transform.worldToLocalMatrix.MultiplyPoint(V0) ;
            Vector3 newRightPoint = transform.worldToLocalMatrix.MultiplyPoint(v1) ;
            curDis = (vertexs[0] - newLeftPoint).magnitude;
            if (curDis < oldDis)
                return;
            graffiti.Raycast();
            oldDis = curDis;
            vertexs.Add(newLeftPoint);
            vertexs.Add(newRightPoint);
            lastLeftPoint= newLeftPoint;
            lastRightPoint = newRightPoint;
            oldPos = curPos;
            uvs.Add(Vector2.one);
            uvs.Add(Vector2.one);
        }
        triangles.Clear();
        if (vertexs.Count > 2)
        {
            int i = 0;
            while (i + 2 < vertexs.Count)
            {
                //äÖÈ¾ÕýÃæ
                //triangles.Add(i);
                //triangles.Add(i + 2);
                //triangles.Add(i + 1);
                //triangles.Add(i + 1);
                //triangles.Add(i + 2);
                //triangles.Add(i + 3);
                //äÖÈ¾±³Ãæ
                triangles.Add(i);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
                triangles.Add(i + 1);
                triangles.Add(i + 3);
                triangles.Add(i + 2);
                i += 2;
            }

       
         //   Debug.Log(dis);
           // RollProgressedCPU.SetRadius(1);
            if(IsCurve)
           RollProgressedCPU.SetPointY(InitPointY + -curDis * PointYScale);

            mesh.vertices = RollProgressedCPU.Bend(vertexs.ToArray());
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }


    private void OnDrawGizmos()
    {
        if(vertexs.Count>0)
        {
           for(int i=0;i<vertexs.Count;i++)
            {
                Gizmos.DrawSphere(vertexs[i], 0.1f);
            }
        }
    }
}
