using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashSystem : Padinput
{

    public bool dashFlg = true;                         /*ダッシュアビリティ用フラグ(trueで発動)*/
    public float coolTime = 0;                          /*クールタイム(初期値は5秒)*/
    [SerializeField] public float dashForce = 5f;       /*ダッシュの強さ(初期値は５)*/
    [SerializeField] private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    public Rigidbody rb;

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        dashFlg = true;

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        /*ダッシュフラグがtrueだったら*/
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
        else
        {
            /*クールタイムを計測*/
            if (coolTime < 5f)
            {

                coolTime += Time.deltaTime;

            }
            else
            {
                coolTime = 0;   /*リセット*/
                dashFlg = true;/*フラグをtrueに*/

            }


        }


    }

    /*ダッシュシステム(ジャンプの向きをよこに変えただけ)*/
    public void Dush()
    {
        /*シューズタイプがSpeed_Shoesだったら　*/
        switch (change_shoes.type)
        {
            case ShoesType.Speed_Shoes:
                rb.AddForce(dashForce + rb.velocity.x, rb.velocity.y, 0, ForceMode.VelocityChange);
                rb.velocity = Vector3.zero;
                dashFlg = false;
                break;

        }

    }
}
