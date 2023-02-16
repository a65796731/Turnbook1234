using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct DamagedtissueData
{
    
  //  public float dis;
    public float speed;
    public float radius;
    public float interval;
    public bool IsmoreThanHalfCir;
    public bool IsDrawDebugPoints;
    public Material material;
   
}


public class RayTest : MonoBehaviour
{
    [Tooltip("大于移动距离,才移动")]
    public float MoveDistance=0.003f;
    [Range(0,1)]
    public float angleLimit;
    public float pointOffset=0f;
    public float f;
    public float rayLength;
    public Transform LeftPoint;
    public Transform RightPoint;
    public ChangeTexture changeTexture = null;
    List<Vector3> rayPoints=new List<Vector3>();
  

   private GenratePlaneTest CurGenratePlaneTest;
  
   public DamagedtissueData damagedtissueData;
    List<GenratePlaneTest> DamagedtissueList = new List<GenratePlaneTest>();

   
    int HitNum;
   // Vector3[] pointQueue = new Vector3[6];
    Vector3[] everyFrameLeftPoint = new Vector3[5];//记录最新5帧左边点的位置;
    Vector3[] everyFrameRightPoint = new Vector3[5];//记录最新5帧右边点的位置;
    Vector3[] PointsArr= new Vector3[4];
    Vector2 lastLeftUV;
    Vector2 lastRightUV;
    List<Vector3> pointsL = new List<Vector3>();
    List<Vector3> pointsR = new List<Vector3>();
    void Start()
    {
        GenrateRay();
        Reset();
        changeTexture.init();
    }

    List<RaycastHit> raycastsList = new List<RaycastHit>();
    List<Vector3> vertexsList = new List<Vector3>();

   
    void Update()
    {
        GenrateRay();
        CutButton();
    }
    private  void GenrateRay()
    {
        rayPoints.Clear();
        float dis = (LeftPoint.position - RightPoint.position).magnitude;
        Vector3 forward = LeftPoint.forward.normalized;
        float offset= dis / f;
        for (float i = 0; i < dis; i += offset)
        {
            Vector3 rayPoint = Vector3.Lerp(LeftPoint.position, RightPoint.position, i/ dis);
            rayPoints.Add(rayPoint);
        
        }
       
        for (int i = 0; i < rayPoints.Count; i++)
        {
         Debug.DrawLine(rayPoints[i], rayPoints[i] + forward * rayLength, Color.red);
        }
    }
    public bool ON;
    public Vector3 lastPos;

    //public bool CutButton()
    //{
     
       
    //    if (!CollsionTest.collsion && !ON)
    //    {
    //        Reset();
    //        return false;
    //    }
    //    raycastsList.Clear();
    //    vertexsList.Clear();
    //    int layerMask = 0;
    //    layerMask |= 1 << LayerMask.NameToLayer("hotSpot");
    //    Vector3 forward = LeftPoint.forward.normalized;
    //    for (int j = 0; j < rayPoints.Count; j++)
    //    {

    //        Vector3 from = rayPoints[j];
    //        Vector3 to = rayPoints[j] + forward * rayLength;


    //        RaycastHit[] raycastHits = new RaycastHit[1];

    //        int hitCount = Physics.RaycastNonAlloc(from, forward, raycastHits, rayLength, layerMask);
    //        if (hitCount > 0)
    //        {
    //            raycastsList.Add(raycastHits[0]);
    //            vertexsList.Add(from);

    //        }

    //    }
    //    if (raycastsList.Count == 0)
    //    {
    //        Reset();
    //        return false;

    //    }
      
        
    //    Vector3 minPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    //    Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);
     
    //    Vector2 minUV = Vector3.zero; 
    //    Vector2 maxUV=Vector3.zero;
    //    Vector3 normal=Vector3.zero;
    //    for (int i = 0; i < raycastsList.Count; i++)
    //    {
    //        if (changeTexture.isHit(raycastsList[i].textureCoord))
    //            continue;

