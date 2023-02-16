using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollsionTest : MonoBehaviour
{
    public static bool collsion=false;
  

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != 8)
            return;
        collsion = true;
        Debug.Log("Åö×²½øÈë");
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer != 8)
            return;
        collsion = false;
        Debug.Log("Åö×²Àë¿ª");
    }
}
