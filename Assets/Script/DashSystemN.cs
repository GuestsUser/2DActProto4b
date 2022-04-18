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

    private Animator animator;
    private bool timerDashPermit = true; /* 時間経過で管理するダッシュ使用可否 */
    private bool isGround = true; /* ノックバックと接地判定によるダッシュ使用可否 */

    private Vector3 adjust; /* 基準位置を中心に持ってくる為の変数 */

    private Rigidbody rb;
    private PlayerMove control;
    private float rad = 0; /* スフィア半径 */

    [System.NonSerialized] public GameObject standSlopeObj; /*現在乗ってる滑る床オブジェクト*/
    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        control = GetComponent<PlayerMove>();
        rad = GetComponent<CapsuleCollider>().radius;

        adjust = new Vector3(0, transform.localScale.y / 2, 0);
    }

    private void LateUpdate()
    {
        isGround = false; /* 最後にfalse化する */
    }

    public override void Skill()
    {
        /*アビリティ発動ボタンが押されたら*/
        if (timerDashPermit && Gamepad.current.buttonWest.wasPressedThisFrame && (!PlayerKnockBack.runState) && isGround)
        {
            StartCoroutine(Dush());
            StartCoroutine(ReCharge());
        }
    }

    IEnumerator Dush()
    {
        /* 移動禁止とアニメ設定 */
        StartCoroutine(PlayerMove.MoveRestriction());
        StartCoroutine(PlayerMove.RotateRestriction());
        animator.SetTrigger("Attack");

        float count = 0f;
        Vector3 force=Vector3.zero;
        RaycastHit rayHit;

        //ForceSet();
        while (!PlayerKnockBack.runState) /* ノックバック実行で終了する */
        {
            Debug.Log(isGround);
            if (Physics.BoxCast(transform.position + adjust, transform.localScale / 2, transform.right, out rayHit, new Quaternion(), rayDistance))
            {
                if (rayHit.collider.tag == "Enemy") { rayHit.collider.gameObject.SetActive(false); } /* レイキャストに触れたenemyタグを持つオブジェクトは消えることになる */
            }
            ForceSet();
            rb.velocity = force;

            if (count> runTime && isGround) { break; } /* ダッシュジャンプで急失速を防ぐためとりあえずダッシュ時間を超えても接地してないと抜けないようにした、急失速してもいいかどうかは要相談 */
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
    }

    private void OnCollisionStay(Collision other) /* 他オブジェクトとの接触がある時だけ判定 */
    {
        if (other.gameObject.tag == "ground")
        {
            //Debug.Log("ooooooook");
            RaycastHit getInfo;
            Vector3 root = transform.position;
            root.y += 0.5f; /* 完全に足元配置だと床が始点判定で列挙されない、なんて事が起きそうなので始点を上げておく */

            /* 現在SphereColliderなのでSphereCastにしたがBoxColliderならBoxCastと判定にあった形にする必要あり */

            foreach (RaycastHit hit in Physics.SphereCastAll(root, rad, Vector3.down, 0.6f))
            {
                if (hit.transform.gameObject.tag == "ground")
                {
                    isGround = true;
                    return;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + adjust + transform.right * rayDistance, transform.localScale);
    }
}
