using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SlopeDash : Padinput
{

    public bool dashFlg = true;                         /*ダッシュアビリティ用フラグ(trueで発動)*/
    public float coolTime = 0;                          /*クールタイム(初期値は5秒)*/
    [SerializeField] public float dashForce = 5f;       /*ダッシュの強さ(初期値は５)*/
    [SerializeField] private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    /*レイキャスト用変数*/
    public Vector3 rayPosition; /*レイキャストの位置*/

    public GameObject obj;

    /*脳筋式レイキャスト変数*/
    public Ray ray;
    public RaycastHit rayHit;

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 5f;

    public Rigidbody rb;
    [System.NonSerialized] public GameObject standSlopeObj; /*現在乗ってる滑る床オブジェクト*/

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        dashFlg = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */
        rayPosition = rb.transform.position;/*レイキャストの位置*/
        /*レイキャストの位置,レイキャストの角度*/
        ray = new Ray(rayPosition, transform.right);
        /*デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.red);
        if ((Physics.Raycast(ray, out rayHit, rayDistance)) && rayHit.collider.tag == "Enemy")
        {

            rayHit.collider.gameObject.SetActive(false);/*レイキャストに触れたenemyタグを持つオブジェクトは消えることになる*/

        }

        /*追加部分*/
        if (!dashFlg)
        {
            /*クールタイムを計測*/
            if (coolTime < 0.2f)
            {

                coolTime += Time.deltaTime;

            }
            else
            {
                coolTime = 0;   /*リセット*/
                dashFlg = true;/*フラグをtrueに*/

            }
        }
        /*追加部分*/
    }

    public override void Skill()
    {
        if (dashFlg)
        {
            /*アビリティ発動ボタンが押されたら*/
            if (dashFlg && Gamepad.current.buttonWest.wasPressedThisFrame)
            {

                /*ダッシュ発動（中身はジャンプの向きを変えただけ）*/
                Dush();

            }
            coolTime = 0;/*クールタイムリセット*/

        }
    }

    /*ダッシュシステム(ジャンプの向きをよこに変えただけ)*/
    public void Dush()
    {
        //Debug.Log(-dashForce + rb.velocity.x, rb.velocity.y, 0, ForceMode.VelocityChange);
        /*シューズタイプがSpeed_Shoesだったら　*/
        switch (change_shoes.type)
        {
            case ShoesType.Speed_Shoes:
                float dashVector = dashForce * Mathf.Cos(transform.rotation.eulerAngles.y * Mathf.Deg2Rad); /* 力と方向(x軸用)を持った数学的な意味のベクトル */
                if (standSlopeObj == null) /* 滑る床以外でのダッシュ */
                {
                    rb.AddForce(dashVector + rb.velocity.x, rb.velocity.y, 0, ForceMode.VelocityChange);
                }
                else /* 床角度に合わせたダッシュ、滑る床以外もこれを使えば滑らないけど傾斜のある床をスムーズにダッシュできるかも */
                {
                    float floorZRad = standSlopeObj.transform.rotation.eulerAngles.z * Mathf.Deg2Rad; /* 床のz傾き(ラジアン) */
                    rb.AddForce(dashVector * Mathf.Abs(Mathf.Cos(floorZRad)) + rb.velocity.x, dashVector * Mathf.Abs(Mathf.Sin(floorZRad)) + rb.velocity.y, 0, ForceMode.VelocityChange); /* ダッシュベクトルを床の傾きに合わせて加工する */
                }
                //rb.velocity = Vector3.zero;
                dashFlg = false;
                break;

        }

    }
}
