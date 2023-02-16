
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public class MeshInfo
{
    public int subVerticesCount;
    public int subTrianglesCount;
    public List<Vector3> subMeshCenterPoints = new List<Vector3>();
    public List<List<Vector3>> subVertices = new List<List<Vector3>>();
    public List<List<int>> subTriangles = new List<List<int>>();
    public List<List<Vector2>> subUvs = new List<List<Vector2>>();
    public List<Color[]> subColors = new List<Color[]>();

    public List<Vector3> vertices;
    public List<int> triangles;
    public List<int> flags;
    public Vector3[] normals;
    public List<Vector2> uv;
    public Color[] colors;
    public Transform tran;
    public Transform cutter;
    public Matrix4x4 mat;
}
public class CSGBoolean
{
    static Vector3 _vxTv3(Csg.Vertex v)
    {
        return new Vector3((float)v.Pos.X, (float)v.Pos.Y, (float)v.Pos.Z);
    }

    static Csg.Solid m_solidB, m_solidA;
    static Csg.Tree tree;
    /// <summary>
    /// 执行剪切
    /// </summary>
    /// <param name="meshA">被剪</param>
    /// <param name="meshB">剪刀</param>
    /// <returns></returns>
    //public static   async Task<(MeshInfo, MeshInfo)>  Difference(MeshFilter meshA, MeshFilter meshB, Action<List<Vector3>, List<int>, Matrix4x4> _onChipMesh = null)
    //{

    //    //m_solidA = _createSolid(meshA.sharedMesh, meshA.transform.localToWorldMatrix);

    //    //m_solidB = _createSolid(meshB.sharedMesh, meshB.transform.localToWorldMatrix);

    //    //(Csg.Solid m_solidC, Csg.Solid m_solidD) res = await  m_solidA.Substract(m_solidB);


    //    //MeshInfo meshC = _createMesh(res.m_solidC, meshA.tran.worldToLocalMatrix, false);
    //    //MeshInfo meshD = _createMesh(res.m_solidD, meshA.tran.worldToLocalMatrix, true);
    //    //// 

    //    //return (meshC, meshD);
    //    return null;
    //}
    public static  (bool, MeshInfo) Intersect(MeshInfo meshA, MeshInfo meshB, Action<List<Vector3>, List<int>, Matrix4x4> _onChipMesh = null)
    {
        bool IsSuccess = true;
        m_solidA = _createSolid(meshA);

        m_solidB = _createSolid(meshB);

        Csg.Solid  res =  m_solidA.Intersect(m_solidB);
        MeshInfo meshC = null;

        //try
        //{
      Vector3  pos = CalAABBCenter(res);
        /// GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject cube = new GameObject();
        cube.transform.position = pos;
        cube.transform.rotation = meshB.tran.rotation;
        Matrix4x4 w2l=  Matrix4x4.TRS(pos, meshB.tran.rotation, Vector3.one).inverse;
        meshC = _createMesh(res, w2l, false);
        Color[] colors = new Color[meshC.vertices.Count];
        for (int i = 0; i < meshC.vertices.Count; i++)
        {
            colors[i] = Color.white;

        }
        meshC.colors = colors;
        meshC.tran = cube.transform;
        // meshD = _createMesh(res.m_solidD, meshA.tran.worldToLocalMatrix, false);
        //}
        //catch(Exception ex)
        //{
        //    IsSuccess = false;
        //}
        // 
        //   MergeMesh(ref meshA, meshC, meshD);
        return (IsSuccess, meshC);
    }
    
