using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFooter : MonoBehaviour
{
    [Header("足元の接地判定")]
    [Tooltip("完全に足元からrayを出すと床との判定を取ってくれない事があるのでこの値分yの値をずらして出す")] [SerializeField] float adjust = 0.3f;
    [Tooltip("rayの長さ、adjust+distanceの値がrayの長さになる")] [SerializeField] float distance = 0.1f;
    [Tooltip("接触オブジェクト保持配列キャパシティ")] [SerializeField] int groundCapa = 22;

    public bool isGround = false; /* 接地してればtrue */
    public GameObject floor;/* 接地してる床格納 */
    //{ get; set; } 
    private float rad = 0; /* スフィア半径 */
    [SerializeField] private GameObject[] obj;

    private int validArrayVol = 0; /* 配列のnullではない要素数 */

    // Start is called before the first frame update
    void Start()
    {
        rad = GetComponent<CapsuleCollider>().radius;
        obj = new GameObject[groundCapa];
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
        int insertSub = -1; /* obj配列の中身がnullで一番若い添え字、今回の挿入位置を記録、-1は挿入位置未確定を意味する */
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Slope")
        {
            for (int i = 0; i < groundCapa; i++)
            {
                if (other.gameObject == obj[i]) { return; } /* 自身と同じオブジェクトが既に入っていた場合挿入せず終了 */
                if (insertSub < 0 && obj[i] == null) { insertSub = i; } /* 挿入位置未確定且つnullの位置を見つけた場合そこに挿入を決定する */
            }
            if (insertSub != -1) /* 配列に空きがなかった場合を除き挿入 */
            {
                obj[insertSub] = other.gameObject;
                ++validArrayVol; /* 有効要素数増加 */
            }
        }

        
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Slope")
        {
            for (int i = 0; i < groundCapa; i++) /* 同じオブジェクトが複数挿入されてしまう可能性を考慮し配列を全て回し離れたオブジェクトは配列から完全消去する */
            {
                if (other.gameObject == obj[i])
                {
                    obj[i] = null;
                    --validArrayVol;
                }
            }
        }
    }

    public void RideCheck() /* 接地判定 */
    {
        if (validArrayVol>0) /* 1つ以上のオブジェクトと接触がある場合だけ実行 */
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
        isGround = false;
    }


    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.cyan;
        Vector3 root = transform.position;
        root.y += adjust; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */
        Gizmos.DrawSphere(root, rad);

        root.y -= adjust + distance;

        Gizmos.DrawSphere(root, rad);
    }
}
