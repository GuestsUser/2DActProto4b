//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;

//public class DashSystemN : Padinput
//{
//    [Header("新ダッシュ機能")]
//    [Tooltip("ダッシュする時間")] [SerializeField] float runTime = 2f;
//    [Tooltip("ダッシュしてから再発動可能になるまでの時間")] [SerializeField] float coolTime = 5f;
//    [Tooltip("ダッシュの力")] [SerializeField] float dashForce = 5f;
//    [Tooltip("移動方向に伸びる敵削除rayの長さ")] [SerializeField] public float rayDistance = 0.5f;

//    private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

//    private Animator animator;
//    private bool timerDashPermit = true; /* 時間経過で管理するダッシュ使用可否 */

//    private Vector3 adjust; /* 基準位置を中心に持ってくる為の変数 */

//    private Rigidbody rb;
//    private PlayerMove control;
//    private GroundFooter footer;
//    private RetrySystem retrySys;

//    public bool CoolTimeFlg = false;

//    [System.NonSerialized] public GameObject standSlopeObj; /*現在乗ってる滑る床オブジェクト*/
//    void Start()
//    {
//        change_shoes = GetComponent<ChangeShoes>();
//        rb = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();
//        control = GetComponent<PlayerMove>();
//        footer = GetComponent<GroundFooter>();
//        retrySys = GetComponent<RetrySystem>();

//        adjust = new Vector3(0, transform.localScale.y / 2, 0);
//    }



//    public override void Skill()
//    {
//        footer.RideCheck();
//        /*アビリティ発動ボタンが押されたら*/
//        if (timerDashPermit && Gamepad.current.buttonWest.wasPressedThisFrame && (!PlayerKnockBack.runState) && footer.isGround)
//        {
//            StartCoroutine(Dush());
//            StartCoroutine(ReCharge());
//            CoolTimeFlg = true;
//        }
//    }

//    IEnumerator Dush()
//    {
//        /* 移動禁止とアニメ設定 */
//        StartCoroutine(PlayerMove.MoveRestriction());
//        StartCoroutine(PlayerMove.RotateRestriction());
//        animator.SetTrigger("Attack");

//        float count = 0f;
//        Vector3 force=Vector3.zero;
//        //RaycastHit rayHit;
//        Collider[] hitObj;
//        Vector3 overrapAdjust = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, 0); /* overrap用adjust */

//        //ForceSet();
//        while (!PlayerKnockBack.runState && !retrySys.isRetry) /* ノックバック実行、死亡で終了 */
//        {
//            hitObj = Physics.OverlapBox(transform.position + overrapAdjust, transform.localScale / 2);
//            foreach(Collider obj in hitObj)
//            {
//                if (obj.tag == "Enemy") { obj.gameObject.SetActive(false); } /* レイキャストに触れたenemyタグを持つオブジェクトは消えることになる */
//            }
//            //if (Physics.BoxCast(transform.position + adjust, transform.localScale / 2, transform.right, out rayHit, new Quaternion(), rayDistance))
//            //{
//            //    if (rayHit.collider.tag == "Enemy") { rayHit.collider.gameObject.SetActive(false); } /* レイキャストに触れたenemyタグを持つオブジェクトは消えることになる */
//            //}
//            ForceSet();
//            rb.velocity = force;

//            footer.RideCheck();
//            if (count> runTime && footer.isGround) { break; } /* ダッシュジャンプで急失速を防ぐためとりあえずダッシュ時間を超えても接地してないと抜けないようにした、急失速してもいいかどうかは要相談 */
//            count += Time.deltaTime;
//            yield return StartCoroutine(TimeScaleYield.TimeStop());
//        }

//        PlayerMove.MoveRestrictionRelease();
//        PlayerMove.RotateRestrictionRelease();

//        ForceReSet();

//        void ForceSet() /* 毎フレームこれを実行する事によるダッシュ中の方向転換、それによる滑る床上でのvelocity.y固定化によるジャンプ無効問題の解決から */
//        {
//            float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
//            if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
//            {
//                //Debug.Log("normal");
//                force.x = dashVector;
//                force.y = rb.velocity.y;
//            }
//            else /* 床角度に合わせたダッシュ、滑る床以外もこれを使えば滑らないけど傾斜のある床をスムーズにダッシュできるかも */
//            {
//                //Debug.Log("slope");
//                float floorZRad = standSlopeObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad; /* 床のz傾き(ラジアン) */
//                force = new Vector3(dashVector * Mathf.Abs(Mathf.Cos(floorZRad)), dashVector * Mathf.Abs(Mathf.Sin(floorZRad))); /* ダッシュベクトルを床の傾きに合わせて加工する */
//            }
//        }
//        void ForceReSet() /* 終了処理 */
//        {
//            float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
//            if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
//            {
//                force = rb.velocity;
//                force.x = 0;
//            }
//            else /* y移動もするのでこっちではyもリセット */
//            {
//                force.y = 0;
//                force.x = 0;
//            }

//            rb.velocity = force;
//        }
//    }

//    IEnumerator ReCharge()
//    {
//        timerDashPermit = false;
//        float count = 0f;
//        while (count < coolTime)
//        {
//            count += Time.deltaTime;
//            yield return StartCoroutine(TimeScaleYield.TimeStop());
//        }
//        timerDashPermit = true;
//        CoolTimeFlg = false;
//    }



//    void OnDrawGizmos()
//    {
//        //　Cubeのレイを疑似的に視覚化
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireCube(transform.position + adjust + transform.right * rayDistance, transform.localScale);
//    }
//}






