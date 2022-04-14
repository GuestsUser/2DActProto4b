using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 移動床は乗ってきたオブジェクトを子オブジェクト化する都合上、床に回転があると乗ったオブジェクトが乱れてしまうので滑る床との併用不可 */
public class SlopeFloor : MonoBehaviour
{
    [Header("乗ると滑る床")]
    [Tooltip("乗っている間加え続けられる力")] [SerializeField] float force = 0.5f;

    private float[] minMax = { 0, 0 }; /* この床のx座標最小値と最大値 */
    private float calculatRad = 0; /* 最後に計算に使ったスフィア半径サイズ記録、これとradが違った場合ヒット判定の再計算を必要とする */

    private static GameObject player; /* プレイヤーを保持する変数 */
    private static JumpSystem jump; /* ジャンプシステム保持 */
    private static Rigidbody pb; /* プレイヤーリジットボディ保持 */
    private static float rad = 0; /* スフィア半径 */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /* Enterで接触判定を取って足元から真下に向けて長距離(長さ無限が好ましい)rayを飛ばしそのrayがEnterで接触したオブジェクトとの接触を示したら接地とする */
    /* 確実に離れるアクションはジャンプ、消える床との併用 */
    /* プレイヤーx座標が滑る床中心から床x拡大率の半分を超えるか-半分を下回ると離れた事になる */
    
    /* よじ登る処理の影響 */
    /* 触れた瞬間は床の横なので接地してない判定、しかし触れた瞬間のみの判定になってるのでよじ登りで上陸するとEnterが呼ばれない事により滑り効果が発揮されない */
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Debug.Log("ooooooook");
            if (player == null) { GetPlayer(other.gameObject); } /* playerに何も入ってなければ入れておく */
            if (calculatRad != rad) { HitAreaCalculat(); } /* 判定の最小、最大値の計算 */

            RaycastHit getInfo;
            Vector3 root = other.gameObject.transform.position;
            root.y += 0.5f; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

            /* 現在SphereColliderなのでSphereCastにしたがBoxColliderならBoxCastと判定にあった形にする必要あり */
            if ( Physics.SphereCast(root, rad, Vector3.down, out getInfo) && getInfo.transform.gameObject==gameObject) /* 下向きに飛ばしたrayが一番最初に接触したオブジェクトが自身なら接地 */
            {
                //Debug.Log("rayhit");
                other.gameObject.GetComponent<DashSystemN>().standSlopeObj = gameObject;
                StartCoroutine(SlopeForce());
            }
            
        }
    }
    private static void GetPlayer(GameObject obj) /* プレイヤー系情報をstatic変数に格納 */
    {
        player = obj; /* playerに関するアクションはEnterから始まるのでインスペクターからplayerを受け取らずともok */
        jump = player.GetComponent<JumpSystem>(); /* ジャンプシステムを受け取る */
        pb = player.GetComponent<Rigidbody>(); /* リジットボディ格納 */
        rad = player.GetComponent<CapsuleCollider>().radius;
    }

    private void HitAreaCalculat() /* ヒット判定エリアの計算 */
    {
        calculatRad = rad;
        float size = (transform.localScale.x + calculatRad / 2) * Mathf.Cos(transform.localEulerAngles.z * Mathf.Deg2Rad) / 2; /* xサイズ半分とプレイヤー当たり判定半径半分が床に乗れる限界位置 */
        minMax[0] = transform.position.x - size;
        minMax[1] = transform.position.x + size;
    }

    IEnumerator SlopeForce()
    {
        StartCoroutine(JumpSystem.Restriction()); /* ジャンプ禁止化 */

        Vector3 moveVol = Vector3.zero; /* フレーム間の各軸移動量を取得 */
        Vector3 oldPos = player.transform.position; /* 前回位置 */
        while (true)
        {
            float px = player.transform.position.x;
            if (px < minMax[0] || px > minMax[1]) { break; } /* 床部分を過ぎると終了 */
            //if (jump.completion) { break; } /* ジャンプ成立で終了 */

            Vector3 angle = transform.rotation.eulerAngles; /* 毎回角度を取得する事で床を回したりもできそう */
            pb.AddForce(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), 0, ForceMode.Acceleration);

            moveVol = player.transform.position - oldPos;
            oldPos = player.transform.position;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        /* 出た瞬間のvelocityを減らせば凄まじい力で吹っ飛ばされなくなる */

        JumpSystem.RestrictionRelease(); /* ジャンプ禁止化解除(仮導入) */
        ObjLeave();
        //StartCoroutine(CourseOutFriction(moveVol));
    }

    /* 床が消えると困るのでこのコルーチンは別の場所に移すべきかも */
    IEnumerator CourseOutFriction(Vector3 move) /* 床を抜けると凄まじい力が掛かるのでオリジナル摩擦 */
    {
        /* ジャンプで抜けた場合について */
        /* 逆方向に小さな座標しか動いてない状況でジャンプすると再着地される恐れあり */
        /* 再着地されると滑る力を無力化して進めてしまう */
        /* 今のジャンプスクリプトによるとジャンプによりx加速度初期化が入ってないのでジャンプで抜けない方がいいかもしれない */
        /* ジャンプ中滑る力一時停止的な処理にできるといいかも */
        pb.velocity = Vector3.zero;
        Vector3 angle = transform.rotation.eulerAngles;

        /* 各軸の摩擦コルーチン実行状況 */
        bool x_run = false;
        bool y_run = false;

        /* 各軸に掛かっていた力 */
        float x_force = force * Mathf.Cos(angle.z * Mathf.Deg2Rad);
        float y_force = force * Mathf.Sin(angle.z * Mathf.Deg2Rad);

        /* 滑る力を掛けられていた方向と同じ方向に動いていた(押し戻された若しくは駆け下りていた)場合摩擦コルーチンを開始(力が掛かっていなかった場合開始しない) */
        if (x_force != 0 && (move.x > 0) == (x_force > 0)) { }
        if (y_force != 0 && (move.y > 0) == (y_force > 0)) { }

        /* 終了条件は他にもプレイヤーのダッシュやノックバック、死亡、他の移動床に乗った場合 */
        while (x_run && y_run)
        {
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

        JumpSystem.RestrictionRelease(); /* ジャンプ禁止化解除 */
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