    public static (bool, MeshInfo) Difference(MeshInfo meshA, MeshInfo meshB, Action<List<Vector3>, List<int>, Matrix4x4> _onChipMesh = null)
    {
        bool IsSuccess = true;
        m_solidA = _createSolid(meshA);

        m_solidB = _createSolid(meshB);
        
        (Csg.Solid m_solidC, Csg.Solid m_solidD) res =  m_solidA.Substract(m_solidB);
        MeshInfo meshC = null;
        MeshInfo meshD = null;
        //try
        //{
             meshC = _createMesh(res.m_solidC, meshA.tran.worldToLocalMatrix, false);
             meshD = _createMesh(res.m_solidD, meshA.tran.worldToLocalMatrix, false);
        //}
        //catch(Exception ex)
        //{
        //    IsSuccess = false;
        //}
        // 
        MergeMesh(ref meshA, meshC, meshD);
        return (IsSuccess, meshA);
    }
    private static (bool, MeshInfo) SubMeshDifference(MeshInfo meshA,MeshInfo meshB, Action<List<Vector3>, List<int>, Matrix4x4> _onChipMesh = null)
    {
       
        m_solidA = _createSolid(meshA);

        m_solidB = _createSolid(meshB);

        (Csg.Solid m_solidC, Csg.Solid m_solidD) res =  m_solidA.Substract(m_solidB);
        MeshInfo meshC = null;
        MeshInfo meshD = null;
        //try
        //{
        meshC = _createMesh(res.m_solidC, meshA.tran.worldToLocalMatrix, false);
        meshD = _createMesh(res.m_solidD, meshA.tran.worldToLocalMatrix, false);
        //}
        //catch(Exception ex)
        //{
        //    IsSuccess = false;
        //}
        // 
        MergeMesh(ref meshA, meshC, meshD);
        return (false, meshA);
    }
    //static   MeshInfo MergeMesh(MeshInfo meshA,MeshInfo meshB)
    //   {
    //       MeshInfo meshInfo = new MeshInfo();
    //       List<Vector3> newVertices = new List<Vector3>(meshA.vertices.Count+ meshB.vertices.Count);
    //       newVertices.AddRange(meshA.vertices);
    //       newVertices.AddRange(meshB.vertices);
    //       List<int> newTriangles = new List<int>(meshA.triangles.Count + meshB.triangles.Count);
    //       int startIndex = meshA.vertices.Count;
    //      for (int i=0;i< meshB.triangles.Count;i++)
    //       {
    //           meshB.triangles[i] = startIndex + meshB.triangles[i];
    //       }
    //       newTriangles.AddRange(meshA.triangles);
    //       newTriangles.AddRange(meshB.triangles);
    //       List<int> newFlags = new List<int>(meshA.flags.Count + meshB.flags.Count);
    //       newFlags.AddRange(meshA.flags);
    //       newFlags.AddRange(meshB.flags);
    //       List<Vector2> uv = new List<Vector2>(meshA.uv.Count + meshB.uv.Count);
    //       uv.AddRange(meshA.uv);
    //        uv.AddRange(meshB.uv);
    //       Color[] colors = new Color[newVertices.Count];
    //       for(int i=0;i< meshA.vertices.Count;i++)
    //       {
    //           //colors[i] = meshA.flags[i] == 2 ? Color.black : Color.white;

    //         if (meshA.flags[i]==2)
    //           {
    //               colors[i] = Color.black;
    //           }
    //         else
    //           {
    //               colors[i] = Color.white;
    //           }

    //       }
    //       for (int i =  meshA.vertices.Count; i < newVertices.Count; i++)
    //       {
    //           colors[i] = Color.black;
    //       }
    //       //for (int i =0; i < newFlags.Count; i++)
    //       //{
    //       //  if(newFlags[i]==1)
    //       //    {
    //       //        Debug.Log(" UV :" + uv[i].x + " ," + uv[i].y);
    //       //    }
    //       //}
    //       meshInfo.vertices = newVertices;
    //       meshInfo.triangles = newTriangles;
    //       meshInfo.flags = newFlags;
    //       meshInfo.tran = meshA.tran;
    //       meshInfo.uv = uv;
    //       meshInfo.colors = colors;
    //       return meshInfo;
    //    //   Array.Copy(meshA.vertices,0, newVertices,0, meshA.vertices.Count);
    //   }

