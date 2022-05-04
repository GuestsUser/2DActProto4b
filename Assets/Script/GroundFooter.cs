using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFooter : MonoBehaviour
{
    [Header("足元の接地判定")]
    [Tooltip("完全に足元からrayを出すと床との判定を取ってくれない事があるのでこの値分yの値をずらして出す")] [SerializeField] float adjust = 0.3f;
    [Tooltip("rayの長さ、adjust+distanceの値がrayの長さになる")] [SerializeField] float distance = 0.1f;

    public bool isGround { get; set; } /* 接地してればtrue */
    public GameObject floor { get; set; } /* 接地してる床格納 */

    private float rad = 0; /* スフィア半径 */
    private Collision obj;

    // Start is called before the first frame update
    void Start()
    {
        rad = GetComponent<CapsuleCollider>().radius;
    }

    private void LateUpdate()
    {
        //if (obj != null) { OnCollisionStay(obj); }
        //isGround = false; /* 最後にfalse化する */
        //floor = null; /* 空にする */
    }

    /* 接地判定がlateUpDateでnullにされ再代入されるのがOnCollisionXXXな都合上それより前のライフサイクルのイベントではこの接地判定は機能しない */
    /* OnCollisionEnter→Stayの順で実行されるらしいのでEnterでこの接地判定は利用不可 */

    //private void OnCollisionStay(Collision other) /* 他オブジェクトとの接触がある時だけ判定 */
    //{
    //    obj = other;
    //    Debug.Log("oooooo");
    //    if (other.gameObject.tag == "ground")
    //    {
    //        Vector3 root = transform.position;
    //        root.y += adjust; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

    //        /* 現在SphereColliderなのでSphereCastにしたがBoxColliderならBoxCastと判定にあった形にする必要あり */
    //        foreach (RaycastHit hit in Physics.SphereCastAll(root, rad, Vector3.down, adjust + distance))
    //        {
    //            if (hit.transform.gameObject.tag == "ground")
    //            {
    //                isGround = true;
    //                floor = hit.transform.gameObject;
    //                return;
    //            }
    //        }
    //    }
    //}

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Slope") { obj = other; }
    }
    private void OnCollisionExit(Collision other)
    {
        obj = null;
        isGround = false; /* 最後にfalse化する */
        floor = null; /* 空にする */
    }

    public void RideCheck() /* 接地判定 */
    {
        if (obj!=null)
        {
            Vector3 root = transform.position;
            root.y += adjust; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

            /* 現在SphereColliderなのでSphereCastにしたがBoxColliderならBoxCastと判定にあった形にする必要あり */
            foreach (RaycastHit hit in Physics.SphereCastAll(root, rad, Vector3.down, adjust + distance))
            {
                if (hit.transform.gameObject.tag == "ground" || hit.transform.gameObject.tag == "Slope")
                {
                    isGround = true;
                    floor = hit.transform.gameObject;
                    return;
                }
            }
        }
    }
}