    //        Vector3 pointL = transform.worldToLocalMatrix.MultiplyPoint(raycastsList[i].point);
    //        if (pointL.x < minPoint.x)
    //        {
    //            minPoint = pointL;
    //            minUV = raycastsList[i].textureCoord;
    //        }

    //        if (pointL.x > maxPoint.x)
    //        {
    //            maxPoint = pointL;
    //            maxUV = raycastsList[i].textureCoord;
    //            normal = raycastsList[i].normal;
    //        }

         
            

    //    }
    //    if (maxPoint.x == float.MinValue || minPoint.x == float.MaxValue || minPoint.x == maxPoint.x)
    //    {
    //        if(CurGenratePlaneTest!=null)
    //        {
    //            Reset();
    //        }
    //        return false;
    //    }
    //    minPoint= transform.localToWorldMatrix.MultiplyPoint(minPoint);
    //    maxPoint = transform.localToWorldMatrix.MultiplyPoint(maxPoint);

    //    HitNum++;
      
    //    Vector3 centerPoint = minPoint + (maxPoint - minPoint) * 0.5f;


        
    //    if (HitNum == 1)
    //    {
    //        for (int i = 0; i < everyFrameLeftPoint.Length; i++)
    //        {
    //            everyFrameLeftPoint[i] = minPoint;
    //            everyFrameRightPoint[i] = maxPoint;
    //        }
    //    }
      
    //    PointsArr[0] = PointsArr[2];
    //    PointsArr[1] = PointsArr[3];
    //    PointsArr[2] = minPoint;
    //    PointsArr[3] = maxPoint;
    //    if (HitNum>1)
    //    {

    //        // 移动距离是否过短
    //        float dis = (PointsArr[2] - PointsArr[0]).magnitude;
    //        float dis1 = (PointsArr[3] - PointsArr[1]).magnitude;
    //        if(dis<= MoveDistance || dis1<= MoveDistance)
    //        {
    //           HitNum--;

    //            return false;
    //        }
          
    //        UpdateArr(everyFrameLeftPoint, minPoint);
    //        UpdateArr(everyFrameRightPoint, maxPoint);
           
    //        int count = 0;
    //        int count1 = 0;
    //        for (int i = 0; i < everyFrameLeftPoint.Length - 1; i++)
    //        {
    //            Vector3 P = (everyFrameLeftPoint[i + 1] - everyFrameLeftPoint[i]).normalized;
    //            count += Vector3.Dot(P, transform.up) < 0?1:0;

    //            P = (everyFrameRightPoint[i + 1] - everyFrameRightPoint[i]).normalized;
    //            count1 += Vector3.Dot(P, transform.up) < 0?1:0;
                
    //        }
    //        if (count == everyFrameLeftPoint.Length-1 ||
    //                  count1 == everyFrameRightPoint.Length-1)
    //        {
    //            Debug.Log("前进");
    //        }
    //       else 
    //        {
    //            HitNum--;
    //            if (count == 0 && count1 == 0)
    //            {
    //              Debug.Log("重置");
    //                Reset();
    //            }
    //              Debug.Log("停留");
    //            return false;

    //        }
            
           
            
    //    }

    //    ////判断移动的方向是否大于angleLimit设置的最大角度
    //    //if (HitNum > 2)
    //    //{

    //    //    Vector3 P0 = (everyFrameLeftPoint[3] - everyFrameLeftPoint[2]).normalized;
    //    //    Vector3 P1 = (everyFrameLeftPoint[4] - everyFrameLeftPoint[3]).normalized;
    //    //    Vector3 P2 = (everyFrameRightPoint[3] - everyFrameRightPoint[2]).normalized;
    //    //    Vector3 P3 = (everyFrameRightPoint[4] - everyFrameRightPoint[3]).normalized;
    //    //    float angle0 = Vector3.Dot(P0, P1);
    //    //    float angle1 = Vector3.Dot(P2, P3);

