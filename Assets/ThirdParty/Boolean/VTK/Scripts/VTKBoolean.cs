using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VTKBoolean
{
    [DllImport("ZXFMC.dll")]
    public static extern void Initialize();

    [DllImport("ZXFMC.dll")]
    public static extern void SeImageOperationType(int type); 


    [DllImport("ZXFMC.dll")]
    public static extern void SetFirstVertex(float[] vtx,int vLen);


    [DllImport("ZXFMC.dll")]
    public static extern void SetFirstIndexes(int[] idx, int iLen); 


    [DllImport("ZXFMC.dll")]
    public static extern void SetSecondVertex(float[] vtx, int vLen);


    [DllImport("ZXFMC.dll")]
    public static extern void SetSecondIndexes(int[] idx, int iLen);


    [DllImport("ZXFMC.dll")]
    public static extern long GetVertexSize(ref int rvLen);


    [DllImport("ZXFMC.dll")]
    public static extern long GetVertexOutput(IntPtr rvtx);  

    [DllImport("ZXFMC.dll")]
    public static extern long GetIndexesSize(ref int rvLen);


    [DllImport("ZXFMC.dll")]
    public static extern long GetIndexesOutput(IntPtr ridx); 

    [DllImport("ZXFMC.dll")]
    public static extern long Blend();

    [DllImport("ZXFMC.dll")]
    public static extern void Release();


}
