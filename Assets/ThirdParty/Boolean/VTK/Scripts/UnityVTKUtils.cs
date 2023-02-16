using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnityVTKUtils
{
    public static void createMeshData(MeshFilter mf, out float[] vtxs, out int[] idxs)
    {
        Mesh m = mf.mesh;
        Matrix4x4 mL2W = mf.transform.localToWorldMatrix;
        int vLen = m.vertices.Length;
        vtxs = new float[vLen * 3];
        idxs = m.triangles;
        //
        for (int v = 0; v < vLen; v++)
        {
            Vector3 vx = mL2W.MultiplyPoint(m.vertices[v]);
            vtxs[v * 3 + 0] = vx.x;
            vtxs[v * 3 + 1] = vx.y;
            vtxs[v * 3 + 2] = vx.z;
        }

    }

    public static Mesh Subtract(MeshFilter meshA, MeshFilter meshB)
    {
        float[] vtxA;
        int[] idxA;
        float[] vtxB;
        int[] idxB;
        createMeshData(meshA, out vtxA, out idxA);
        createMeshData(meshB, out vtxB, out idxB);
        VTKBoolean.SetFirstIndexes(idxA, idxA.Length);
        VTKBoolean.SetSecondIndexes(idxB, idxB.Length);
        VTKBoolean.SetFirstVertex(vtxA, vtxA.Length);
        VTKBoolean.SetSecondVertex(vtxB, vtxB.Length);
        VTKBoolean.SeImageOperationType(3);
        VTKBoolean.Blend();
        int iSize = 0;
        int vSize = 0;
        VTKBoolean.GetIndexesSize(ref iSize);
        VTKBoolean.GetVertexSize(ref vSize);
        if(iSize==0||vSize==0)
        {
            //容错：如果模型没有了，则返回原来的目模型
            Debug.Log("容错：如果模型没有了，则返回原来的目模型:iSize:" + iSize + ",vSize:" + vSize);
            return meshA.mesh;
            //
        }
        int[] resTriangles = new int[iSize];
        double[] resVetexes = new double[vSize];

        IntPtr ptrI = Marshal.AllocHGlobal(sizeof(int) * iSize);
        IntPtr ptrV = Marshal.AllocHGlobal(sizeof(double) * vSize);

        VTKBoolean.GetVertexOutput(ptrV);
        VTKBoolean.GetIndexesOutput(ptrI);
        Marshal.Copy(ptrV, resVetexes, 0, vSize);
        Marshal.Copy(ptrI, resTriangles, 0, iSize);
        return createUnityMesh(resVetexes, resTriangles, meshA.transform.worldToLocalMatrix);


    }

    public static Mesh createUnityMesh(double[] resVetexes, int[] resTriangles, Matrix4x4 mW2L)
    {
        Mesh mesh = new Mesh();
        int len = resVetexes.Length;
        Vector3[] vtxes = new Vector3[len / 3];
        for (int i = 0; i < len; i += 3)
        {
            Vector3 v3 = new Vector3((float)resVetexes[i + 0], (float)resVetexes[i + 1], (float)resVetexes[i + 2]);
            vtxes[i / 3] = mW2L.MultiplyPoint(v3);
        }
        mesh.vertices = vtxes;
        mesh.triangles = resTriangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        return mesh;
    }



}
