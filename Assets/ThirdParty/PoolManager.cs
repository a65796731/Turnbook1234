using Csg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m_fixedPoints1
{
    public int index = 0;
    public Vector3 pos;
    public MeshFilter meshFilter = null;
    public void SetData(int index, Vector3 pos, MeshFilter meshFilter)
    {
        this.index = index;
        this.pos = pos;
        this.meshFilter = meshFilter;
    }
    public m_fixedPoints1()
    {

    }
}

public static class PoolManager 
{

    static int Count = 100000;
    public static ObjPool<m_fixedPoints1> fixedPointsPool = null;
    public static ObjPool<Vector3D> Vector3DPool = null;
    public static ObjPool<Vector3> Vector3Pool = null;
    // Start is called before the first frame update
    static  PoolManager()
    {
        fixedPointsPool = new ObjPool<m_fixedPoints1>();
        Vector3Pool = new ObjPool<Vector3>();
        Vector3DPool = new ObjPool<Vector3D>();
        fixedPointsPool.CreatePool(Count);
        Vector3DPool.CreatePool(Count);
        Vector3Pool.CreatePool(Count);
    }
}
