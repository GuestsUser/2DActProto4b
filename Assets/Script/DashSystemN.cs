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
using UnityEngine.SceneManagement;

public class DashSystemN : Padinput
{
    [Header("新ダッシュ機能")]
    [Tooltip("ダッシュする時間")] [SerializeField] float runTime = 2f;
    [Tooltip("ダッシュしてから再発動可能になるまでの時間")] [SerializeField] float coolTime = 5f;
    [Tooltip("ダッシュの力")] [SerializeField] float dashForce = 5f;
    [Tooltip("移動方向に伸びる敵削除rayの長さ")] [SerializeField] public float rayDistance = 0.5f;
    [Tooltip("StageClearスクリプト")] [SerializeField] private StageClear stageClear;
    [Tooltip("敵削除overrapサイズ")] [SerializeField] private Vector3 delSize;
    [Tooltip("疾走UI")] [SerializeField] private GameObject dashUI;
    [Tooltip("移動量がこの値以下になった場合動いてない判定")] [SerializeField] private float moveBorder = 0.05f;
    static public bool dash = false;

     public bool _dash {get { return dash; } }  /* true疾走が使える false 疾走が使えない */
   
    private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    /*追加*/
    private PlayerMove player;

    private Animator animator;
    private bool timerDashPermit = true; /* 時間経過で管理するダッシュ使用可否 */

    [Tooltip("オーバーラップをプレイヤーの位置に持っていく為にずらす値")] [SerializeField] private Vector3 adjust;

    private Rigidbody rb;
    private PlayerMove control;
    private GroundFooter footer;
    private RetrySystem retrySys;
    private Vector3 overrapAdjust; /* overrap用adjust */
    private SESystem sound;

    /*疾走SE用変数*/
    private AudioSource DashSource;
    [SerializeField] private AudioClip DashSe;
    [SerializeField] private AudioClip CoolTimeSe;
    /*疾走SE用変数*/

    /*エフェクト発生用変数*/
    [SerializeField] ParticleSystem DashEfect;
    //ParticleSystem newParticle;
    [SerializeField] Transform ts;
    /*エフェクト発生用変数*/

    public bool CoolTimeFlg = false;

    /*追加*/
    public bool isDashDead = false;
    

    [System.NonSerialized] public GameObject standSlopeObj; /*現在乗ってる滑る床オブジェクト*/

