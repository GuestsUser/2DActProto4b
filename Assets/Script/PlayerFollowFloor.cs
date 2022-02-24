using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* これを付けたオブジェクトにプレイヤーが触れる事でプレイヤーの親をこのオブジェクトに変更、そうする事で乗ってる床なんかとプレイヤーが一緒に動くようになる */
public class PlayerFollowFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = transform;
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.transform.parent = null;
        }
    }
}
