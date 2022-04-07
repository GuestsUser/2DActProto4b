using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpSystem : Padinput
{

    /*3月24日ビルド用*/
    private ChangeShoes change_shoes;
    private Animator animator;
    private Rigidbody rb;    /*みんな大好きリジッドボディー*/
    private float y;

    [SerializeField] private float jumpForce = 5.0f;/*ジャンプ力*/
    [SerializeField] private int doubleJump = 0;     /*ジャンプ回数*/

    [SerializeField] private bool isGrounded;       /* 地面と接触しているかどうか*/
    [SerializeField] private bool jumping;          /*Jumpingアニメーションしているかどうか*/
    [SerializeField] private bool fall;             /*Fallアニメーションしているかどうか*/
    [SerializeField] private bool landing;          /*Landingアニメーションしているかどうか*/

    /*脳筋式レイキャスト変数*/
    public Ray ray;
    public Ray ray2;
    public Ray ray3;
    public Ray ray4;
    public Ray ray5;
    public Ray ray6;
    public RaycastHit rayHit;

    public Vector3 rayPosition; /*レイキャストの位置*/

    /*レイキャスト修正版*/
    public Transform legtrans;
    public Transform legtrans2;
    public Transform legtrans3;
    public Transform legtrans4;
    public Transform legtrans5;
    public Transform legtrans6;

    public Vector3 legposition;
    public Vector3 legposition2;
    public Vector3 legposition3;
    public Vector3 legposition4;
    public Vector3 legposition5;
    public Vector3 legposition6;
    /*レイキャスト修正版*/

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.5591f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        change_shoes = GetComponent<ChangeShoes>();
        animator = GetComponent<Animator>();

    }
    void FixedUpdate()
    {
        var velocity_y = rb.velocity.y;

        y = velocity_y;
        //Debug.Log(y);

        /*レイキャスト修正版*/
        legposition = legtrans.position;
        legposition2 = legtrans2.position;
        legposition3 = legtrans3.position;
        legposition4 = legtrans4.position;
        legposition5 = legtrans5.position;
        legposition6 = legtrans6.position;

        /*レイキャストの位置,レイキャストの角度*/
        ray = new Ray(legposition, -legtrans.up);
        ray2 = new Ray(legposition2, -legtrans2.up);
        ray3 = new Ray(legposition3, -legtrans3.up);
        ray4 = new Ray(legposition4, -legtrans4.up);
        ray5 = new Ray(legposition5, -legtrans5.up);
        ray6 = new Ray(legposition6, -legtrans6.up);

        /*デバッグ用の可視光線*/
        Debug.DrawRay(legposition, ray.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition2, ray2.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition3, ray3.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition4, ray4.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition5, ray5.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition6, ray6.direction * rayDistance, Color.green);
        /*レイキャスト修正版*/

        /*脳筋式レイキャストの当たり判定処理*/
        if ((Physics.Raycast(ray, out rayHit, rayDistance) || Physics.Raycast(ray2, out rayHit, rayDistance)
            || Physics.Raycast(ray3, out rayHit, rayDistance) || Physics.Raycast(ray4, out rayHit, rayDistance)
            || Physics.Raycast(ray5, out rayHit, rayDistance)) && (rayHit.collider.tag == "ground" || rayHit.collider.tag == "kuttuku"))
        {

            switch (change_shoes.type)
            {
                case ShoesType.Jump_Shoes:
                    doubleJump = 2;
                    break;

                case ShoesType.Speed_Shoes:
                    doubleJump = 1;
                    break;

                case ShoesType.Magnet_Shoes:
                    doubleJump = 1;
                    break;
            }

            /*コライダーを持つオブジェクトから、タグを読み取る（地面オブジェクトをgroundに設定）*/
            if (rayHit.collider.tag == "ground" || rayHit.collider.tag == "kuttuku")
            {
                animator.SetBool("IsGrounded", true);
                isGrounded = true;
            }
        }/*地面から離れたら*/
        else if (isGrounded)
        {
            if (doubleJump == 2) --doubleJump;//ジャンプ回数を減らす
            animator.SetBool("IsGrounded", false);
            isGrounded = false;
        }
        Animation();
    }


    private void Animation()
    {
        /*地面と接触していてAボタンが押されたら（ジャンプモーション）*/
        if (isGrounded && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            animator.SetBool("Jumping", true);
        }
        /*ジャンプ上昇中でキャラクターのｙ方向の速度がマイナスになったら（Fallモーション）*/
        else if (jumping && y < 0)
        {
            fall = true;
            jumping = false;
            animator.SetBool("Fall", true);
            animator.SetBool("Jumping", false);
        }
        /*落下中で地面と接触したら（Landingモーション）*/
        else if (fall && isGrounded)
        {
            landing = true;
            fall = false;
            animator.SetBool("Landing", true);
            animator.SetBool("Fall", false);
        }
        else if (!isGrounded && y < 0 && !jumping)
        {
            fall = true;
            animator.SetBool("Fall", true);
        }
        /*Landingモーションをしたら*/
        else if (landing)
        {
            landing = false;
            animator.SetBool("Landing", false);
        }
    }

    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    override public void Jump()
    {
        //switch (change_shoes.type)
        //{
        //    /*ジャンプシューズ*/
        //    case ShoesType.Jump_Shoes:
        //        if (doubleJump == 2 && isGrounded)
        //        {
        //            jumping = true;
        //            --doubleJump;
        //            rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
        //            rb.velocity = Vector3.zero;
        //        }
        //        else if (doubleJump >= 1 && !isGrounded)
        //        {
        //            --doubleJump;
        //            rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
        //            rb.velocity = Vector3.zero;
        //        }
        //        break;

        //    /*ダッシュシューズ*/
        //    case ShoesType.Speed_Shoes:
        //        if (doubleJump == 1 && isGrounded)
        //        {
        //            jumping = true;
        //            --doubleJump;
        //            rb.AddRelativeForce(rb.velocity.x, jumpForce, 0, ForceMode.VelocityChange);
        //            rb.velocity = Vector3.zero;
        //        }
        //        break;

        //    /*マグネットシューズ*/
        //    case ShoesType.Magnet_Shoes:
        //        if (doubleJump == 1 && isGrounded)
        //        {
        //            jumping = true;
        //            --doubleJump;
        //            rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
        //            rb.velocity = Vector3.zero;
        //        }
        //        break;
        //}        
            /*ジャンプシューズ*/
            if (doubleJump == 2 && isGrounded)
            {
                jumping = true;
                --doubleJump;
                rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
            }
            else if (doubleJump >= 1 && !isGrounded)
            {
                --doubleJump;
                rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
            }

            if (doubleJump == 1 && isGrounded)
            {
                jumping = true;
                --doubleJump;
                rb.AddRelativeForce(rb.velocity.x, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
            }

            if (doubleJump == 1 && isGrounded)
            {
                jumping = true;
                --doubleJump;
                rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
            }
        
    }
}