    static void MergeMesh( ref MeshInfo oriMesh,MeshInfo meshA, MeshInfo meshB)
    {

        oriMesh.subTrianglesCount += meshB.triangles.Count;
        oriMesh.subVerticesCount += meshB.vertices.Count;
      
     
   
        Color[] colors = new Color[meshA.vertices.Count];
        for (int i = 0; i < meshA.vertices.Count; i++)
        {
                colors[i] = Color.white;
            
        }
        Color[] colors_B = new Color[meshB.vertices.Count];
        Vector3 totalVec = Vector3.zero;
        for (int i = 0; i < meshB.vertices.Count; i++)
        {
            colors_B[i] = Color.black;
            totalVec += meshB.vertices[i];
        }
        oriMesh.vertices = meshA.vertices;
        oriMesh.triangles = meshA.triangles;
        oriMesh.uv = meshA.uv;
        oriMesh.colors = colors;

        oriMesh.subVertices.Add(meshB.vertices);
        oriMesh.subTriangles.Add(meshB.triangles);
        oriMesh.subUvs.Add(meshB.uv);
        oriMesh.subColors.Add(colors_B);
   
        oriMesh.subMeshCenterPoints.Add(totalVec / meshB.vertices.Count);
        int startIndex = meshA.vertices.Count;
        for (int i = 0; i < oriMesh.subTriangles.Count; i++)
        {
            List<int> subTriList = oriMesh.subTriangles[i];
            for (int j = 0; j < subTriList.Count; j++)
            {
                subTriList[j] = startIndex + subTriList[j];
            }
            startIndex += oriMesh.subVertices[i].Count;
        }
        //   Array.Copy(meshA.vertices,0, newVertices,0, meshA.vertices.Count);
    }
    public static Csg.Solid _createSolid( MeshInfo meshInfo)
    {
        Matrix4x4 l2w = meshInfo.tran. localToWorldMatrix;
        int vLen = meshInfo.vertices.Count;
        int iLen = meshInfo.triangles.Count;
        List<Vector3> vertices = meshInfo.vertices;
        List<int> triangles = meshInfo.triangles;
        List<int> flags = meshInfo.flags;
      List<Vector2> uv = meshInfo.uv;
        Csg.Vector2D zero = new Csg.Vector2D();
        // List<Csg.Polygon> ps = new List<Csg.Polygon>();
        List<Csg.Polygon> ps = new List<Csg.Polygon>();
        Csg.Polygon[] psArr = new Csg.Polygon[iLen / 3];
        // Vector3 []worldPoints=new Vector3[iLen]

        DateTime dateTime = DateTime.Now;
        Parallel.For(0, iLen, (vi) =>
        {
            if (vi % 3 == 0)
            {
                int flag = flags[triangles[vi + 0]];
                Vector3 vw0 = l2w.MultiplyPoint(vertices[triangles[vi + 0]]);
                Vector3 vw1 = l2w.MultiplyPoint(vertices[triangles[vi + 1]]);
                Vector3 vw2 = l2w.MultiplyPoint(vertices[triangles[vi + 2]]);
                Vector2 uv0 = uv[triangles[vi + 0]];
                Vector2 uv1 = uv[triangles[vi + 1]];
                Vector2 uv2 = uv[triangles[vi + 2]];
                //Vector3 vw0 = (vertices[triangles[vi + 0]]);
                //Vector3 vw1 =(vertices[triangles[vi + 1]]);
                //Vector3 vw2 = (vertices[triangles[vi + 2]]);
                Csg.Vertex[] cvx = new Csg.Vertex[3];
                //    vertex= Pool.Inst.VertexPool.New();
                Csg.Vector3D vector3D = new Csg.Vector3D();
                Csg.Vector2D uvtemp = new Csg.Vector2D();
                uvtemp.X = uv0.x;
                uvtemp.Y = uv0.y;
                vector3D.X = vw0.x;
                vector3D.Y = vw0.y;
                vector3D.Z = vw0.z;
              
                cvx[0] = new Csg.Vertex(vector3D, uvtemp, zero);
                vector3D = new Csg.Vector3D();
                uvtemp = new Csg.Vector2D();
                uvtemp.X = uv1.x;
                uvtemp.Y = uv1.y;
                vector3D.X = vw1.x;
                vector3D.Y = vw1.y;
                vector3D.Z = vw1.z;
                cvx[1] = new Csg.Vertex(vector3D, uvtemp,zero);
                vector3D = new Csg.Vector3D();
                uvtemp = new Csg.Vector2D();
                uvtemp.X = uv2.x;
                uvtemp.Y = uv2.y;
                vector3D.X = vw2.x;
                vector3D.Y = vw2.y;
                vector3D.Z = vw2.z;
                cvx[2] = new Csg.Vertex(vector3D, uvtemp, zero);
                psArr[vi / 3] = new Csg.Polygon(flag, cvx);
              
            }
        });
        ps.Clear();
        ps.AddRange(psArr);

        Csg.Solid s = Csg.Solid.FromPolygons(ps);

        return s;

    }
  
    
   
