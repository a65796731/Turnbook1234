using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProjectorTest : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform C;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(A.position, B.position, Color.white);
        Debug.DrawLine(B.position, C.position, Color.red);
       Vector3 D =A.position - B.position;
        Vector3 E = (C.position - B.position).normalized;
       Vector3 pro=  Vector3.Project(D, E);
       float dis=  Vector3.Dot(E, D);
        Debug.Log(dis);
        Debug.DrawLine(B.position, (C.position - B.position) * dis, Color.black);
       // Debug.DrawLine(C.position, E * dis, Color.white);
    }
}
