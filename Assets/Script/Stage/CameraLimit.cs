using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraLimit : MonoBehaviour
{
    [SerializeField] private PlayerMove player;
    
    //private Renderer targetRenderer = null;
    public bool end;
    public Vector3 end_pos;
    bool right; /* true:オブジェクトが右にある */
    bool left; /* true:オブジェクトが左にある */

    [SerializeField] private float distance; /* playerとobjectの間の距離 */
    private void Start()
    {
        player = GameObject.Find("Haruko").GetComponent<PlayerMove>();
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
        if(player.transform.position.x > transform.position.x)
        {
            distance = player.transform.position.x - transform.position.x;
        }
        else if(player.transform.position.x < transform.position.x)
        {
            distance = transform.position.x - player.transform.position.x;
        }

        if (distance <= 8)
        {
            end = true;
            end_pos = transform.position;
        }
        else if (distance > 8)
        {
            end = false;
            end_pos = Vector3.zero;
        }
    }

    //void OnBecameVisible()
    //{
    //    Debug.Log("カメラ");
    //    if(distance <= 8)
    //    {
    //        end = true;
    //        end_pos = transform.position;
    //    }

    //}
    //void OnBecameInvisible()
    //{
    //    if (distance > 8)
    //    {
    //        end = false;
    //        end_pos = Vector3.zero;
    //    }

    //}
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
