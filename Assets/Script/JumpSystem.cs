using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpSystem : Padinput
{
    //    /*追加した部分*/
    //    [SerializeField] private ChangeShoes change_shoes;
    //    /*追加した部分*/

    //    [SerializeField] Animator animator;

    //    /*ジャンプ用変数*/
    //    [SerializeField] public float jumpForce = 5.0f; /*ジャンプ力*/
    //    [SerializeField] public int dubleJump = 0;      /*ジャンプ回数*/
    //    [SerializeField] public bool jump = false;      /*ジャンプフラグ*/
    //    /*ジャンプ用変数*/

    //    /*レイキャスト用変数*/
    //    public Vector3 rayPosition; /*レイキャストの位置*/

    //    /*脳筋式レイキャスト変数*/
    //    public Ray ray;
    //    public Ray ray2;
    //    public Ray ray3;
    //    public Ray ray4;
    //    public Ray ray5;
    //    public Ray ray6;
    //    public RaycastHit rayHit;

    //    /*（レイキャスト）可視光線の長さ*/
    //    [SerializeField] public float rayDistance = 0.5591f;


    //    /*レイキャスト用変数*/

    //    public Rigidbody rb;    /*みんな大好きリジッドボディー*/

    //    void Start()
    //    {

    //        rb = GetComponent<Rigidbody>();
    //        change_shoes = GetComponent<ChangeShoes>();
    //        animator = GetComponent<Animator>();

    //    }

    //    void FixedUpdate()
    //    {

    //        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */
    //        rayPosition = rb.transform.position;/*レイキャストの位置*/
    //        //rayPosition = transform.localPosition;/*レイキャストの位置*/
    //        /*追加部分*/
    //        if(Physics.gravity == new Vector3(0, -9.8f, 0))/*通常時*/
    //        {
    //            rayPosition.y += 0.5f;
    //        }
    //        else if(Physics.gravity == new Vector3(0, 9.8f, 0))
    //        {
    //            rayPosition.y -= 0.5f;
    //        }
    //        /*追加部分*/
    //        /*レイキャストの位置,レイキャストの角度*/
    //        ray = new Ray(rayPosition, transform.up * -1f);
    //        ray2 = new Ray(rayPosition, new Vector3(0.5f, -1f, 0));
    //        ray3 = new Ray(rayPosition, new Vector3(0, -1f, 0.5f));
    //        ray4 = new Ray(rayPosition, new Vector3(-0.5f, -1f, 0));
    //        ray5 = new Ray(rayPosition, new Vector3(0, -1f, -0.5f));
    //        //ray6 = new Ray(rayPosition, new Vector3(transform.localPosition));
    //        /*デバッグ用の可視光線*/
    //        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.green);
    //        Debug.DrawRay(rayPosition, ray2.direction * rayDistance, Color.green);
    //        Debug.DrawRay(rayPosition, ray3.direction * rayDistance, Color.green);
    //        Debug.DrawRay(rayPosition, ray4.direction * rayDistance, Color.green);
    //        Debug.DrawRay(rayPosition, ray5.direction * rayDistance, Color.green);

    //        print(jump);/*デバッグログ,ジャンプフラグを出力*/



    //        /*脳筋式レイキャストの当たり判定処理*/
    //        if ((Physics.Raycast(ray, out rayHit, rayDistance) || Physics.Raycast(ray2, out rayHit, rayDistance)
    //            || Physics.Raycast(ray3, out rayHit, rayDistance) || Physics.Raycast(ray4, out rayHit, rayDistance)
    //            || Physics.Raycast(ray5, out rayHit, rayDistance)) && (rayHit.collider.tag == "ground" || rayHit.collider.tag == "kuttuku"))
    //        {
    //            /*追加した部分*/
    //            switch (change_shoes.type)
    //            {
    //                case ShoesType.Jump_Shoes:
    //                    dubleJump = 2;
    //                    break;

    //                case ShoesType.Speed_Shoes:
    //                    dubleJump = 1;
    //                    break;

    //                case ShoesType.Magnet_Shoes:
    //                    dubleJump = 1;
    //                    break;
    //            }
    //            Debug.Log(dubleJump);
    //            /*追加した部分*/

    //            /*コライダーを持つオブジェクトから、タグを読み取る（地面オブジェクトをgroundに設定）*/
    //            if (rayHit.collider.tag == "ground" || rayHit.collider.tag == "kuttuku")
    //            {

    //                //dubleJump = 2;  /*残りジャンプ回数*/
    //                jump = true;    /*ジャンプフラグ*/

    //            }

    //        }/*地面から離れたら*/
    //        else if (jump)
    //        {
    //            /*コメントアウトした部分*/
    //            //dubleJump = 1;
    //            if (dubleJump == 2) --dubleJump;//ジャンプ回数を減らす
    //            /*コメントアウトした部分*/

    //            jump = false;   /*ジャンプフラグをfalseにする*/

    //        }

    //    }

    //    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    //    override public void Jump()
    //    {
    //        /*追加した部分*/
    //        switch (change_shoes.type)
    //        {
    //            case ShoesType.Jump_Shoes:
    //                /*PadInputを継承した処理*/
    //                /*オーバーライドした関数呼び出し*/
    //                /*残りジャンプ回数によりジャンプさせる*/
    //                if (dubleJump == 2 && jump)
    //                {

    //                    --dubleJump;
    //                    animator.SetTrigger("Jump");
    //                    rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
    //                    rb.velocity = Vector3.zero;
    //                }
    //                else if (dubleJump >= 1 && !jump)
    //                {

    //                    --dubleJump;

    //                    animator.SetTrigger("DoubleJump");

    //                    rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
    //                    rb.velocity = Vector3.zero;
    //                }
    //                break;

    //            case ShoesType.Speed_Shoes:
    //                /*追加した部分*/
    //                if (dubleJump == 1 && jump)
    //                {

    //                    --dubleJump;
    //                    animator.SetTrigger("Jump");
    //                    rb.AddRelativeForce(rb.velocity.x, jumpForce, 0, ForceMode.VelocityChange);
    //                    rb.velocity = Vector3.zero;
    //                }
    //                /*追加した部分*/
    //                break;

    //            case ShoesType.Magnet_Shoes:
    //                /*追加した部分*/
    //                if (dubleJump == 1 && jump)
    //                {

    //                    --dubleJump;
    //                    animator.SetTrigger("Jump");
    //                    rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
    //                    rb.velocity = Vector3.zero;
    //                }
    //                /*追加した部分*/
    //                break;
    //        }

    //    }

    //}


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

    /*3/29テスト用*/
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
    /*3/29テスト用*/

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
        Debug.Log(y);


        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */
        //rayPosition = rb.transform.position;/*レイキャストの位置*/

        /*3/29テスト用*/
        legposition = legtrans.position;
        legposition2 = legtrans2.position;
        legposition3 = legtrans3.position;
        legposition4 = legtrans4.position;
        legposition5 = legtrans5.position;
        legposition6 = legtrans6.position;
        /*3/29テスト用*/

        //if (Physics.gravity == new Vector3(0, -9.8f, 0))/*通常時*/
        //{
        //    rayPosition.y += 0.5f;
        //}
        //else if (Physics.gravity == new Vector3(0, 9.8f, 0))
        //{
        //    rayPosition.y -= 0.5f;
        //}

        /*3/29テスト用*/
        if (Physics.gravity == new Vector3(0, -9.8f, 0))/*通常時*/
        {
            legposition2.x += 0.05f;
            legposition3.x -= 0.05f;
            legposition4.x += 0.05f;
            legposition6.x -= 0.05f;
        }
        else if (Physics.gravity == new Vector3(0, 9.8f, 0))
        {
            legposition2.z += 0.05f;
            legposition3.z -= 0.05f;
            legposition4.z += 0.05f;
            legposition6.z -= 0.05f;
        }
        /*3/29テスト用*/

        /*3/29テスト用*/
        /*レイキャストの位置,レイキャストの角度*/
        ray = new Ray(legposition, -legtrans.up);
        ray2 = new Ray(legposition2, -legtrans2.up);
        //legposition2.x += 0.05f;
        ray3 = new Ray(legposition3, -legtrans3.up);
        //legposition3.x -= 0.05f;
        ray4 = new Ray(legposition4, -legtrans4.up);
        ray5 = new Ray(legposition5, -legtrans5.up);
        //legposition4.x += 0.05f;
        ray6 = new Ray(legposition6, -legtrans6.up);
        //legposition6.x -= 0.05f;

        /*デバッグ用の可視光線*/
        Debug.DrawRay(legposition, ray.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition2, ray2.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition3, ray3.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition4, ray4.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition5, ray5.direction * rayDistance, Color.green);
        Debug.DrawRay(legposition6, ray6.direction * rayDistance, Color.green);
        /*3/29テスト用*/

        /*追加部分*/
        ///*レイキャストの位置,レイキャストの角度*/
        //ray = new Ray(rayPosition, transform.up * -1f);
        //ray2 = new Ray(rayPosition, new Vector3(0.5f, -1f, 0));
        //ray3 = new Ray(rayPosition, new Vector3(0, -1f, 0.5f));
        //ray4 = new Ray(rayPosition, new Vector3(-0.5f, -1f, 0));
        //ray5 = new Ray(rayPosition, new Vector3(0, -1f, -0.5f));
        ////ray6 = new Ray(rayPosition, new Vector3(transform.localPosition));
        ///*デバッグ用の可視光線*/
        //Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.green);
        //Debug.DrawRay(rayPosition, ray2.direction * rayDistance, Color.green);
        //Debug.DrawRay(rayPosition, ray3.direction * rayDistance, Color.green);
        //Debug.DrawRay(rayPosition, ray4.direction * rayDistance, Color.green);
        //Debug.DrawRay(rayPosition, ray5.direction * rayDistance, Color.green);

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
        switch (change_shoes.type)
        {
            /*ジャンプシューズ*/
            case ShoesType.Jump_Shoes:
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
                break;

            /*ダッシュシューズ*/
            case ShoesType.Speed_Shoes:
                if (doubleJump == 1 && isGrounded)
                {
                    jumping = true;
                    --doubleJump;
                    rb.AddRelativeForce(rb.velocity.x, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                break;

            /*マグネットシューズ*/
            case ShoesType.Magnet_Shoes:
                if (doubleJump == 1 && isGrounded)
                {
                    jumping = true;
                    --doubleJump;
                    rb.AddRelativeForce(0, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                break;
        }
    }
}
