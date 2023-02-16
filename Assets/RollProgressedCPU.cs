using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;



public class RollProgressedCPU : MonoBehaviour
{
    public bool IsBend = false;
    public Vector3 _upDir = Vector3.zero;
    public Vector3 _rollDir = Vector3.zero;
    public float MeshTop;
    public float _Radius;
    public float _Deviation;
    public float _PointY;
    public float _PointX;
    Vector3[] vertex = null;
    Vector3[] newVertex = null;
    List<Vector3> newVertexList = new List<Vector3>();
    Mesh newMesh;
    // Start is called before the first frame update
    
  public  void Init(Vector3 upDir, Vector3 rollDir,float meshTop,float radus,float deviation,
        float pointY,float pointX)
    {
        _upDir = upDir;
        _rollDir = rollDir; 
        _Radius = radus;
        _Deviation= deviation;
        _PointY = pointY;
        _PointX = pointX;
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        newMesh = Mesh.Instantiate(mesh);


        vertex = newMesh.vertices;
        newVertex = new Vector3[vertex.Length];
        GetComponent<MeshFilter>().sharedMesh = newMesh;
    }
    public void Init1(Vector3 upDir, Vector3 rollDir, float meshTop, float radus, float deviation,
        float pointY, float pointX)
    {
        _upDir = upDir;
        _rollDir = rollDir;
        _Radius = radus;
        _Deviation = deviation;
        _PointY = pointY;
        _PointX = pointX;
       

      
    }
    // Update is called once per frame
    void Update()
    {
        if(IsBend)
        Bend();
    }
    public void SetUpDir(Vector3 dir)
    {
        _upDir = dir;
    }
    public void SetRadius(float r)
    {
        _Radius = r;
    }
    public void SetPointY(float pointY)
    {
        _PointY = pointY;
         }
    public float SCALE = 1;
    public Vector3[] Bend(Vector3[] vertex)
    {
        newVertexList.Clear();
        Matrix4x4 worldMat = transform.localToWorldMatrix;
        //Parallel.For(0, vertex.Length, (i) =>
        {
            Vector3 upDir = _upDir.normalized;
            Vector3 rollDir = _rollDir.normalized;
            for (int i = 0; i < vertex.Length; i++)
            {
                Vector3 v = vertex[i];
          
                float y = _PointY;
                float dp = Vector3.Dot(v- upDir * y, upDir);


                dp = Mathf.Max(0, dp);
                Vector3 fromInitialPos = upDir * dp* SCALE;
                v -= fromInitialPos;
                if (i == 0)
                {
                    Debug.DrawLine(v, v + Vector3.up);
                }
                float radius = _Radius + _Deviation * Mathf.Max(0, -(y - MeshTop));
                float length = 2 * Mathf.PI * (radius - _Deviation * Mathf.Max(0, -(y - MeshTop)) / 2);
                float r = dp / Mathf.Max(0, length);
                float a = 2 * r * Mathf.PI;
                float s = Mathf.Sin(a);
                float c = Mathf.Cos(a);
                float one_minus_c = 1.0f - c;
                Vector3 axis = Vector3.Cross(upDir, rollDir).normalized;
                Matrix4x4 rot_mat = new Matrix4x4();
                rot_mat.SetRow(0, new Vector4(one_minus_c * axis.x * axis.x + c, one_minus_c * axis.x * axis.y - axis.z * s, one_minus_c * axis.z * axis.x + axis.y * s));
                rot_mat.SetRow(1, new Vector4(one_minus_c * axis.x * axis.y + axis.z * s, one_minus_c * axis.y * axis.y + c, one_minus_c * axis.y * axis.z - axis.x * s));
                rot_mat.SetRow(2, new Vector4(one_minus_c * axis.z * axis.x - axis.y * s, one_minus_c * axis.y * axis.z + axis.x * s, one_minus_c * axis.z * axis.z + c));
                Vector3 cycleCenter = rollDir * _PointX + rollDir * radius + upDir * y;
                Vector3 fromCenter = v - cycleCenter;
                Vector3 shiftFromCenterAxis = Vector3.Cross(axis, fromCenter);
                shiftFromCenterAxis = Vector3.Cross(shiftFromCenterAxis, axis).normalized;

                fromCenter -= shiftFromCenterAxis * _Deviation * dp;// * ;
                v = rot_mat * new Vector4(fromCenter.x, fromCenter.y, fromCenter.z, 0) + new Vector4(cycleCenter.x, cycleCenter.y, cycleCenter.z, 0);

                newVertexList.Add(v);
              
            }

        }
        return newVertexList.ToArray();
        //);
     
    }
    
    public void Bend()
    {
        Matrix4x4 worldMat = transform.localToWorldMatrix;
        //Parallel.For(0, vertex.Length, (i) =>
        {
            for (int i = 0; i < vertex.Length; i++)
            {
                Vector3 v = vertex[i];
                Vector3 upDir = _upDir.normalized;
                Vector3 rollDir = _rollDir.normalized;
                float y = _PointY;
                float dp = Vector3.Dot(v - upDir * y, upDir);
              
               
                dp = Mathf.Max(0, dp);
                Vector3 fromInitialPos = upDir * dp;
                v -= fromInitialPos;
                if (i == 0)
                {
                    Debug.DrawLine(v, v + Vector3.up);
                }
                float radius = _Radius + _Deviation * Mathf.Max(0, -(y - MeshTop));
                float length = 2 * Mathf.PI * (radius - _Deviation * Mathf.Max(0, -(y - MeshTop)) / 2);
                float r = dp / Mathf.Max(0, length);
                float a = 2 * r * Mathf.PI;
                float s = Mathf.Sin(a);
                float c = Mathf.Cos(a);
                float one_minus_c = 1.0f - c;
                Vector3 axis = Vector3.Cross(upDir, rollDir).normalized;
                Matrix4x4 rot_mat = new Matrix4x4();
                rot_mat.SetRow(0, new Vector4(one_minus_c * axis.x * axis.x + c, one_minus_c * axis.x * axis.y - axis.z * s, one_minus_c * axis.z * axis.x + axis.y * s));
                rot_mat.SetRow(1, new Vector4(one_minus_c * axis.x * axis.y + axis.z * s, one_minus_c * axis.y * axis.y + c, one_minus_c * axis.y * axis.z - axis.x * s));
                rot_mat.SetRow(2, new Vector4(one_minus_c * axis.z * axis.x - axis.y * s, one_minus_c * axis.y * axis.z + axis.x * s, one_minus_c * axis.z * axis.z + c));
                Vector3 cycleCenter = rollDir * _PointX + rollDir * radius + upDir * y;
                Vector3 fromCenter = v - cycleCenter;
                Vector3 shiftFromCenterAxis = Vector3.Cross(axis, fromCenter);
                shiftFromCenterAxis = Vector3.Cross(shiftFromCenterAxis, axis).normalized;

                fromCenter -= shiftFromCenterAxis * _Deviation * dp;// * ;
                v = rot_mat * new Vector4(fromCenter.x, fromCenter.y, fromCenter.z, 0) + new Vector4(cycleCenter.x, cycleCenter.y, cycleCenter.z, 0);


                newVertex[i] = v;
            }

        }

        //);
        newMesh.SetVertices(newVertex);
        newMesh.RecalculateBounds();
        GetComponent<MeshFilter>().sharedMesh = newMesh;
    }
 
}
