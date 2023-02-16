using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectionTest : MonoBehaviour
{
    public static bool test;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        test = true;
        Debug.Log("碰撞对象：" + other.transform.name);
        // if (other.name.Contains("股骨")&& !isCollision)
        // {
        //     isCollision = true;
        //     Target.position = sphere.position;
        //     foreach (var v in lineRenders)
        //         v.beginBrush(sphere);
        // }
        //else  if (other.tag.Contains("TouchArea") && !InArea)
        // {
        //     InArea = true;

        // }
        //hotSpot.gameObject.SetActive(true);
        //hotSpot.transform.position = sphere.position;
        // hotSpot.transform.LookAt(lookAtTarget);


    }
    private void OnTriggerExit(Collider other)
    {
        test = false;
        //if (other.name.Contains("股骨"))
        //{
        //    isCollision = false;

        //    foreach (var v in lineRenders)
        //        v.endBrush();
        //}
        //else if (other.tag.Contains("TouchArea") )
        //{
        //    InArea = false;
        //    foreach (var v in lineRenders)
        //        v.endBrush();
        //}
    }
}