    /* 外部からダッシュを禁止化する為のシステム用変数 */
    private static bool externalPermit = true;
    private static int banTaskCount = 0;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "StageSelect")
        {
            stageClear = GameObject.Find("ClearPoint").GetComponent<StageClear>();
        }
        dashUI = GameObject.Find("Gauge");
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        control = GetComponent<PlayerMove>();
        footer = GetComponent<GroundFooter>();
        retrySys = GetComponent<RetrySystem>();
        sound = GetComponent<SESystem>();
        DashSource = GetComponent<AudioSource>();

        /*追加*/
        player = GetComponent<PlayerMove>();

        //adjust = new Vector3(0, transform.localScale.y / 2, 0);
        overrapAdjust = new Vector3(delSize.x / 2, adjust.y, 0); /* overrap用adjust */

        externalPermit = true;
        banTaskCount = 0;


    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Stage2")
        {
            if (stageClear.stage2Clear && !dash)
            {
                dash = true;
            }
        }
        //newParticle = Instantiate(DashEfect, ts.transform.position, Quaternion.identity);
        //newParticle.transform.position = ts.transform.position;
    }

    public override void Skill()
    {
        if (dash)
        {
            footer.RideCheck();
            /*アビリティ発動ボタンが押されたら*/
            if (timerDashPermit && externalPermit && Gamepad.current.buttonWest.wasPressedThisFrame && (!PlayerKnockBack.runState) && footer.isGround)
            {
                StartCoroutine(Dush());
                StartCoroutine(ReCharge());
                CoolTimeFlg = true;
            }
        }
        
    }
    public void OnClickButton()
    {
        /*二段ジャンプフラグがfalseなら*/
        if (!dash)
        {
            /*二段ジャンプフラグをtrueにし、テスト用二段ジャンプオブジェクトを非表示にする*/
            dash = true;
            dashUI.SetActive(true);
            //doublejump.SetActive(false);
        }
        /*二段ジャンプフラグがtrueなら*/
        else
        {
            /*二段ジャンプフラグをfalseにし、テスト用二段ジャンプオブジェクトを表示する*/
            dash = false;
            //doublejump.SetActive(true);
        }
    }
    IEnumerator Dush()
    {
        /* 移動禁止とアニメ設定 */
        StartCoroutine(PlayerMove.MoveRestriction());
        //StartCoroutine(PlayerMove.RotateRestriction());

        /*アニメーション開始*/
        animator.SetBool("Attack",true);

        /*疾走音を鳴らす*/
        DashSource.PlayOneShot(DashSe);


        /*プリファブからエフェクトをセット*/
        ParticleSystem newParticle = Instantiate(DashEfect, ts.transform.position, Quaternion.identity);
        newParticle.loop = true;
        newParticle.Play(); /*エフェクトを発生*/
        /*プリファブからエフェクトをセット*/

        float count = 0f;
        Vector3 force = Vector3.zero;
        //RaycastHit rayHit;
        Collider[] hitObj;
        Vector2 old = transform.position; /* 前回位置 */
        Vector2 move = Vector2.zero; /* 前回から今回の位置を引いて出た値の格納、つまり移動量を取得、2dゲームなので念の為z移動量は加味しない */

        //ForceSet();
        while (!PlayerKnockBack.runState && !retrySys.isRetry && externalPermit) /* ノックバック実行、死亡、外部割込みで終了 */
        {
            newParticle.transform.position = ts.transform.position;
            move = (Vector2)transform.position - old; /* 座標移動の量を取得 */
            old = transform.position;

            footer.RideCheck();
            if (count > runTime && (footer.isGround || Mathf.Abs(move.x) + Mathf.Abs(move.y) < moveBorder)) { break; } /* 時間を超えている且つ接地若しくは移動量が規定値以下の場合終了 */

            /*追加*/
            /*右に入力されているとき*/
            float sub = Mathf.Cos(transform.localEulerAngles.y * Mathf.Deg2Rad); /* プレイヤーのy軸回転から向いている方向を符号で取得、0度なら1が、180なら-1が返ってくる寸法 */
            Vector3 useAdjust = overrapAdjust;
            useAdjust.x *= sub; /* xだけ向きに応じて回転する */
            hitObj = Physics.OverlapBox(transform.position + useAdjust, delSize / 2);
            //if (player.right != 0)
            //{
            //    hitObj = Physics.OverlapBox(transform.position + overrapAdjust, transform.localScale / 2);
            //}
            //else
            //{
            //    hitObj = Physics.OverlapBox(transform.position - overrapAdjust, transform.localScale / 2);
            //}
            foreach (Collider obj in hitObj)
            {
                if ( (obj.tag == "Enemy" || obj.tag == "Enemy3") && !isDashDead)
                {
                    Debug.Log("疾走で倒しました");

                    sound.audioSource.PlayOneShot(sound.se[sound.IndexToSub("dashKill")]);

                    isDashDead = true;
                    EnemyObjectCollision eCollision = obj.GetComponent<EnemyObjectCollision>();
                    if (eCollision != null)
                    {
                        eCollision.playerDash = true;
                    }
                }
                //else
                //{
                //    //isDashDead = false;
                //}

                /*追加*/
                //if (obj.tag == "Enemy3" && !isDashDead)
                //{
                //    //Debug.Log("疾走で倒しました");

                //    isDashDead = true;
                //    EnemyObjectCollision eCollision = obj.GetComponent<EnemyObjectCollision>();
                //    if(eCollision != null)
                //    {
                //        eCollision.playerDash = true;
                //    }
                //}
                //else
                //{
                //    //isDashDead = false;
                //}
            }
            ForceSet();
            rb.velocity = force;
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

        /*アニメーション終了*/
        animator.SetBool("Attack", false);

        PlayerMove.MoveRestrictionRelease();
        //PlayerMove.RotateRestrictionRelease();

        ForceReSet();

        void ForceSet() /* 毎フレームこれを実行する事によるダッシュ中の方向転換、それによる滑る床上でのvelocity.y固定化によるジャンプ無効問題の解決から */
        {
            float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
            if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
            {
                force.x = dashVector;
                force.y = rb.velocity.y;
            }
            else /* 床角度に合わせたダッシュ、滑る床以外もこれを使えば滑らないけど傾斜のある床をスムーズにダッシュできるかも */
            {
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
            newParticle.loop = false;
            newParticle.Stop();
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
        DashSource.PlayOneShot(CoolTimeSe);

    }

    

    public static IEnumerator Restriction() /* 禁止化コルーチン */
    {
        banTaskCount++; /* コルーチン実行数増加 */
        if (!externalPermit) { yield break; } /* コルーチンを複数起動させないため既に禁止状態なら抜ける */
        externalPermit = false; /* 禁止化 */
        while (banTaskCount > 0) { yield return null; } /* 実行数が0になるまで待機 */
        externalPermit = true; /* 制限解除 */

    }
    public static void RestrictionRelease() /* 禁止化解除 */
    {
        if (banTaskCount <= 0) { return; } /* 禁止化コルーチンを実行せず実行されると */
        banTaskCount--; /* 実行数削減 */
    }

    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + adjust + transform.right * rayDistance, delSize);
    }
}

