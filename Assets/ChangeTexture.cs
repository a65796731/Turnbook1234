using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChangeTexture : MonoBehaviour
{
    public bool test;
    public float PixelOffset=1.0f;
    private Texture2D srcTextureClone = null;
    public Texture2D srcTexture = null;
    public Texture2D destTexture = null;
    Material material = null;
    private int width;
    private int height;
    private bool[] HitBuffer;
    private Vector2 initUVLeft;
    private Vector2 initUVRight;
    // Start is called before the first frame update
  public  void init()
    {
        material = GetComponent<MeshRenderer>().material;
        if (srcTexture == null|| destTexture==null|| material==null)
        {
            Debug.LogError("Material or Texture is null");
            return;
        }

        srcTextureClone = CreateTextureByRawData(srcTexture);
        material.SetTexture("_MainTex", srcTextureClone);

        //    destTexture = CreateTextureByRawData(destTexture);
        width = srcTextureClone.width;
        height = srcTextureClone.height;
        HitBuffer = new bool[width * height];
        for (int i = 0; i < HitBuffer.Length; i++)
        {
            HitBuffer[i] = false;
        }

        if (test)
        {
          
            destTexture = new Texture2D(width, height, srcTextureClone.format, true);
           
            //for (int x=0;x<width;x++)
            //{
            //    for (int y = 0; y < height; y++)
            //    {
            //        destTexture.SetPixel(x, y, Color.red);
            //    }
            //}
        }
        //    destTexture.Apply();

        Test();
    }
    public void Test()
    {
        Vector3 initUVLeft = new Vector3(0.45f, 0.45f, 0.0f);
        Vector3 initUVRight = new Vector3(0.55f, 0.45f, 0.0f);
        Vector3 inUVLeft = new Vector3(0.45f, 0.55f, 0.0f);
        Vector3  inUVRight = new Vector3(0.55f, 0.55f, 0.0f);

        float dis = (initUVLeft - inUVLeft).magnitude;
        Vector3 right = (initUVRight - initUVLeft).normalized;
        Vector3 up = Vector3.Cross(Vector3.forward.normalized, right);
        Vector3 finalUVLeft = initUVLeft + new Vector3(up.x, up.y) * dis;
      
    }
    public bool isHit(Vector2 hitUV)
    {
       
        int x = Mathf.RoundToInt(hitUV.x * width);
        int y = Mathf.RoundToInt(hitUV.y * height);
        //int Num = 0;
        //int total = width * height;
        //int hitIndex = x + y * width;
        //int rightIndex = hitIndex + 1>= total? hitIndex : hitIndex + 1;
        //int leftIndex = hitIndex - 1 < 0 ? hitIndex : hitIndex - 1;
        //int topIndex = hitIndex + width  >= total ? hitIndex : hitIndex + width ;
        //int rightTopIndex =  hitIndex + width + 1 >= total ? hitIndex : hitIndex + width + 1;
        //int leftTopIndex = hitIndex + width - 1 >= total ? hitIndex : hitIndex + width - 1;
        //int ButtomIndex = hitIndex - width < 0 ? hitIndex : hitIndex - width;
        //int rightButtomIndex = hitIndex - width + 1 < 0 ? hitIndex : hitIndex - width + 1;
        //int leftButtomIndex = hitIndex - width - 1 <0 ? hitIndex : hitIndex - width - 1;
        //Num += HitBuffer[hitIndex] ? 1 : 0;
        //Num += HitBuffer[rightIndex ] ? 1 : 0;
        //Num += HitBuffer[leftIndex ] ? 1 : 0;
        //Num += HitBuffer[topIndex] ? 1 : 0;
        //Num += HitBuffer[leftTopIndex] ? 1 : 0;
        //Num += HitBuffer[ButtomIndex] ? 1 : 0;
        //Num += HitBuffer[rightTopIndex] ? 1 : 0;
        //Num += HitBuffer[rightButtomIndex] ? 1 : 0;
        //Num += HitBuffer[leftButtomIndex] ? 1 : 0;
        //判断周围9个点
        return HitBuffer[x + y * width];
    }
    public void recordInitUV(Vector2 uvLeft,Vector2 uvRight)
    {
        initUVLeft = uvLeft;
        initUVRight = uvRight;
    }
    int n = 0;
    public void correctionUV(Vector2 inLastUVLeft, Vector2 inLastUVRight, Vector2 inCurUVLeft, Vector2 inCurUVRight,out Vector2 outUVLeft, out Vector2 outUVRight)
    {
        
        Vector3 right= (inLastUVRight - inLastUVLeft).normalized;
        Vector3 up = Vector3.Cross( right, Vector3.forward.normalized);
        float distanceL=  Vector3.Distance(inCurUVLeft, inLastUVLeft) *Vector3.Dot((inCurUVLeft - inLastUVLeft).normalized, up);
        float distanceR = Vector3.Distance(inCurUVRight, inLastUVRight) * Vector3.Dot((inCurUVRight - inLastUVRight).normalized, up);
        Debug.Log("L:" + distanceL + "R:" + distanceR +"  Dir:"+ (inCurUVLeft - initUVLeft).normalized+" Dir1 :"+ up);
        outUVLeft = inLastUVLeft + new Vector2(up.x, up.y) * distanceL;
        outUVRight = inLastUVRight + new Vector2(up.x, up.y) * distanceR;

 
    }
    public void TestDraw(Vector2 uv,Color COLOR)
    {
        int x = (int)(uv.x * width);
        int y = (int)(uv.y * width);
        srcTextureClone.SetPixel(x,y, COLOR);
        //srcTextureClone.SetPixel(x+1, y, COLOR);
        //srcTextureClone.SetPixel(x + 1, y+1, COLOR);
        //srcTextureClone.SetPixel(x + 1, y - 1, COLOR);
        //srcTextureClone.SetPixel(x - 1, y, COLOR);
        //srcTextureClone.SetPixel(x - 1, y + 1, COLOR);
        //srcTextureClone.SetPixel(x - 1, y - 1, COLOR);
        //srcTextureClone.SetPixel(x  , y + 1, COLOR);
        //srcTextureClone.SetPixel(x , y - 1, COLOR);
        srcTextureClone.Apply();
    }
    public   void UpdateTexture(Vector2[] uvs)
    {
       
        //Debug.Log(" lastL" + uvs[0] + "lastR" + uvs[1]+ " curL" + uvs[2] + "curR" + uvs[3]);
        //Debug.LogFormat("LastL: {0,1}", uvs[0].x, uvs[0].y);
        Vector2Int minBox;
        Vector2Int maxBox;
        Vector2[] texels;
        ToTextureSpacePos(uvs,out texels);
        GetAABBBox(texels, out minBox, out maxBox);

        Vector3 texelPos0 = texels[0];
        Vector3 texelPos1 = texels[1];
        Vector3 texelPos2 = texels[2];
        Vector3 texelPos3 = texels[3];
        for (int y= minBox.y; y< maxBox.y; y++)
        {
            for (int x = minBox.x; x < maxBox.x; x++)
            {
                (float A, float B, float G) = Barycentric(new Vector3(x, y, 0), texelPos0, texelPos1, texelPos3);
                if (A < 0 || B < 0 || G < 0)
                {

                    (A, B, G) = Barycentric(new Vector3(x, y, 0), texelPos3, texelPos2, texelPos0);
                    if (A < 0 || B < 0 || G < 0)
                    {
                        continue;
                    }

                }

                srcTextureClone.SetPixel(x, y, Color.red);
                HitBuffer[x + y * width]=true;

            }
        }
        srcTextureClone.Apply();
   
    }
    
    void   ToTextureSpacePos(Vector2[] uvs, out Vector2[] texels)
    {
        texels = new Vector2[uvs.Length];
        for(int i=0;i< uvs.Length;i++)
        {
            
            texels[i] =new Vector2( (uvs[i].x * width), uvs[i].y * height);
       
        }
    
        Vector2 offset0=(texels[0] - texels[1]).normalized;
        Vector2 offset1 = (texels[1] - texels[0]).normalized;
        Vector2 offset2 = (texels[2] - texels[3]).normalized;
        Vector2 offset3 = (texels[3] - texels[2]).normalized;
        texels[0] = texels[0] + offset0 * PixelOffset;
        texels[1] = texels[1] + offset1 * PixelOffset;
        texels[2] = texels[2] + offset2 * PixelOffset;
        texels[3] = texels[3] + offset3 * PixelOffset;
    }
    private void GetAABBBox(Vector2[] v, out Vector2Int minBox, out Vector2Int maxBox)
    {

        Vector2 maxBoxTemp = new Vector2(float.MinValue, float.MinValue);
        Vector2 minBoxTemp = new Vector2(float.MaxValue, float.MaxValue);
        for (int i = 0; i < v.Length; i++)
        {
            if (minBoxTemp.x > v[i].x)
                minBoxTemp.x = v[i].x;

            if (minBoxTemp.y > v[i].y)
                minBoxTemp.y = v[i].y;


            if (maxBoxTemp.x < v[i].x)
                maxBoxTemp.x = v[i].x;

            if (maxBoxTemp.y < v[i].y)
                maxBoxTemp.y = v[i].y;



        }
        maxBox = new Vector2Int(Mathf.CeilToInt(maxBoxTemp.x), Mathf.CeilToInt(maxBoxTemp.y));
        minBox = new Vector2Int(Mathf.FloorToInt(minBoxTemp.x), Mathf.FloorToInt(minBoxTemp.y));
        //maxBox = new Vector2Int((int)maxBoxTemp.x, (int)(maxBoxTemp.y));
        //minBox = new Vector2Int((int)(minBoxTemp.x), (int)(minBoxTemp.y));
    }
    private (float, float, float) Barycentric(Vector3 p, Vector3 p1, Vector3 p2, Vector3 p3)
    {

        float a = ((p.y - p3.y) * (p2.x - p3.x) + (p2.y - p3.y) * (p3.x - p.x)) / ((p1.y - p3.y) * (p2.x - p3.x) + (p2.y - p3.y) * (p3.x - p1.x));
        float b = ((p.y - p1.y) * (p3.x - p1.x) + (p3.y - p1.y) * (p1.x - p.x)) / ((p1.y - p3.y) * (p2.x - p3.x) + (p2.y - p3.y) * (p3.x - p1.x));
        float g = ((p.y - p2.y) * (p1.x - p2.x) + (p1.y - p2.y) * (p2.x - p.x)) / ((p1.y - p3.y) * (p2.x - p3.x) + (p2.y - p3.y) * (p3.x - p1.x));
        return (a, b, g);
    }
    private Texture2D CreateTextureByRawData(Texture2D sourceTexture)
    {
        if (sourceTexture != null)
        {
            Texture2D newTextureByRawData = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, false);

            // 对于运行时纹理生成，也可以通过GetRawTextureData直接写入纹理数据，返回一个Unity.Collections.NativeArray
            // 这可以更快，因为它避免了 LoadRawTextureData 会执行的内存复制。
            newTextureByRawData.LoadRawTextureData(sourceTexture.GetRawTextureData());
            newTextureByRawData.Apply();
            return newTextureByRawData;
        }
        else
        {
            Debug.LogWarning("Texture is null");
            return null;
        }
    }
   public Texture2D getTexture()
    {
        return srcTexture;
    }

}
