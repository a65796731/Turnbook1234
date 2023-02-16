using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public float speed = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.T))
        {
           
            transform.Translate(new Vector3(0, 0*Time.deltaTime* speed, -0.1f*Time.deltaTime* speed),Space.World);

        }

        //float h = Input.GetAxis("Horizontal");//这个代码的意思是 对应键盘上面的左右箭头，当按下左或右箭头时触发

        //float v = Input.GetAxis("Vertical"); //对应键盘上面的上下箭头，当按下上或下箭头时触发

        //if(h!=0||v!=0)
        //{
        //    transform.position += Vector3.forward * v * Time.deltaTime*5f + Vector3.up * v * Time.deltaTime * 5f;
        //    transform.position += Vector3.right * h * Time.deltaTime * 5f;
        //}
        
    }
}