/*上と同じだが元々のものを残しておくためのもの（龍が作業中）*/
/*とげ敵を倒したときの処理追加したもの*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashSystemN : Padinput
{
    [Header("新ダッシュ機能")]
    [Tooltip("ダッシュする時間")] [SerializeField] float runTime = 2f;
    [Tooltip("ダッシュしてから再発動可能になるまでの時間")] [SerializeField] float coolTime = 5f;
    [Tooltip("ダッシュの力")] [SerializeField] float dashForce = 5f;
    [Tooltip("移動方向に伸びる敵削除rayの長さ")] [SerializeField] public float rayDistance = 0.5f;

    private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    /*追加*/
    private PlayerMove player;

    private Animator animator;
    private bool timerDashPermit = true; /* 時間経過で管理するダッシュ使用可否 */

    private Vector3 adjust; /* 基準位置を中心に持ってくる為の変数 */

    private Rigidbody rb;
    private PlayerMove control;
    private GroundFooter footer;
    private RetrySystem retrySys;

    public bool CoolTimeFlg = false;

    /*追加*/
    public bool isDashDead = false;

    [System.NonSerialized] public GameObject standSlopeObj; /*現在乗ってる滑る床オブジェクト*/
    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        control = GetComponent<PlayerMove>();
        footer = GetComponent<GroundFooter>();
        retrySys = GetComponent<RetrySystem>();

        /*追加*/
        player = GetComponent<PlayerMove>();

        adjust = new Vector3(0, transform.localScale.y / 2, 0);
    }



    public override void Skill()
    {
        footer.RideCheck();
        /*アビリティ発動ボタンが押されたら*/
        if (timerDashPermit && Gamepad.current.buttonWest.wasPressedThisFrame && (!PlayerKnockBack.runState) && footer.isGround)
        {
            StartCoroutine(Dush());
            StartCoroutine(ReCharge());
            CoolTimeFlg = true;
        }
    }

    IEnumerator Dush()
    {
        /* 移動禁止とアニメ設定 */
        StartCoroutine(PlayerMove.MoveRestriction());
        StartCoroutine(PlayerMove.RotateRestriction());
        animator.SetTrigger("Attack");

        float count = 0f;
        Vector3 force = Vector3.zero;
        //RaycastHit rayHit;
        Collider[] hitObj;
        Vector3 overrapAdjust = new Vector3(transform.localScale.x / 2, transform.localScale.y / 2, 0); /* overrap用adjust */

        //ForceSet();
        while (!PlayerKnockBack.runState && !retrySys.isRetry) /* ノックバック実行、死亡で終了 */
        {
            /*追加*/
            /*右に入力されているとき*/
            if (player.right != 0)
            {
                hitObj = Physics.OverlapBox(transform.position + overrapAdjust, transform.localScale / 2);
            }
            else
            {
                hitObj = Physics.OverlapBox(transform.position - overrapAdjust, transform.localScale / 2);
            }
            foreach (Collider obj in hitObj)
            {
                if (obj.tag == "Enemy" && !isDashDead)
                {
                    Debug.Log("疾走で倒しました");

                    isDashDead = true;
                    EnemyObjectCollision eCollision = obj.GetComponent<EnemyObjectCollision>();
                    if (eCollision != null)
                    {
                        eCollision.playerDash = true;
                    }
                }
                else
                {
                    //isDashDead = false;
                }

                /*追加*/
                if (obj.tag == "Enemy3" && !isDashDead)
                {
                    //Debug.Log("疾走で倒しました");

                    isDashDead = true;
                    EnemyObjectCollision eCollision = obj.GetComponent<EnemyObjectCollision>();
                    if(eCollision != null)
                    {
                        eCollision.playerDash = true;
                    }
                }
                else
                {
                    //isDashDead = false;
                }
            }
            ForceSet();
            rb.velocity = force;

            footer.RideCheck();
            if (count > runTime && footer.isGround) { break; } /* ダッシュジャンプで急失速を防ぐためとりあえずダッシュ時間を超えても接地してないと抜けないようにした、急失速してもいいかどうかは要相談 */
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

        PlayerMove.MoveRestrictionRelease();
        PlayerMove.RotateRestrictionRelease();

        ForceReSet();

        void ForceSet() /* 毎フレームこれを実行する事によるダッシュ中の方向転換、それによる滑る床上でのvelocity.y固定化によるジャンプ無効問題の解決から */
        {
            float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
            if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
            {
                //Debug.Log("normal");
                force.x = dashVector;
                force.y = rb.velocity.y;
            }
            else /* 床角度に合わせたダッシュ、滑る床以外もこれを使えば滑らないけど傾斜のある床をスムーズにダッシュできるかも */
            {
                //Debug.Log("slope");
                float floorZRad = standSlopeObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad; /* 床のz傾き(ラジアン) */
                force = new Vector3(dashVector * Mathf.Abs(Mathf.Cos(floorZRad)), dashVector * Mathf.Abs(Mathf.Sin(floorZRad))); /* ダッシュベクトルを床の傾きに合わせて加工する */
            }
        }
        void ForceReSet() /* 終了処理 */
        {
            float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
            if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
            {
                force = rb.velocity;
                force.x = 0;
            }
            else /* y移動もするのでこっちではyもリセット */
            {
                force.y = 0;
                force.x = 0;
            }

            rb.velocity = force;
        }
    }

    IEnumerator ReCharge()
    {
        timerDashPermit = false;
        float count = 0f;
        while (count < coolTime)
        {
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        timerDashPermit = true;
        CoolTimeFlg = false;
    }



    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + adjust + transform.right * rayDistance, transform.localScale);
    }
}