    //    //    if (angle0 < angleLimit || angle1 < angleLimit)
    //    //    {
    //    //        Debug.Log("方向大于最大限制角度");
    //    //        Reset();
    //    //        return false;
    //    //    }

    //    //}

    //    if (HitNum>=2)
    //    {

    //        if (HitNum == 2)
    //        {
             
    //            GameObject sphere = GenrateGameObject(centerPoint, Vector3.zero) ;
              
    //            DamagedtissueList.Add(CurGenratePlaneTest);
    //            CurGenratePlaneTest.Init(damagedtissueData.IsmoreThanHalfCir, damagedtissueData.IsDrawDebugPoints, damagedtissueData.radius,
    //                damagedtissueData.speed, damagedtissueData.interval, damagedtissueData.material,changeTexture.getTexture());
                
    //        }
    //        else
    //        {

    //            Vector3 localMaxPoint = Vector3.zero;
    //            Vector3 localMinPoint = Vector3.zero;
    //            localMaxPoint = transform.worldToLocalMatrix.MultiplyPoint(maxPoint);
    //            localMinPoint = transform.worldToLocalMatrix.MultiplyPoint(minPoint);
    //            if (localMinPoint.x > localMaxPoint.x)
    //            {
    //                Vector3 tmep = maxPoint;
    //                maxPoint = minPoint;
    //                minPoint = tmep;
    //                Vector2 uvTemp = maxUV;
    //                maxUV = minUV;
    //                minUV = uvTemp;
    //            }
    //            if (HitNum == 3)
    //            {

                   
                  
    //                Vector3 right = (maxPoint - minPoint).normalized;
    //                Vector3 forward1 = normal;
    //                Vector3 up = Vector3.Cross(forward1, right);
      
    //                CurGenratePlaneTest.transform.rotation = Quaternion.LookRotation(-forward1, -up);
    //                CurGenratePlaneTest.recordInitTransfrom(minPoint, maxPoint,minUV,maxUV);
    //              //  CurGenratePlaneTest.dir = Vector3.up;
    //            }
    //            Vector3 correctPointMin;
    //            Vector3 correctPointMax;
    //            Vector2 correctUVMax;
    //            Vector2 correctUVMin;
    //            //CurGenratePlaneTest.correctionPositionAndUV(minPoint, maxPoint, minUV,maxUV,
    //            //    out correctPointMin, out correctPointMax,out correctUVMin,out correctUVMax);
    //            CurGenratePlaneTest.correctionPosition(minPoint, maxPoint,
    //             out correctPointMin, out correctPointMax);
    //            Vector3 from = correctPointMin + Vector3.up * 0.03f;
    //            Vector3 Dir = -Vector3.up;
    //            RaycastHit[] raycastHitsL = new RaycastHit[1];
    //            int hitCountL = Physics.RaycastNonAlloc(from, Dir, raycastHitsL, rayLength, layerMask);
    //            pointsL.Add(from);
    //            pointsR.Add(Dir);
    //            from = correctPointMax + Vector3.up * 0.03f;

    //            RaycastHit[] raycastHitsR = new RaycastHit[1];
    //            int hitCountR = Physics.RaycastNonAlloc(from, Dir, raycastHitsR, rayLength, layerMask);
    //            pointsL.Add(from);
    //            pointsR.Add(Dir);
    //            if (hitCountR == 0 || hitCountL == 0)
    //            {
    //                Debug.LogError("射线检测未检测到物体");
    //                return false;
    //            }

    //            minUV = raycastHitsL[0].textureCoord;
    //            maxUV = raycastHitsR[0].textureCoord;
    //            //minUV = correctUVMin;
    //            //maxUV = correctUVMax;

