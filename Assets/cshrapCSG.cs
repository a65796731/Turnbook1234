using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class cshrapCSG : MonoBehaviour
{
    public Material material = null;
    public Transform B2;
    public Transform cutter;
    public static Vector3[] basicVertices;
    public MeshInfo B_1;
    public MeshInfo cutter1;
    // Start is called before the first frame update
    void Start()
    {
        if (cutter1 == null)
        {
            Mesh mesh1 = B2.GetComponent<MeshFilter>().sharedMesh;
            basicVertices = mesh1.vertices.Clone() as Vector3[];
            B_1 = new MeshInfo();
            B_1.tran = B2;
            B_1.triangles = new List<int>(mesh1.triangles.Length);
            B_1.triangles.AddRange(mesh1.triangles);
            B_1.vertices = new List<Vector3>(mesh1.vertices.Length);
            B_1.vertices.AddRange(mesh1.vertices);
            B_1.uv = new List<Vector2>(mesh1.uv.Length);
            B_1.uv.AddRange(mesh1.uv);
            B_1.flags = new List<int>(B_1.vertices.Count);

            for (int i = 0; i < B_1.vertices.Count; i++)
            {
                B_1.flags.Add(0);
            }
            GameObject go = cutter.gameObject;
            cutter1 = new MeshInfo();
            mesh1 = go.GetComponent<MeshFilter>().sharedMesh;
            cutter1.tran = go.transform;
            cutter1.triangles = new List<int>(mesh1.triangles.Length);
            cutter1.triangles.AddRange(mesh1.triangles);
            cutter1.vertices = new List<Vector3>(mesh1.vertices.Length);
            cutter1.vertices.AddRange(mesh1.vertices);
            cutter1.uv = new List<Vector2>(mesh1.uv.Length);
            cutter1.uv.AddRange(mesh1.uv);
            cutter1.flags = new List<int>(cutter1.vertices.Count);
            for (int i = 0; i < cutter1.vertices.Count; i++)
            {
                cutter1.flags.Add(1);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Space))
        {
            TestCut();
        }
    }
  public  void TestCut()
    {




         doCutting(cutter.gameObject, false);


    }
    public  void doCutting(GameObject cutter, bool generateBlood, bool softbody = true)
    {
        
         _diffrence(cutter, (mesh_diffrence,mesh_intersect, pos, rot) =>
        {


            B2.gameObject.GetComponent<MeshFilter>().mesh = mesh_diffrence;
              GameObject b2_intersect= GameObject.Instantiate<GameObject>(B2.gameObject);
            b2_intersect.transform.position = pos;
            b2_intersect.transform.rotation = rot;
            b2_intersect.transform.localScale = new Vector3(1, 1, 1);
            //GameObject b2_intersect = new GameObject();
            b2_intersect.GetComponent<MeshFilter>().mesh = mesh_intersect;
            //b2_intersect.AddComponent<MeshRenderer>();
            b2_intersect.GetComponent<MeshRenderer>().material = material;
            RollProgressedCPU rollProgressedCPU= b2_intersect.AddComponent<RollProgressedCPU>();
            rollProgressedCPU.Init(new Vector3(0,1f,0),new Vector3(0,0,1f),0,0.5f,0,1.8f,0);
            rollProgressedCPU.IsBend = true;
        });



    }
   

    private void  _diffrence(GameObject cutter, Action<Mesh,Mesh,Vector3,Quaternion> action)
    {

        Mesh meshRest_Difference = null;
        Mesh meshRest_Intersect = null;

        (bool IsSuccess, MeshInfo mesh01) mesh_Difference = default;
        (bool IsSuccess, MeshInfo mesh01) mesh_Intersect = default;
        DateTime start = DateTime.Now;


        mesh_Intersect = CSGBoolean.Intersect(B_1, cutter1);
        mesh_Difference =  CSGBoolean.Difference(B_1, cutter1);

      



        if (!mesh_Difference.IsSuccess || !mesh_Intersect.IsSuccess)
            return;

        DateTime end = DateTime.Now;

        meshRest_Difference = GenerateMesh(mesh_Difference.mesh01);
        meshRest_Intersect = GenerateMesh(mesh_Intersect.mesh01);

        //++Num;
        //aa += (end - start).TotalMilliseconds;



        action(meshRest_Difference,meshRest_Intersect, mesh_Intersect.mesh01.tran.position, mesh_Intersect.mesh01.tran.rotation);
     
    }
    Mesh GenerateMesh(MeshInfo meshInfo)
    {
        Mesh newMesh = new Mesh();

        List<Vector3> newVertices = new List<Vector3>(meshInfo.subVerticesCount + meshInfo.vertices.Count);
        List<int> newTriangles = new List<int>(meshInfo.subTrianglesCount + meshInfo.triangles.Count);
        List<Vector2> newUVs = new List<Vector2>(meshInfo.subVerticesCount + meshInfo.vertices.Count);
     
        List<Color> newColors = new List<Color>(meshInfo.subVerticesCount + meshInfo.vertices.Count);
        newVertices.AddRange(meshInfo.vertices);
        newUVs.AddRange(meshInfo.uv);
        newColors.AddRange(meshInfo.colors);
        for (int i = 0; i < meshInfo.subVertices.Count; i++)
        {
            newVertices.AddRange(meshInfo.subVertices[i]);
            newUVs.AddRange(meshInfo.subUvs[i]);
            newColors.AddRange(meshInfo.subColors[i]);
        }
        newTriangles.AddRange(meshInfo.triangles);

        for (int i = 0; i < meshInfo.subTriangles.Count; i++)
        {
            newTriangles.AddRange(meshInfo.subTriangles[i]);

        }

        newMesh.SetVertices(newVertices);
        newMesh.SetTriangles(newTriangles, 0);

        newMesh.uv = newUVs.ToArray();
        newMesh.colors = newColors.ToArray();
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateBounds();
        meshInfo.normals = newMesh.normals;
        return newMesh;
    }
}
