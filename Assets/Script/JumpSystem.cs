using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSystem : Padinput
{
    /*追加した部分*/
    [SerializeField] private ChangeShoes change_shoes;
    /*追加した部分*/

    /*ジャンプ用変数*/
    [SerializeField] public float jumpForce = 5.0f; /*ジャンプ力*/
    [SerializeField] public int dubleJump = 0;      /*ジャンプ回数*/
    [SerializeField] public bool jump = false;      /*ジャンプフラグ*/
    /*ジャンプ用変数*/

    /*レイキャスト用変数*/
    public Vector3 rayPosition; /*レイキャストの位置*/

    /*脳筋式レイキャスト変数*/
    public Ray ray;
    public Ray ray2;
    public Ray ray3;
    public Ray ray4;
    public Ray ray5;
    public RaycastHit rayHit;

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.5591f;


    /*レイキャスト用変数*/

    public Rigidbody rb;    /*みんな大好きリジッドボディー*/

    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }

    void FixedUpdate()
    {

        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */
        rayPosition = rb.transform.position;/*レイキャストの位置*/
        /*レイキャストの位置,レイキャストの角度*/
        ray = new Ray(rayPosition, transform.up * -1f);
        ray2 = new Ray(rayPosition, new Vector3(0.5f, -1f, 0));
        ray3 = new Ray(rayPosition, new Vector3(0, -1f, 0.5f));
        ray4 = new Ray(rayPosition, new Vector3(-0.5f, -1f, 0));
        ray5 = new Ray(rayPosition, new Vector3(0, -1f, -0.5f));
        /*デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray2.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray3.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray4.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray5.direction * rayDistance, Color.red);

        print(jump);/*デバッグログ,ジャンプフラグを出力*/



        /*脳筋式レイキャストの当たり判定処理*/
        if (Physics.Raycast(ray, out rayHit, rayDistance) || Physics.Raycast(ray2, out rayHit, rayDistance)
            || Physics.Raycast(ray3, out rayHit, rayDistance) || Physics.Raycast(ray4, out rayHit, rayDistance)
            || Physics.Raycast(ray5, out rayHit, rayDistance))
        {
            /*追加した部分*/
            switch (change_shoes.type)
            {
                case ShoesType.Jump_Shoes:
                    dubleJump = 2;
                    break;

                case ShoesType.Speed_Shoes:
                    dubleJump = 1;
                    break;

                case ShoesType.Magnet_Shoes:
                    dubleJump = 1;
                    break;
            }
            Debug.Log(dubleJump);
            /*追加した部分*/

            /*コライダーを持つオブジェクトから、タグを読み取る（地面オブジェクトをgroundに設定）*/
            if (rayHit.collider.tag == "ground")
            {

                //dubleJump = 2;  /*残りジャンプ回数*/
                jump = true;    /*ジャンプフラグ*/

            }

        }/*地面から離れたら*/
        else if (jump)
        {
            /*コメントアウトした部分*/
            //dubleJump = 1;
            if (dubleJump == 2) --dubleJump;//ジャンプ回数を減らす
            /*コメントアウトした部分*/

            jump = false;   /*ジャンプフラグをfalseにする*/

        }

    }

    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    override public void Jump()
    {
        /*追加した部分*/
        switch (change_shoes.type)
        {
            case ShoesType.Jump_Shoes:
                /*PadInputを継承した処理*/
                /*オーバーライドした関数呼び出し*/
                /*残りジャンプ回数によりジャンプさせる*/
                if (dubleJump == 2 && jump)
                {

                    --dubleJump;

                    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                else if (dubleJump >= 1 && !jump)
                {

                    --dubleJump;

                    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                break;

            case ShoesType.Speed_Shoes:
                /*追加した部分*/
                if (dubleJump == 1 && jump)
                {

                    --dubleJump;

                    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                /*追加した部分*/
                break;

            case ShoesType.Magnet_Shoes:
                /*追加した部分*/
                if (dubleJump == 1 && jump)
                {

                    --dubleJump;

                    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
                    rb.velocity = Vector3.zero;
                }
                /*追加した部分*/
                break;
        }
        //Debug.Log("ジャンプ");
        //if (dubleJump == 1) --dubleJump;//ジャンプ回数を減らす
        /*追加した部分*/

        ///*PadInputを継承した処理*/
        ///*オーバーライドした関数呼び出し*/
        ///*残りジャンプ回数によりジャンプさせる*/
        //if (dubleJump == 2 && jump)
        //{

        //    --dubleJump;

        //    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        //    rb.velocity = Vector3.zero;
        //}
        //else if (dubleJump >= 1 && !jump)
        //{

        //    --dubleJump;

        //    rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        //    rb.velocity = Vector3.zero;
        //}

    }
}
