using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class JumpSystem : Padinput
{

    private Animator animator;
    private Rigidbody rb;

    /*y方向の位置を保存*/
    private float ySpeed;

    [SerializeField] private float jumpForce = 5.0f;/*ジャンプ力*/
    [SerializeField] private int jumpCount = 0;     /*ジャンプ回数*/

    [SerializeField] public bool isGrounded = false;       /* 地面と接触しているかどうか*/
    [SerializeField] private bool jumping = false;          /*Jumpingアニメーションしているかどうか*/
    [SerializeField] private bool fall = false;             /*Fallアニメーションしているかどうか*/
    [SerializeField] private bool groundFall = false;

    [SerializeField] private DashSystemN Dsn;

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.25f;

    /*靴のテストフラグ（ステージ攻略で靴を切り替える）*/
    static public bool jumpFlg_Test = false;   /*二弾ジャンプにさせたい場合このフラグにtrueを入れる*/

    /*テストフラグをシーン上に表示させる変数*/
    public Text JumpFlg;
    public GameObject gb;
    /*テストフラグをシーン上に表示させる変数*/

    /*アイテムを取得すると二弾ジャンプに変化する変数*/
    //public GameObject doublejump;
    /*アイテムを取得すると二弾ジャンプに変化する変数*/

    /* 4/11 - 仲里により追加 */
    public bool completion { set; get; } /* ジャンプが成立した瞬間trueにする、滑り床からジャンプで離れた瞬間を得る為必要になった、このスクリプトからtrueにした後、最後に別スクリプトからfalse化される */
    private static bool permit = true; /* trueでジャンプ使用可能 */
    private static int banTaskCount = 0; /* ジャンプ禁止化コルーチン実行数 */
    /* 仲里追加以上 */

    /*久場追加*/
    RaycastHit hit;
    private Vector3 size = new Vector3(1, -1f, 1); /*boxcastのサイズ*/

    /*ジャンプのSEよう変数*/
    [SerializeField] AudioClip jumpSe;
    [SerializeField] AudioClip jump2Se;
    AudioSource jumpSouce;
    /*ジャンプのSEよう変数*/

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        gb = GameObject.Find("JumpFlg");
        JumpFlg = gb.GetComponent<Text>();
        //doublejump = GameObject.Find("Test_DoubleJump");

        /* staticフラグリセット */
        permit = true;
        banTaskCount = 0;

        /*ジャンプのSEよう変数初期化*/
        jumpSouce = GetComponent<AudioSource>();
        /*ジャンプのSEよう変数初期化*/

    }
    private void Update()
    {
        /*ジャンプフラグをテキストで表示*/
        JumpFlg.text = "JumpFlg : " + jumpFlg_Test.ToString() + "\nジャンプ可能回数" + jumpCount.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Stage1Clear") /* 04/27糸数変更点(条件のみ変更) */
        {
            if (!jumpFlg_Test)
            {
                jumpFlg_Test = true;
                //doublejump.SetActive(false);
            }
            //else
            //{
            //    jumpFlg_Test = false;
            //}
        }
    }

    /*テスト用（ボタンをマウスでクリックすると二段ジャンプに切り替えることができる）*/
    public void OnClickButton()
    {
        /*二段ジャンプフラグがfalseなら*/
        if (!jumpFlg_Test)
        {
            /*二段ジャンプフラグをtrueにし、テスト用二段ジャンプオブジェクトを非表示にする*/
            jumpFlg_Test = true;
            //doublejump.SetActive(false);
        }
        /*二段ジャンプフラグがtrueなら*/
        else
        {
            /*二段ジャンプフラグをfalseにし、テスト用二段ジャンプオブジェクトを表示する*/
            jumpFlg_Test = false;
            //doublejump.SetActive(true);
        }
    }

    void FixedUpdate()
    {

        if (jumpFlg_Test)
        {

            //doublejump.SetActive(false);

        }
        else if (!jumpFlg_Test)
        {

            //doublejump.SetActive(true);

        }


        var velocity_y = rb.velocity.y;
        ySpeed = velocity_y;

        var scale = transform.lossyScale.x * 0.2f;
        var isHit = Physics.BoxCast(transform.position, size * scale, transform.up * -1, out hit, transform.rotation, rayDistance);
        /*boxCastの判定(箱の起点, 箱の大きさ * 調整用のscale, 判定をboxの下の面にする, hit , 箱の角度 , rayの長さ)*/
        if (isHit && (hit.collider.tag == "ground" || hit.collider.tag == "Slope")) // || isHit && hit.collider.tag == "Slope" && !isGrounded
        {
            //Debug.Log("あたってる");
            switch (jumpFlg_Test)
            {
                case true:
                    jumpCount = 2;
                    break;
                case false:
                    jumpCount = 1;
                    break;
            }

            isGrounded = true;
            animator.SetBool("IsGrounded", true);
        }
        /*地面から離れたら*/
        else
        {
            if (jumpCount == 2) --jumpCount;//ジャンプ回数を減らす

            isGrounded = false;
            animator.SetBool("IsGrounded", false);
        }

        /*５月19日追加（龍）*/
        //if(isHit && hit.collider.tag == "Slope")
        //{
        //    isGrounded = true;
        //    animator.SetBool("IsGrounded", true);
        //}

        Animation();
    }


    private void Animation()
    {
        /*地面と接触していてAボタンが押されたら（ジャンプモーション）*/
        if (isGrounded && Gamepad.current.buttonSouth.wasPressedThisFrame && Dsn.CoolTimeFlg == false)
        {
            jumping = true;
            animator.SetBool("Jumping", true);
        }
        /*ジャンプ上昇中でキャラクターのｙ方向の速度がマイナスになったら*/
        if (jumping && ySpeed < 0 && !isGrounded)
        {
            fall = true;
            jumping = false;
            animator.SetBool("Fall", true);
        }
        /*落下中で地面と接触したら*/
        if (fall && isGrounded)
        {
            fall = false;
            animator.SetBool("Jumping", false);
            animator.SetBool("Fall", false);
        }

        if (ySpeed < 0 && !jumping && !isGrounded)
        {
            fall = true;
            animator.SetBool("Fall", true);
        }
    }

    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    override public void Jump()
    {
        /* 4/14 仲里追加 */
        if (!permit) { return; } /* falseなら実行しない */
        /* 仲里追加以上 */

        /*二段ジャンプフラグがfalseなら*/
        if (!jumpFlg_Test)
        {
            /*ジャンプ回数制限が1以上かつ地面と接触していたら*/
            if (jumpCount >= 1 && isGrounded)
            {
                --jumpCount;
                rb.AddRelativeForce(rb.velocity.x, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
                completion = true; /* 成立したらtrue */

                jumpSouce.PlayOneShot(jumpSe);  /*音を鳴らす*/
            }
        }
        else
        {
            if (jumpCount == 2 && isGrounded)
            {
                --jumpCount;
                rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
                completion = true; /* 成立したらtrue */

                jumpSouce.PlayOneShot(jumpSe);  /*音を鳴らす*/
            }
            else if (jumpCount == 1 && !isGrounded)
            {
                --jumpCount;
                rb.AddRelativeForce(0, jumpForce * 1.2f, 0, ForceMode.VelocityChange);  /*二段目のジャンプは少し高く*/
                rb.velocity = Vector3.zero;
                completion = true; /* 成立したらtrue */

                jumpSouce.PlayOneShot(jump2Se);  /*音を鳴らす*/
            }
        }
    }

    /* 4/14 仲里追加 */
    public static IEnumerator Restriction() /* ジャンプ禁止化コルーチン */
    {
        /* 禁止化フラグをそのまま公開するスタイルだと複数箇所から禁止化された時、本来止めていたい状況で片方からジャンプ許可が降りる事があり、ジャンプできてしまう恐れがある */
        /* なのでこのコルーチンを通し実行数で管理する */
        banTaskCount++; /* コルーチン実行数増加 */
        if (!permit) { yield break; } /* コルーチンを複数起動させないため既に禁止状態なら抜ける */
        permit = false; /* 禁止化 */
        while (banTaskCount > 0) { yield return null; } /* 実行数が0になるまで待機 */
        permit = true; /* 制限解除 */

    }
    public static void RestrictionRelease() /* 禁止化解除 */
    {
        if (banTaskCount <= 0) { return; } /* 禁止化コルーチンを実行せず実行されると */
        banTaskCount--; /* 実行数削減 */
    }
    /* 仲里追加以上 */
    /*久場追加*/
    void OnDrawGizmos() /*boxcastを疑似的に可視化する(gizmosを利用)*/
    {
        var scale = transform.lossyScale.x * 0.2f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + transform.up * -1 * hit.distance, size * scale * 2);
    }
}