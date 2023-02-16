using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class NewCSG : MonoBehaviour
{
    public Text text = null;
    private delegate void DebugCallback(string message);

    [DllImport("CSGDLL")]
    private static extern void RegisterDebugCallback(DebugCallback callback);
    [DllImport("CSGDLL")]
    static extern void InitMemoryPool(int count);
    [DllImport("CSGDLL")]
    static extern void ResetMemoryPool();
    [DllImport("CSGDLL")]
    static extern bool Difference(double[] Mesh01_vertex,int Mesh01_vertexLen, int[] Mesh_01trangles, int Mesh01_tranglesLen,
       double[] Mesh02_vertex, int Mesh02_vertexLen, int[] Mesh_02trangles, int Mesh02_tranglesLen,
       ref IntPtr outVertexs, ref IntPtr outTriangles, ref int outVertesxLen, ref int ouTrianglesLen);
    [DllImport("CSGDLL")]
    static extern bool createSolidTest(double[] Mesh01_vertex, int Mesh01_vertexLen, int[] Mesh_01trangles, int Mesh01_tranglesLen,
      double[] Mesh02_vertex, int Mesh02_vertexLen, int[] Mesh_02trangles, int Mesh02_tranglesLen);

    [DllImport("CSGDLL")]
    static extern bool DifferenceTest();
    [DllImport("CSGDLL")]
    static extern bool ParseSolidTest(ref IntPtr outVertexs, ref IntPtr outTriangles, ref int outVertesxLen, ref int ouTrianglesLen);
    public static NewCSG Instance;
    IntPtr trianglesPtr = IntPtr.Zero;
    IntPtr vertexPtr = IntPtr.Zero;
    int vertexLen = 0;
    int triangleLen = 0;
    double[] vertexPoint = null;
    int[] triangles = null;
    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        DateTime start = DateTime.Now;
        RegisterDebugCallback(new DebugCallback(DebugMethod));
        InitMemoryPool(300000);
        DateTime end = DateTime.Now;
        //    text.text += "生成内存池耗时：" + (end - start).TotalMilliseconds + "\n";
        Debug.Log("生成内存池耗时：" + (end - start).TotalMilliseconds);
        //    ResetMemoryPool();
        B2.GetComponent<MeshFilter>().sharedMesh=   Mesh.Instantiate(B2.GetComponent<MeshFilter>().sharedMesh);
    }
    private static void DebugMethod(string message)
    {
        Debug.Log("Dll: " + message);
    }
    public Transform B2;
    public Transform cutter;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {

            B2.GetComponent<MeshFilter>().sharedMesh=   Difference(B2, cutter);
        }
    }
    public Mesh Difference(Transform meshA, Transform meshB, Action<List<Vector3>, List<int>, Matrix4x4> _onChipMesh = null)
    {
        double[] vertices_B2 = null;
        int[] trangles_B2 = null;
        double[] vertices_cutter = null;
        int[] trangles_cutter = null;
        GenerateMeshData(meshA, out vertices_B2, out trangles_B2);
        // GenerateMeshData(meshB, meshA.worldToLocalMatrix, out vertices_cutter, out trangles_cutter);
        GenerateMeshData(meshB, out vertices_cutter, out trangles_cutter);
        //Difference(vertices_B2, vertices_B2.Length, trangles_B2, trangles_B2.Length, vertices_cutter, vertices_cutter.Length,
        //       trangles_cutter, trangles_cutter.Length, ref vertexPtr, ref trianglesPtr, ref vertexLen, ref triangleLen);
        createSolidTest(vertices_B2, vertices_B2.Length, trangles_B2, trangles_B2.Length, vertices_cutter, vertices_cutter.Length,
             trangles_cutter, trangles_cutter.Length);
        DateTime start = DateTime.Now;
        DifferenceTest();



    

     
        vertexPoint = new double[vertexLen];

      
        ParseSolidTest(ref vertexPtr, ref trianglesPtr, ref vertexLen, ref triangleLen);
       
       

      
        vertexPoint = new double[vertexLen];
        triangles = new int[triangleLen];
        System.Runtime.InteropServices.Marshal.Copy(vertexPtr, vertexPoint, 0, vertexLen);
        System.Runtime.InteropServices.Marshal.Copy(trianglesPtr, triangles, 0, triangleLen);

        vertexPtr = IntPtr.Zero;
        trianglesPtr = IntPtr.Zero;

        StartCoroutine(IEResetMemoryPool(0.1f));
        return GenerateNewObj(meshA,vertexPoint, triangles, meshA.position, meshA.rotation, meshA.localScale);
    }
    void GenerateMeshData(Transform tran,out double[] wVertices,out int []wTrangles)
    { 
       MeshFilter meshFilter= tran.GetComponent<MeshFilter>();
      
        Matrix4x4 lw2 = tran.localToWorldMatrix;
       Mesh mesh=  meshFilter.sharedMesh;
       Vector3[] lVertices = mesh.vertices;
       int[] lTrangles = mesh.triangles;
        int verticeLen = lVertices.Length;
        int trangleLen = lTrangles.Length;
   
        double[] wVerticesTemp = new double[verticeLen * 3];
        int[] wTranglesTemp = new int[trangleLen];
      
        Parallel.For(0, verticeLen, (index) =>
        {
          Vector3 w=  lw2.MultiplyPoint(lVertices[index]);
         //  Vector3 w = lVertices[index];
            wVerticesTemp[index * 3 + 0] = w.x;
            wVerticesTemp[index * 3 + 1] = w.y;
            wVerticesTemp[index * 3 + 2] = w.z;
        });
       
        Array.Copy(lTrangles, wTranglesTemp, trangleLen);
        wVertices = wVerticesTemp;
        wTrangles = wTranglesTemp;
    
    }
    void GenerateMeshData(Transform tran,Matrix4x4 w2l, out double[] wVertices, out int[] wTrangles)
    {
        MeshFilter meshFilter = tran.GetComponent<MeshFilter>();
        Matrix4x4 lw2 = tran.localToWorldMatrix;
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] lVertices = mesh.vertices;
        int[] lTrangles = mesh.triangles;
        int verticeLen = lVertices.Length;
        int trangleLen = lTrangles.Length;

        double[] wVerticesTemp = new double[verticeLen * 3];
        int[] wTranglesTemp = new int[trangleLen];

        Parallel.For(0, verticeLen, (index) =>
        {
            Vector3 w = lw2.MultiplyPoint(lVertices[index]);
           w= w2l.MultiplyPoint(w);
            wVerticesTemp[index * 3 + 0] = w.x;
            wVerticesTemp[index * 3 + 1] = w.y;
            wVerticesTemp[index * 3 + 2] = w.z;
        });
        Array.Copy(lTrangles, wTranglesTemp, trangleLen);
        wVertices = wVerticesTemp;
        wTrangles = wTranglesTemp;
    }
    
   
    Mesh GenerateNewObj(Transform oldB1, double[] vertices, int[] trangles, Vector3 position, Quaternion qua, Vector3 scale)
    {
    Matrix4x4 w2l=    oldB1.transform.worldToLocalMatrix;
        Vector3[] vertex = new Vector3[vertexLen / 3];
      int len=  vertexLen / 3;
        Parallel.For(0, len, (i) =>
        {
            
            vertex[i] = new Vector3();
            vertex[i].x = (float)vertices[3 * i + 0];
            vertex[i].y = (float)vertices[3 * i + 1];
            vertex[i].z = (float)vertices[3 * i + 2];
            vertex[i]=  w2l.MultiplyPoint(vertex[i]);


        });

        //renderer.material = B2.GetComponent<MeshRenderer>().material;
        //for (int i = 0; i < vertex.Length; i++)
        //{
        //    Debug.Log("vertex :" + i + "  :" + vertex[i]);
        //}
        //for (int i = 0; i < trangles.Length; i++)
        //{
        //    Debug.Log("trangles :" + i + "  :" + trangles[i]);
        //}
        Mesh newMesh = new Mesh();
        newMesh.SetVertices(vertex);
        newMesh.SetTriangles(trangles, 0);
        newMesh.uv = CalculationUV(newMesh, oldB1.GetComponent<MeshFilter>().mesh, oldB1.transform.worldToLocalMatrix);
        newMesh.RecalculateNormals();
        newMesh.RecalculateTangents();
        newMesh.RecalculateBounds();
  
        return newMesh;
    }
    Vector2[] CalculationUV(Mesh newMesh,Mesh oldMesh,Matrix4x4 w2l)
    {
        Vector3[] newVectors = newMesh.vertices;
        Vector3[] oldVectors = oldMesh.vertices;
        Vector2[] newUV = new Vector2[newVectors.Length];
        Vector2[] oldUV = oldMesh.uv;

        Parallel.For(0, newVectors.Length, (k) =>
        {
         // Vector3 v3 = w2l.MultiplyPoint(newVectors[k]);
          Vector3 v3 = newVectors[k];
            float d = float.MaxValue;
            float d1 = float.MaxValue;
            float d2 = float.MaxValue;
            int idxUV1 = -1;
            int idxUV2 = -1;
            for (int i = 0; i < oldVectors.Length; i++)
            {
                if (v3 == oldVectors[i])
                {
                    //idxUV = i;
                    //相同UV直接赋值
                    newUV[k] = oldUV[i];
                    return;
                }
                // (v3-oldUV[i])
              
               float dt = Vector3.Distance(v3, oldVectors[i]);
           //    float dt = (v3 - oldVectors[i]).magnitude;
                if (dt < d)
                {
                    idxUV2 = idxUV1;
                    idxUV1 = i;
                    d2 = d1;
                    d1 = dt;
                    d = dt;
                }
            }
        
            // uvs[k] = uvsRef[idxUV]; 
            if (d < 100f)
            {
                newUV[k] = oldUV[idxUV1];
            }
            else
            if (idxUV2 != -1)
            {  //就近2个点，插值算UV
                Vector2 uv1 = oldUV[idxUV1];
                Vector2 uv2 = oldUV[idxUV2];
                float t1_2 = d1 / (d1 + d2);
                newUV[k] = Vector2.Lerp(uv1, uv2, t1_2);
            }
            else
            {
                newUV[k] = oldUV[0];
               
            }

            // uvs[k] = uvsRef[idxUV1];
        });
      return newUV;
        //--
        // Debug.Log("UV time:" + stopwatch.ElapsedMilliseconds);
      
    
   }
    IEnumerator IEResetMemoryPool(float second)
    {
        yield return new WaitForSeconds(second);
        ResetMemoryPool();
    }
  
}