    static Csg.Solid _updateSolid(Csg.Solid sd,Mesh mf, Matrix4x4 localToWorldMatrix)
    {
        Matrix4x4 l2w = localToWorldMatrix;
        int vLen = mf.vertices.Length;
        int iLen = mf.triangles.Length;
        Vector3[] vertices = mf.vertices;
        int[] triangles = mf.triangles;
        Parallel.For(0, sd.Polygons.Count, (vi) =>
        {
            for (int i = 0; i < sd.Polygons.Count; i++)
            {
                Vector3 vw0 = l2w.MultiplyPoint(vertices[triangles[vi + 0]]);
                Vector3 vw1 = l2w.MultiplyPoint(vertices[triangles[vi + 1]]);
                Vector3 vw2 = l2w.MultiplyPoint(vertices[triangles[vi + 2]]);
                //sd.Polygons[i].Vertices[0].Pos.X = vw0.x;
                //sd.Polygons[i].Vertices[0].Pos.Y = vw0.y;
                //sd.Polygons[i].Vertices[0].Pos.Z = vw0.z;

                //sd.Polygons[i].Vertices[1].Pos.X = vw1.x;
                //sd.Polygons[i].Vertices[1].Pos.Y = vw1.y;
                //sd.Polygons[i].Vertices[1].Pos.Z = vw1.z;

                //sd.Polygons[i].Vertices[2].Pos.X = vw2.x;
                //sd.Polygons[i].Vertices[2].Pos.Y = vw2.y;
                //sd.Polygons[i].Vertices[2].Pos.Z = vw2.z;
 
            }
         });
        return sd;
     }
    public static Vector3 CalAABBCenter(Csg.Solid solid)
    {
     var ps=   solid.Polygons;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue,float.MinValue,float.MinValue);
        for (int j = 0; j < ps.Count; j++)
        {

            List< Csg.Vertex > vectors = ps[j].Vertices;

            for (int i = 0; i < vectors.Count; i++)
            {
                if (min.x > vectors[i].Pos.X)
                    min.x = (float)vectors[i].Pos.X;
                if (min.y > vectors[i].Pos.Y)
                    min.y = (float)vectors[i].Pos.Y;
                if (min.z > vectors[i].Pos.Z)
                    min.z = (float)vectors[i].Pos.Z;

                if (max.x < vectors[i].Pos.X)
                    max.x = (float)vectors[i].Pos.X;
                if (max.y < vectors[i].Pos.Y)
                    max.y = (float)vectors[i].Pos.Y;
                if (max.z < vectors[i].Pos.Z)
                    max.z = (float)vectors[i].Pos.Z;
            }
        }

