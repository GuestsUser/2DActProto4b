using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 移動床は乗ってきたオブジェクトを子オブジェクト化する都合上、床に回転があると乗ったオブジェクトが乱れてしまうので滑る床との併用不可 */
public class SlopeFloor : MonoBehaviour
{
    [Header("乗ると滑る床")]
    [Tooltip("乗っている間加え続けられる力")] [SerializeField] float force = 0.5f;

    private float[] minMax = { 0, 0 }; /* この床のx座標最小値と最大値 */
    private GameObject player; /* プレイヤーを保持する変数 */
    private JumpSystem jump; /* ジャンプシステム保持 */

    /* プレイヤーがboxcolliderで滑る床端に乗るとOnCollisionEnterが実行されないのか滑る床が動かない事がある */
    // Start is called before the first frame update
    void Start()
    {
        float size= transform.localScale.x * Mathf.Cos(transform.localEulerAngles.z * Mathf.Deg2Rad) / 2;
        minMax[0] = transform.position.x - size;
        minMax[1] = transform.position.x + size;
    }

    /* Enterで接触判定を取って足元から真下に向けて長距離(長さ無限が好ましい)rayを飛ばしそのrayがEnterで接触したオブジェクトとの接触を示したら接地とする */
    /* 確実に離れるアクションはジャンプ、消える床との併用 */
    /* プレイヤーx座標が滑る床中心から床x拡大率の半分を超えるか-半分を下回ると離れた事になる */
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("ooooooook");
            player = other.gameObject; /* playerに関するアクションはEnterから始まるのでインスペクターからplayerを受け取らずともok */
            jump = player.GetComponent<JumpSystem>(); /* ジャンプシステムを受け取る */

            RaycastHit getInfo;
            Vector3 root = other.gameObject.transform.position;
            root.y += 0.5f; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

            /* 当たり判定の形にあったrayに取り替える */
            if( Physics.Raycast(root, Vector3.down,out getInfo) && getInfo.transform.gameObject==gameObject) /* 下向きに飛ばしたrayが一番最初に接触したオブジェクトが自身なら接地 */
            {
                //Debug.Log("rayhit");
                other.gameObject.GetComponent<DashSystemN>().standSlopeObj = gameObject;
                StartCoroutine(SlopeForce());
            }
            
        }
    }
    IEnumerator SlopeForce()
    {
        while (true)
        {
            float px = player.transform.position.x;
            if (px < minMax[0] || px > minMax[1]) { break; } /* x座標が床の範囲を離れたら終了 */
            if (jump.completion) { break; } /* ジャンプ成立で終了 */

            Vector3 angle = transform.rotation.eulerAngles;
            player.gameObject.GetComponent<Rigidbody>().AddForce(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), 0, ForceMode.Acceleration);

            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        /* 出た瞬間のvelocityを減らせば凄まじい力で吹っ飛ばされなくなる */

        
        ObjLeave();
    }

    void OnDisable() /* このオブジェクトが無効化、削除される瞬間に実行 */
    {
        ObjLeave();
    }

    void ObjLeave() /* 床から離れた瞬間に呼び出してplayerの乗ってる床が自身だった場合取り除く */
    {
        if (player != null && player.gameObject.GetComponent<DashSystemN>().standSlopeObj == gameObject) /* 乗ってる床に自身と関係ないオブジェクトが入っている可能性を取り除く */
        {
            player.gameObject.GetComponent<DashSystemN>().standSlopeObj = null; /* 乗っている床が自身だった場合に限り変数を空にする */
        }
    }
}