    //            CurGenratePlaneTest.SetVertex(correctPointMin, correctPointMax, minUV, maxUV);
    //            if (HitNum >= 4)
    //            {
    //                changeTexture.UpdateTexture( new Vector2[] { lastLeftUV , lastRightUV, minUV,maxUV });
    //            }
    //            lastLeftUV = minUV;
    //            lastRightUV = maxUV;

              
    //        }
    //    }
      
       
    //    return true;
    //}

    public bool CutButton()
    {


        if (!CollsionTest.collsion && !ON)
        {
            Reset();
            return false;
        }
        raycastsList.Clear();
        vertexsList.Clear();
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("hotSpot");
        Vector3 forward = LeftPoint.forward.normalized;
        for (int j = 0; j < rayPoints.Count; j++)
        {

            Vector3 from = rayPoints[j];
            Vector3 to = rayPoints[j] + forward * rayLength;


            RaycastHit[] raycastHits = new RaycastHit[1];

            int hitCount = Physics.RaycastNonAlloc(from, forward, raycastHits, rayLength, layerMask);
            if (hitCount > 0)
            {
                raycastsList.Add(raycastHits[0]);
                vertexsList.Add(from);

            }

        }
        if (raycastsList.Count == 0)
        {
            Reset();
            return false;

        }


        Vector3 minPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxPoint = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        Vector2 minUV = Vector3.zero;
        Vector2 maxUV = Vector3.zero;
        Vector3 normal = Vector3.zero;
        for (int i = 0; i < raycastsList.Count; i++)
        {
            if (changeTexture.isHit(raycastsList[i].textureCoord))
                continue;

            Vector3 pointL = transform.worldToLocalMatrix.MultiplyPoint(raycastsList[i].point);
            if (pointL.x < minPoint.x)
            {
                minPoint = pointL;
                minUV = raycastsList[i].textureCoord;
            }

            if (pointL.x > maxPoint.x)
            {
                maxPoint = pointL;
                maxUV = raycastsList[i].textureCoord;
                normal = raycastsList[i].normal;
            }




        }
        if (maxPoint.x == float.MinValue || minPoint.x == float.MaxValue || minPoint.x == maxPoint.x)
        {
            if (CurGenratePlaneTest != null)
            {
                Reset();
            }
            return false;
        }
        minPoint = transform.localToWorldMatrix.MultiplyPoint(minPoint);
        maxPoint = transform.localToWorldMatrix.MultiplyPoint(maxPoint);

        HitNum++;

        Vector3 centerPoint = minPoint + (maxPoint - minPoint) * 0.5f;



        if (HitNum == 1)
        {
            for (int i = 0; i < everyFrameLeftPoint.Length; i++)
            {
                everyFrameLeftPoint[i] = minPoint;
                everyFrameRightPoint[i] = maxPoint;
            }
        }

        PointsArr[0] = PointsArr[2];
        PointsArr[1] = PointsArr[3];
        PointsArr[2] = minPoint;
        PointsArr[3] = maxPoint;
        if (HitNum > 1)
        {

            // 移动距离是否过短
            float dis = (PointsArr[2] - PointsArr[0]).magnitude;
            float dis1 = (PointsArr[3] - PointsArr[1]).magnitude;
            if (dis <= MoveDistance || dis1 <= MoveDistance)
            {
                HitNum--;

                return false;
            }

            UpdateArr(everyFrameLeftPoint, minPoint);
            UpdateArr(everyFrameRightPoint, maxPoint);

            int count = 0;
            int count1 = 0;
            for (int i = 0; i < everyFrameLeftPoint.Length - 1; i++)
            {
                Vector3 P = (everyFrameLeftPoint[i + 1] - everyFrameLeftPoint[i]).normalized;
                count += Vector3.Dot(P, transform.up) < 0 ? 1 : 0;

                P = (everyFrameRightPoint[i + 1] - everyFrameRightPoint[i]).normalized;
                count1 += Vector3.Dot(P, transform.up) < 0 ? 1 : 0;

            }
            if (count == everyFrameLeftPoint.Length - 1 ||
                      count1 == everyFrameRightPoint.Length - 1)
            {
                Debug.Log("前进");
            }
            else
            {
                HitNum--;
                if (count == 0 && count1 == 0)
                {
                    Debug.Log("重置");
                    Reset();
                }
                Debug.Log("停留");
                return false;

            }



        }

        ////判断移动的方向是否大于angleLimit设置的最大角度
        //if (HitNum > 2)
        //{

        //    Vector3 P0 = (everyFrameLeftPoint[3] - everyFrameLeftPoint[2]).normalized;
        //    Vector3 P1 = (everyFrameLeftPoint[4] - everyFrameLeftPoint[3]).normalized;
        //    Vector3 P2 = (everyFrameRightPoint[3] - everyFrameRightPoint[2]).normalized;
        //    Vector3 P3 = (everyFrameRightPoint[4] - everyFrameRightPoint[3]).normalized;
        //    float angle0 = Vector3.Dot(P0, P1);
        //    float angle1 = Vector3.Dot(P2, P3);

        //    if (angle0 < angleLimit || angle1 < angleLimit)
        //    {
        //        Debug.Log("方向大于最大限制角度");
        //        Reset();
        //        return false;
        //    }

        //}

        if (HitNum >= 2)
        {

            if (HitNum == 2)
            {

                GameObject sphere = GenrateGameObject(centerPoint, Vector3.zero);

                DamagedtissueList.Add(CurGenratePlaneTest);
                CurGenratePlaneTest.Init(damagedtissueData.IsmoreThanHalfCir, damagedtissueData.IsDrawDebugPoints, damagedtissueData.radius,
                    damagedtissueData.speed, damagedtissueData.interval, damagedtissueData.material, changeTexture.getTexture());

            }
            else
            {

                Vector3 localMaxPoint = Vector3.zero;
                Vector3 localMinPoint = Vector3.zero;
                localMaxPoint = transform.worldToLocalMatrix.MultiplyPoint(maxPoint);
                localMinPoint = transform.worldToLocalMatrix.MultiplyPoint(minPoint);
                if (localMinPoint.x > localMaxPoint.x)
                {
                    Vector3 tmep = maxPoint;
                    maxPoint = minPoint;
                    minPoint = tmep;
                    Vector2 uvTemp = maxUV;
                    maxUV = minUV;
                    minUV = uvTemp;
                }
                if (HitNum == 3)
                {



                    Vector3 right = (maxPoint - minPoint).normalized;
                    Vector3 forward1 = normal;
                    Vector3 up = Vector3.Cross(forward1, right);

                    CurGenratePlaneTest.transform.rotation = Quaternion.LookRotation(-forward1, -up);
                    CurGenratePlaneTest.recordInitPoint(minPoint, maxPoint);
                    changeTexture.recordInitUV(minUV, maxUV);
                    CurGenratePlaneTest.SetVertex(minPoint, maxPoint, minUV, maxUV);
                    lastLeftUV = minUV;
                    lastRightUV = maxUV;
                    return false;
                }
                Vector3 correctPointMin;
                Vector3 correctPointMax;
                Vector2 correctUVMax;
                Vector2 correctUVMin;
             
                CurGenratePlaneTest.correctionPosition(minPoint, maxPoint,
                 out correctPointMin, out correctPointMax);
                changeTexture.correctionUV(lastLeftUV, lastRightUV,minUV, maxUV,out correctUVMin,out correctUVMax);
             

                CurGenratePlaneTest.SetVertex(correctPointMin, correctPointMax, correctUVMin, correctUVMax);
                changeTexture.UpdateTexture(new Vector2[] { lastLeftUV, lastRightUV, correctUVMin, correctUVMax });
             
            
                lastLeftUV = minUV;
                lastRightUV = maxUV;


            }
        }


        return true;
    }
        GameObject  GenrateGameObject(Vector3 centerPoint, Vector3 angle)
    {
        
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    
        go.transform.position = centerPoint;
        go.transform.eulerAngles = angle;
        go.transform.localScale = Vector3.one * 0.05f;
        GenratePlaneTest genratePlaneTest= go.AddComponent<GenratePlaneTest>();
       CurGenratePlaneTest = genratePlaneTest;
        return go;
    }

