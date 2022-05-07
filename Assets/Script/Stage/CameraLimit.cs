using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraLimit : MonoBehaviour
{

    /*方向転換用*/
    
    //private Renderer targetRenderer = null;
    public bool end;
    public Vector3 end_pos;
    private void Start()
    {
        //targetRenderer = GetComponent<Renderer>();
        end = false;
    }

    void Update()
    {
        //if (targetRenderer.isVisible)
        //{
        //    end = true;
        //}
        //else 
        //{
        //    end = false;
        //}
    }
    void OnBecameVisible()
    {
        end = true;
        end_pos = transform.position;
    }
    void OnBecameInvisible()
    {
        end = false;
        end_pos = Vector3.zero;
    }
}


//public class CameraLimit : MonoBehaviour
//{
//    private Renderer targetRenderer = null;

//    public bool end;

//    // Start is called before the first frame update
//    void Start()
//    {
//        targetRenderer = GetComponent<Renderer>();
//        end = false;
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (targetRenderer.isVisible)
//        {
//            end = true;
//        }
//        else
//        {
//            end = false;
//        }
//    }
//}
