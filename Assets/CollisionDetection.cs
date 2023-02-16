using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public float NormalScale=1.0f;
    public GameObject hotSpot= null;
    public Transform lookAtTarget = null;
    public Transform sphere=null;
    public bool isCollision;
    public bool InArea;
    public GenratePlane[] lineRenders = null;
    public Camera cam = null;
    public Transform Cutter = null;
    public Transform Target = null;
    public cshrapCSG newCSG = null;

    private Vector3[] Vertexs;
    private Vector3[] Normals;
    public static bool test;
    private void Start()
    {
       Mesh mesh=    sphere.GetComponent<MeshFilter>().mesh;
        Vertexs = mesh.vertices;
        Normals = mesh.normals;
        hotSpot.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        test = true;
       // Debug.Log("碰撞对象：" + other.transform.name);
       // if (other.name.Contains("股骨")&& !isCollision)
       // {
       //     isCollision = true;
       //     Target.position = sphere.position;
       //     foreach (var v in lineRenders)
       //         v.beginBrush(sphere);
       // }
       //else  if (other.tag.Contains("TouchArea") && !InArea)
       // {
       //     InArea = true;

        // }
        //hotSpot.gameObject.SetActive(true);
        //hotSpot.transform.position = sphere.position;
        // hotSpot.transform.LookAt(lookAtTarget);


    }
    private void OnTriggerExit(Collider other)
    {
        test = false;
        //if (other.name.Contains("股骨"))
        //{
        //    isCollision = false;

        //    foreach (var v in lineRenders)
        //        v.endBrush();
        //}
        //else if (other.tag.Contains("TouchArea") )
        //{
        //    InArea = false;
        //    foreach (var v in lineRenders)
        //        v.endBrush();
        //}
    }
    RaycastHit[] raycastHit = new RaycastHit[10];

    private void OnDrawGizmos()
    {
        if (Vertexs == null)
            return;
        for(int i=0;i<Vertexs.Length;i++)
        {
            Vector3 from = Vertexs[i];
            Vector3 to = Vertexs[i] + Normals[i]* NormalScale;
            Matrix4x4 l2w= sphere.localToWorldMatrix;
   
            Gizmos.DrawLine(l2w.MultiplyPoint(from), l2w.MultiplyPoint(to));
        }
    }
    private void Update()
    {
       // CutButton();
    }
    List<RaycastHit> raycastsList = new List<RaycastHit>();
    List<Vector3> vertexsList = new List<Vector3>();
    private void CutButton()
    {
        raycastsList.Clear();
        vertexsList.Clear();
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("hotSpot");
        for (int j = 0; j < Vertexs.Length; j++)
        {
            Matrix4x4 l2w = sphere.localToWorldMatrix;
            Vector3 from = l2w.MultiplyPoint(Vertexs[j]);
            Vector3 to = l2w.MultiplyPoint(Vertexs[j] + Normals[j]* NormalScale);
         

            RaycastHit[] raycastHits = new RaycastHit[1];

            int hitCount = Physics.RaycastNonAlloc(from, (to-from).normalized, raycastHits,(to-from).magnitude, layerMask);
            if (hitCount > 0)
            {
                raycastsList.Add(raycastHits[0]);
                vertexsList.Add(from);
                

            }

        }
        if (raycastsList.Count == 0)
            return;
        float dis = float.MaxValue;
        int index = -1;

        for(int i=0;i< raycastsList.Count;i++)
        {
           if(dis> raycastsList[i].distance)
            {
                dis = raycastsList[i].distance;
                index = i;
            }
        }
      
        RaycastHit hit=    raycastsList[index];
        Vector3 collDir=  (vertexsList[index] - hit.point).normalized;
        Debug.DrawLine(hit.point, hit.point + collDir*NormalScale, Color.red);

        foreach (var v in lineRenders)
            v.beginBrush(sphere);
    }
}
