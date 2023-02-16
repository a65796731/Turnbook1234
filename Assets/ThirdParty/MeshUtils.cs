using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class MeshUtils
{

    //public MeshFilter mf;
    //void Test()
    //{

    //    string[] file = File.ReadAllLines("C:\\Users\\PDC-48\\Desktop\\TTTest\\Assets\\MeshTest\\meshData.txt");
    //    string[] verts = file[1].Split(',');
    //    string[] tris = file[3].Split(',');
    //    List<Vector3> vertexs = new List<Vector3>();
    //    for (int i = 0; i < verts.Length; i += 3)
    //    {
    //        vertexs.Add(new Vector3(float.Parse(verts[i]), float.Parse(verts[i + 1]), float.Parse(verts[i + 2])));
    //    }
    //    List<int> triangles = new List<int>();
    //    for (int i = 0; i < tris.Length; i++)
    //    {
    //        triangles.Add(int.Parse(tris[i]));
    //    }
    //    Stopwatch sw = new Stopwatch();
    //    sw.Start();
    //    List<int> littlePart = new List<int>();
    //    List<int> bigPart = new List<int>();

    //    ScreenTwoPart(vertexs, triangles, littlePart, bigPart);

    //    VandFupdate(ref vertexs, ref littlePart);

    //    sw.Stop();
    //    UnityEngine.Debug.Log("TIME: " + sw.ElapsedMilliseconds);



    //    Mesh m = new Mesh();
    //    m.vertices = vertexs.ToArray();
    //    m.triangles = littlePart.ToArray();
    //    m.RecalculateNormals();
    //    mf.mesh = m;

    //}

    public static void DepartTwo(List<Vector3> vertexs, List<int> triangles, List<int> littlePart, List<int> bigPart)
    {
        List<List<int>> flags = new List<List<int>>(new List<int>[vertexs.Count]);

        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (flags[triangles[i]] == null)
            {
                flags[triangles[i]] = new List<int>();
            }
            if (flags[triangles[i + 1]] == null)
            {
                flags[triangles[i + 1]] = new List<int>();
            }
            if (flags[triangles[i + 2]] == null)
            {
                flags[triangles[i + 2]] = new List<int>();
            }
            flags[triangles[i]].Add(triangles[i + 1]);
            flags[triangles[i]].Add(triangles[i + 2]);
            flags[triangles[i + 1]].Add(triangles[i]);
            flags[triangles[i + 1]].Add(triangles[i + 2]);
            flags[triangles[i + 2]].Add(triangles[i]);
            flags[triangles[i + 2]].Add(triangles[i + 1]);
        }

        List<int> nums = new List<int>(new int[vertexs.Count]);
        Stack<List<int>> st = new Stack<List<int>>();
        st.Push(flags[0]);
        nums[0] = 1;
        int sum = 1;
        while (st.Count > 0)
        {
            List<int> a = st.Pop();
            for (int i = 0; i < a.Count; i++)
            {
                if (nums[a[i]] == 0)
                {
                    sum++;
                    nums[a[i]] = 1;
                    st.Push(flags[a[i]]);
                }
            }
        }

        int f = sum * 1.0 / vertexs.Count > 0.5 ? 0 : 1;

        for (int i = 0; i < triangles.Count; i += 3)
        {
            if (nums[triangles[i]] == f)
            {
                littlePart.Add(triangles[i]);
                littlePart.Add(triangles[i + 1]);
                littlePart.Add(triangles[i + 2]);
            }
            else
            {
                bigPart.Add(triangles[i]);
                bigPart.Add(triangles[i + 1]);
                bigPart.Add(triangles[i + 2]);
            }
        }
    }

    public static void VandFupdate(ref List<Vector3> vertexs, ref List<int> triangles)
    {
        List<Vector3> newVertex = new List<Vector3>();
        List<int> newTriangle = new List<int>();
        List<int> tabel = new List<int>();
        for (int i = 0; i < vertexs.Count; i++)
        {
            tabel.Add(-1);
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            if (tabel[triangles[i]] == -1)
            {
                tabel[triangles[i]] = newVertex.Count;
                newVertex.Add(vertexs[triangles[i]]);
            }
        }
        for (int i = 0; i < triangles.Count; i++)
        {
            newTriangle.Add(tabel[triangles[i]]);
        }
        vertexs = newVertex;
        triangles = newTriangle;
    }

    public static void ReformMesh(Mesh mesh)
    {
        List<int> lPart = new List<int>();
        List<int> bPart = new List<int>();
        List<Vector3> listV = new List<Vector3>(mesh.vertices);
        DepartTwo(listV, new List<int>(mesh.triangles), lPart, bPart);
        VandFupdate(ref listV, ref bPart);
        mesh.Clear();
        mesh.SetVertices(listV);
        mesh.SetTriangles(bPart, 0);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

    }
}
