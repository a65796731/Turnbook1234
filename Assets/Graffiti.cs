using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graffiti : MonoBehaviour
{
    private Vector3[] vertexs = null;
    public Transform RayPoint;
    public Texture2D texture2D = null;
    public Texture2D brushTex = null;   
    public int width;
    public int height;
    public int blockWidth=8;
    public Color color = Color.red;
    private Color[] ClearColor;
    Vector2Int curUV;
    Vector2Int oldUV;
    // Start is called before the first frame update
    void Start()
    {
        Color initColor = Color.white;
        texture2D = new Texture2D(width, height);
        ClearColor = new Color[width * height];
        int index = 0;
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                ClearColor[index++] = initColor;
                texture2D.SetPixel(x, y, initColor);
            }
        }
        texture2D.Apply();
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture2D);
        vertexs = GetComponent<MeshFilter>().mesh.vertices;
    }
    //private void Update()
    //{
    //    Raycast();
    //}
    bool first=true;
    public void reset()
    {
        first = true;
        texture2D.SetPixels(ClearColor);
        texture2D.Apply();
    }
    public void Raycast()
    {
        return;
        RaycastHit hitInfo;
       
        Vector3 collPoint;
        if (!Coll(out collPoint))
            return;
        bool IsHit = false;
       Vector3 rayDir= (collPoint - RayPoint.position).normalized;
        IsHit= Physics.Raycast(RayPoint.position, rayDir, out hitInfo, 10f);
       
        Debug.DrawLine(RayPoint.position, RayPoint.position + rayDir, Color.red);
        if (IsHit)
        {
            Debug.Log(hitInfo.transform.name + "UV :" + hitInfo.textureCoord);
             
            int x = (int)(hitInfo.textureCoord.x * width);
            int y = (int)(hitInfo.textureCoord.y * height);
           
            curUV = new Vector2Int(x, y);
            //if (first)
            //{
            //    first = false;
            //    oldUV = curUV;
            //}
            //if ((curUV - oldUV).magnitude > blockWidth * 0.5f)
            //{
               

            //    float lerpNum = 0;

            //    float interval = 1 / (Mathf.Max(Mathf.Abs(curUV.x - oldUV.x), Mathf.Abs(curUV.y - oldUV.y)) / (blockWidth / 0.25f));

            //    while (lerpNum <= 1)

            //    {

            //        lerpNum += interval;

            //       Vector2 catchPos = Vector2.Lerp(oldUV, curUV, InterpolationCalculation(lerpNum));

            //        Draw((int)catchPos.x, (int)catchPos.y, blockWidth, blockWidth);

            //    }
            //    Draw(x, y, blockWidth, blockWidth);

            //}
            //else
            //{
               
            //    Draw(x, y, blockWidth, blockWidth);
            //}
            Draw(x, y, blockWidth, blockWidth);
            oldUV = curUV;
        }
    }
    void Draw(int x, int y, int width,int height)
    {
        Color[] colors = texture2D.GetPixels(x, y, width, height);
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        texture2D.SetPixels(x, y, width, height, colors);
        texture2D.Apply();
    }
    float InterpolationCalculation(float num)
    {

        return 3 * Mathf.Pow(num, 2) - 2 * Mathf.Pow(num, 3);

    }
    private bool Coll(out Vector3 collPoint)
    {
       Matrix4x4 l2w=  transform.localToWorldMatrix;
     // Vector3 localPoint=   l2w.MultiplyPoint(RayPoint.position);
        for (int i=0;i< vertexs.Length;i++)
        {
           Vector3 worldPoint=   l2w.MultiplyPoint(vertexs[i]);
            if ((RayPoint.position - worldPoint).magnitude<=0.2f)
            {
                Debug.Log("Åö×²³É¹¦");
                collPoint = worldPoint;
                return true;
            }
        }
        collPoint= Vector3.zero;
        return false;
    }
    
}