        Vector3 Center = Vector3.zero;
        Center.x= min.x+(max.x - min.x)*0.5f;
        Center.y = min.y+(max.y - min.y) * 0.5f;
        Center.z = min.z + (max.z - min.z) * 0.5f;
     
     
    return Center;
    }
    public  static MeshInfo  _createMesh(Csg.Solid solid, Matrix4x4 w2l, bool IsOffset)
    {

   
     //   Mesh resultMesh = new Mesh();
        List<Vector3> vectors = null;
        List<int> indexes = null;
        List<int> flags = null;
        List<Vector2> uv = null;
     
        var temp= _createMesh(solid, w2l);
     
        vectors = temp.vertices;
        indexes = temp.triangles;
        flags = temp.flags;
        uv = temp.uv;
       
        if (IsOffset)
        {
            for (int i = 0; i < vectors.Count; i++)
            {

                vectors[i] = new Vector3(vectors[i].x, vectors[i].y + 0.000001f, vectors[i].z);
            }
        }
        MeshInfo meshInfo = new MeshInfo();
        meshInfo.vertices = vectors;
        meshInfo.triangles = indexes;
        meshInfo.flags = flags;
        meshInfo.uv = uv;
        //resultMesh.SetVertices(vectors);
        //resultMesh.SetTriangles(indexes, 0);
        //resultMesh.RecalculateNormals();
        //resultMesh.RecalculateTangents();
        //resultMesh.RecalculateBounds();
        return meshInfo;
    }

    static (List<Vector3> vertices, List<int> triangles,List<int> flags,List<Vector2> uv) _createMesh(Csg.Solid solid, Matrix4x4 w2l)
    {

     PoolManager.Vector3Pool.ResetPool();
        List<Csg.Polygon> ps = solid.Polygons;
        int cnt = ps.Count;
        List<Vector3> vectors = new List<Vector3>();
  
        List<int> indexes = new List<int>();
        List<int> flag = new List<int>();
        List<Vector2> uV = new List<Vector2>();
        for (int i = 0; i < cnt; i++)
        {

            int cntV = ps[i].Vertices.Count; 
            if (cntV > 3)
            {
                for (int s = 1; s < cntV - 1; s++)
                {
                    _createTriaVI(w2l, ps[i].Flags,ps[i].Vertices[0], ref vectors, ref indexes,ref flag,ref uV);
                    _createTriaVI(w2l, ps[i].Flags,ps[i].Vertices[s], ref vectors, ref indexes, ref flag,ref uV);
                    _createTriaVI(w2l, ps[i].Flags,ps[i].Vertices[s + 1], ref vectors, ref indexes, ref flag,ref uV);
                }

            }
            else
            {
                for (int j = 0; j < cntV; j++)
                {
                    _createTriaVI(w2l, ps[i].Flags, ps[i].Vertices[j], ref vectors, ref indexes, ref flag,ref uV);
                }
            }
        }

        //整理模型，去除多余的点和孤岛
        List<int> lPart = new List<int>();
        List<int> bPart = new List<int>();
        MeshUtils.DepartTwo(vectors, indexes, lPart, bPart);
       MeshUtils.VandFupdate(ref vectors, ref bPart);
        indexes = bPart;

       
        //
     //   var data = new KeyValuePair<List<Vector3>, List<int>>(vectors, indexes);
        return (vectors, indexes, flag, uV);

    }
    static private void _createTriaVI(Matrix4x4 w2l,int flag, Csg.Vertex vtx, ref List<Vector3> vectors, ref List<int> indexes,ref List<int> flags,ref List<Vector2> uv)
    {
        Vector3 v3= PoolManager.Vector3Pool.New();
        v3.x = (float)vtx.Pos.X;
        v3.y = (float)vtx.Pos.Y;
        v3.z = (float)vtx.Pos.Z;
        
        v3 = w2l.MultiplyPoint(v3);
        int idx = vectors.IndexOf(v3);
        if (idx == -1)
        {
            idx = vectors.Count;
            vectors.Add(v3);
            Vector2 newUV = new Vector2((float)vtx.Uv.X, (float)vtx.Uv.Y);
            uv.Add(newUV);
            flags.Add(flag);
        }
        indexes.Add(idx);
    }


}