    private void Reset()
    {
      
      
        HitNum = 0;
        //if (CurGenratePlaneTest != null)
        //    Destroy(CurGenratePlaneTest.transform.gameObject);

        CurGenratePlaneTest = null;
       
        
        for (int i = 0; i < everyFrameLeftPoint.Length; i++)
        {
            everyFrameLeftPoint[i] = Vector3.zero;
            everyFrameRightPoint[i] = Vector3.zero;
        }
        for (int i = 0; i < PointsArr.Length; i++)
        {
            PointsArr[i] = Vector3.zero;
           
        }
    }
    public bool CutButton(ref Vector3 leftPoint,ref Vector3 rightPoint)
    {
        raycastsList.Clear();
        vertexsList.Clear();
        int layerMask = 0;
        layerMask |= 1 << LayerMask.NameToLayer("hotSpot");
        Vector3 forward = LeftPoint.forward.normalized;
        for (int j = 0; j < rayPoints.Count; j++)
        {
          
            Vector3 from = rayPoints[j];
            Vector3 to = rayPoints[j] + forward * rayLength;


            RaycastHit[] raycastHits = new RaycastHit[1];

            int hitCount = Physics.RaycastNonAlloc(from, (to - from).normalized, raycastHits, (to - from).magnitude, layerMask);
            if (hitCount > 0)
            {
                raycastsList.Add(raycastHits[0]);
                vertexsList.Add(from);
               

            }

        }
        if (raycastsList.Count == 0)
            return false;
      
        int index = -1;
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        for (int i = 0; i < raycastsList.Count; i++)
        {
            Vector3 pointL=  transform.worldToLocalMatrix.MultiplyPoint(raycastsList[i].point);
            if ( pointL.x< min.x)
            {
                min = raycastsList[i].point;
     
            }
            
            if (pointL.x > max.x)
            {
                max = raycastsList[i].point;
                max.x = raycastsList[i].point.x;
            }
      
        }
        leftPoint = min+(-forward* pointOffset);
        rightPoint = max + (-forward * pointOffset);
        Debug.DrawLine(min, min + -forward * rayLength, Color.blue);
        Debug.DrawLine(max, max + -forward * rayLength, Color.blue);
        return true;
    }

    void UpdateArr<T>(T []arr,T newValue)
    {
        if (arr == null)
            return;
        for(int i=0;i<arr.Length-1;i++)
        {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = newValue;
    }
    void AxisLookAt(Transform tr_self, Vector3 lookPos, Vector3 directionAxis)
    {


        var rotation = tr_self.rotation;
        var targetDir = lookPos - tr_self.position;
        //指定哪根轴朝向目标,自行修改Vector3的方向
        var fromDir = tr_self.rotation * directionAxis;
        //计算垂直于当前方向和目标方向的轴
        var axis = Vector3.Cross(fromDir, targetDir).normalized;
        //计算当前方向和目标方向的夹角
        var angle = Vector3.Angle(fromDir, targetDir);
        //将当前朝向向目标方向旋转一定角度，这个角度值可以做插值
        tr_self.rotation = Quaternion.AngleAxis(angle, axis) * rotation;
     //   tr_self.localEulerAngles = new Vector3(0, tr_self.localEulerAngles.y, 90);//后来调试增加的，因为我想让x，z轴向不会有任何变化
    }
    private void OnDrawGizmos()
    {
     
        for (int i = 0; i < pointsL.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(pointsL[i],0.01f);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(pointsL[i], pointsR[i]);
        }
    }
}
