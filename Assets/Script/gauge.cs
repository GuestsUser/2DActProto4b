using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class gauge : MonoBehaviour
{
    [Header("ゲージを動かすスクリプト（新）kuba")]

    [SerializeField] private ChangeShoes change_shoes;

    Image fillImg;
    float timeAmt = 5;  /*ゲージの稼働時間*/
    float time;
    public bool GaugeFlg = false; /*ゲージのフラグ*/

    void Start()
    {
        //change_shoes = GetComponent<ChangeShoes>();

        fillImg = this.GetComponent<Image>();   /*ゲージの画像*/
        time = timeAmt;
    }

    void Update()
    {


        if (Gamepad.current.buttonWest.wasPressedThisFrame && GaugeFlg == false)    /*Xボタンを押す＋フラグがfalseなら*/
        {
            GaugeFlg = true;        /*ゲージのフラグをONに*/
            fillImg.fillAmount = 0.0f;  /*ゲージの画像を非表示*/
        }


        if (GaugeFlg)   /*ゲージのフラグがtrueなら*/
        {
            fillImg.fillAmount += 1.0f / timeAmt * Time.deltaTime;  /*ゲージの画像を少しずつ表示*/
            /*fillAmountは1～0の間で動くので　1 / countTimeの時間で表示がMAXになるように(1 / 5 で5秒間)*/
        }

        if (fillImg.fillAmount == 1f)
        {
            /*Debug.Log("aaaaaaa");*/
            GaugeFlg = false;
            timeAmt = 5;
            //fillImg.fillAmount = 1.0f;
            time += 5;
        }

    }
}