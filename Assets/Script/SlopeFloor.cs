using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 移動床は乗ってきたオブジェクトを子オブジェクト化する都合上、床に回転があると乗ったオブジェクトが乱れてしまうので滑る床との併用不可 */
public class SlopeFloor : MonoBehaviour
{
    [Header("乗ると滑る床")]
    [Tooltip("乗っている間加え続けられる力")] [SerializeField] float force = -10f;

    private float[] minMax = { 0, 0 }; /* この床のx座標最小値と最大値 */
    private float calculatRad = 0; /* 最後に計算に使ったスフィア半径サイズ記録、これとradが違った場合ヒット判定の再計算を必要とする */

    private static GameObject player; /* プレイヤーを保持する変数 */
    private static JumpSystem jump; /* ジャンプシステム保持 */
    private static Rigidbody pb; /* プレイヤーリジットボディ保持 */
    private static SlopeFriction friction; /* 吹っ飛び防止コンポーネント保持 */
    private static float rad = 0; /* スフィア半径 */

    private bool run = false; /* 滑りコルーチン実行状況 */

    // Start is called before the first frame update
    void Start()
    {
        Vector3 scale = transform.localScale / 2;
        float size = (transform.localScale.x) * Mathf.Cos(transform.localEulerAngles.z * Mathf.Deg2Rad) / 2;

        int capacity = 16;
        for(int i = 0; i < 2; i++) /* 1回目は+方向、2回目は-方向へrayが飛ぶ */
        {
            Vector3 objRotate = transform.localRotation.eulerAngles;

            if (i == 1) { objRotate.z += 180; } /* 2回目の実行時はz+180で方向転換する */
            objRotate *= Mathf.Deg2Rad;

            Vector3 direction = new Vector3(Mathf.Cos(objRotate.z), Mathf.Sin(objRotate.z), 0);

            Ray ray = new Ray(transform.position, direction);
            RaycastHit[] hit = Physics.RaycastAll(ray, scale.x);

            List<float> point = new List<float>(capacity);
            point.Add(transform.position.x + size * (1 - 2 * i));
            foreach (RaycastHit col in hit)
            {
                if (col.transform.tag == "ground" || col.transform.tag == "Slope") { point.Add(col.point.x); }
            }
            point.Sort();
            minMax[(i + 1) % 2] = point[(point.Count - 1) * (i % 2)]; /* 一番接触の早かった位置を限界点とする(何処とも接触がなかった場合自身サイズ)、-方向にrayを飛ばした場合一番大きい値、+方向なら一番小さい値 */
        }
        

    }

    /* Enterで接触判定を取って足元から真下に向けて長距離(長さ無限が好ましい)rayを飛ばしそのrayがEnterで接触したオブジェクトとの接触を示したら接地とする */
    /* 確実に離れるアクションはジャンプ、消える床との併用 */
    /* プレイヤーx座標が滑る床中心から床x拡大率の半分を超えるか-半分を下回ると離れた事になる */

