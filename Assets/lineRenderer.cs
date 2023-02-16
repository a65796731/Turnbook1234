using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lineRenderer : MonoBehaviour
{
    public Vector3 _upDir = Vector3.zero;
    public Vector3 _rollDir = Vector3.zero;
    public float MeshTop;
    public float _Radius;
    public float _Deviation;
    public float _PointY;
    public float _PointX;
    public List<Vector3> points = new List<Vector3>();
    LineRenderer linerenderer = null;
    public float lineWidth = 0.3f;
    public Material material = null;

    public bool IsCurve;
    public float radiusScale;
 
    public Transform Brush;

    bool IsBrush;
    bool first;
    void Start()
    {
        linerenderer = GetComponent<LineRenderer>();
       linerenderer.startWidth = lineWidth;
        linerenderer.endWidth = lineWidth;
        linerenderer.positionCount = 0;
        linerenderer.material = material;
        //float step = 2f / 1000f;
        //for (int i = 0; i < 1000; i++)
        //{
        //    //linerenderer.positionCount++;
        //    //linerenderer.SetPosition(linerenderer.positionCount - 1, Vector3.up * i * 0.1f);
        //    //  points.Add(Vector3.up * i * -0.1f);
        //    Vector3 position = Vector3.zero;
        //    position.y = (i + 0.5f) * step - 1f;
        //    position.x = position.y * position.y;
        //    points.Add(position);
        //}
        //  SetPoints(points.ToArray());
    }
    Vector3 curPos;
    Vector3 oldPos;
    public void BeginBrush(Transform brush)
    {
        Brush = brush;
        IsBrush = true;
        curPos = Brush.position;
        oldPos = curPos;
        first = false;
        SetPoint(curPos);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            IsBrush = false;
        }
        if (IsBrush && (Brush.position - oldPos).magnitude >= 0.01f )
        {
            oldPos= curPos = Brush.position;
            SetPoint(curPos);
         
        }
     SetPoints(Bend(points.ToArray()));
    }
    // Update is called once per frame

    public void SetPoint(Vector3 point)
    {
       
        points.Add(point);
    }
    public void SetPointY(float y)
    {
        _PointY = y;
    }
    public void SetPoints(Vector3 []points)
    {
        if (points == null)
            return;
        linerenderer.positionCount = points.Length;

        linerenderer.SetPositions(points);
       
    }
    public int GetPointCount()
    {
        return linerenderer.positionCount;
    }
    List<Vector3> newVertexList = new List<Vector3>();
    public Vector3[] Bend(Vector3[] points)
    {
        if (points.Length <= 1)
            return null;
        float dis = 0;
        if(IsCurve)
         dis= (points[0] - points[points.Length - 1]).magnitude;

     //   _PointY = -dis*1.3f;
        newVertexList.Clear();
      
        //Parallel.For(0, vertex.Length, (i) =>
        {
            for (int i = 0; i < points.Length; i++)
            {
                Vector3 v = points[i];
                Vector3 upDir = _upDir.normalized;
                Vector3 rollDir = _rollDir.normalized;
                float y = _PointY;
                float dp = Vector3.Dot(v - upDir * y, upDir);


                dp = Mathf.Max(0, dp);
                Vector3 fromInitialPos = upDir * dp;
                v -= fromInitialPos;
             
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
}
