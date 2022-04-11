using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class gauge : MonoBehaviour
{
    public Image UIgauge; /*ゲージの画像イメージ*/
    public float countTime = 5.0f; /*ゲージの稼働時間*/
    public bool gaugeFlg;   /*ゲージのフラグ*/

    void Update()
    {

        if (Gamepad.current.buttonWest.wasPressedThisFrame)
        {
            gaugeFlg = true;                /*ゲージを動かすフラグをONに*/
            UIgauge.fillAmount = 0.0f;      /*ゲージの画像の表示を0に（0だと表示されない１だと全部表示される）*/
        }
        if (gaugeFlg)                       /*ゲージを動かすフラグがONになれば*/
        {
            UIgauge.fillAmount += 1.0f / countTime * Time.deltaTime;    /*fillAmountは1～0の間で動くので　1 / countTimeの時間で表示がMAXになるように(1 / 5 で5秒間)*/
        }
    }

}