    /* よじ登る処理の影響 */
    /* 触れた瞬間は床の横なので接地してない判定、しかし触れた瞬間のみの判定になってるのでよじ登りで上陸するとEnterが呼ばれない事により滑り効果が発揮されない */
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && (!run))
        {
            //Debug.Log("ooooooook");
            if (player == null) { GetPlayer(other.gameObject); } /* playerに何も入ってなければ入れておく */
            if (calculatRad != rad) { HitAreaCalculat(); } /* 判定の最小、最大値の計算 */


            //RaycastHit getInfo;
            Vector3 root = other.gameObject.transform.position;
            root.y += 0.5f; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

            /* 現在SphereColliderなのでSphereCastにしたがBoxColliderならBoxCastと判定にあった形にする必要あり */

            foreach (RaycastHit hit in Physics.SphereCastAll(root, rad, Vector3.down))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    other.gameObject.GetComponent<DashSystemN>().standSlopeObj = gameObject;
                    StartCoroutine(SlopeForce());
                    break;
                }
            }

            //if ( Physics.SphereCast(root, rad, Vector3.down, out getInfo) && getInfo.transform.gameObject==gameObject) /* 下向きに飛ばしたrayが一番最初に接触したオブジェクトが自身なら接地 */
            //{
            //    //Debug.Log("rayhit");
            //    other.gameObject.GetComponent<DashSystemN>().standSlopeObj = gameObject;
            //    StartCoroutine(SlopeForce());
            //}
            //Debug.Log(getInfo.transform.gameObject);
        }
    }

    //private void OnCollisionExit(Collision other)
    //{
    //    if (other.gameObject.tag == "Player" && run) { run = false; }
    //}

    private static void GetPlayer(GameObject obj) /* プレイヤー系情報をstatic変数に格納 */
    {
        player = obj; /* playerに関するアクションはEnterから始まるのでインスペクターからplayerを受け取らずともok */
        jump = player.GetComponent<JumpSystem>(); /* ジャンプシステムを受け取る */
        pb = player.GetComponent<Rigidbody>(); /* リジットボディ格納 */
        friction = player.GetComponent<SlopeFriction>(); /* 吹っ飛び防止格納 */

        foreach(CapsuleCollider get in player.GetComponents<CapsuleCollider>())
        {
            if (!get.isTrigger) /* isTriggerがfalseのコンポーネントを取得する */
            {
                rad = get.radius;
                break;
            }
        }
    }

    private void HitAreaCalculat() /* ヒット判定エリアの計算 */
    {
        calculatRad = rad;
        //float size = (transform.localScale.x ) * Mathf.Cos(transform.localEulerAngles.z * Mathf.Deg2Rad) / 2; /* xサイズ半分とプレイヤー当たり判定半径半分が床に乗れる限界位置 */
        minMax[0] -= rad / 2;
        //minMax[1] += rad / 2;
    }

    IEnumerator SlopeForce()
    {
        run = true;
        StartCoroutine(JumpSystem.Restriction()); /* ジャンプ禁止化 */
        //StartCoroutine(PlayerMove.RotateRestriction());

        //Debug.Log("kkkkkk");

        Vector3 moveVol = Vector3.zero; /* フレーム間の各軸移動量を取得 */
        Vector3 oldPos = player.transform.position; /* 前回位置 */
        Vector3 angle = transform.rotation.eulerAngles;
        while (run)
        {
            //Debug.Log(pb.velocity.x);
            float px = player.transform.position.x;
            if (px < minMax[0] || px > minMax[1]) { break; } /* 床部分を過ぎると終了 */
            //if (jump.completion) { break; } /* ジャンプ成立で終了 */

            angle = transform.rotation.eulerAngles; /* 毎回角度を取得する事で床を回したりもできそう */
            pb.AddForce(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), 0, ForceMode.Acceleration);

            moveVol = player.transform.position - oldPos;
            oldPos = player.transform.position;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        /* 出た瞬間のvelocityを減らせば凄まじい力で吹っ飛ばされなくなる */
        run = false;
        angle = transform.rotation.eulerAngles;

        JumpSystem.RestrictionRelease(); /* 禁止化解除 */
        ObjLeave();
        StartCoroutine(friction.CourseOutFriction(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), moveVol));

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

    //void OnDrawGizmos() /*boxcastを疑似的に可視化する(gizmosを利用)*/
    //{
    //    var scale = transform.lossyScale.x * 0.2f;
    //    Gizmos.color = Color.green;

    //    Vector3 start = transform.position;
    //    Vector3 end = transform.position;
    //    start.x = minMax[0];
    //    end.x = minMax[0];
    //    end.y = -4;

    //    Gizmos.DrawLine(start,end);

    //    start = transform.position;
    //    end = transform.position;
    //    start.x = minMax[1];
    //    end.x = minMax[1];
    //    end.y = 9;

    //    Gizmos.DrawLine(start, end);
    //}
}
