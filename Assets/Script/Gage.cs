using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class Gage : MonoBehaviour
{

    [SerializeField] private ChangeShoes change_shoes;

    Image fillImg;
    float timeAmt = 5;  /*ゲージの稼働時間*/
    float time;
    public bool GaugeFlg; /*ゲージのフラグ*/

    void Start()
    {
        //change_shoes = GetComponent<ChangeShoes>();

        fillImg = this.GetComponent<Image>();   /*ゲージの画像*/
        time = timeAmt;
    }

    void Update()
    {
        if (change_shoes.type == ShoesType.Speed_Shoes)
        {
            //Debug.Log("ダッシュ靴");
            //if ((GaugeFlg) /*&& time > 0*/)
            //{
            //    time -= Time.deltaTime;
            //    fillImg.fillAmount = time / timeAmt;
            //}

            if (Gamepad.current.buttonWest.wasPressedThisFrame)//追加
            {
                GaugeFlg = true;
            }

            //if (fillImg.fillAmount == 0f)
            //{
            //    Debug.Log("aaaaaaa");
            //    GaugeFlg = false;
            //    timeAmt = 5;
            //    fillImg.fillAmount = 1.0f;
            //    time += 5;
            //}
       }
        if ((GaugeFlg) /*&& time > 0*/)
        {
            time -= Time.deltaTime;
            fillImg.fillAmount = time / timeAmt;
        }

        //if (Gamepad.current.buttonWest.wasReleasedThisFrame)//追加
        //{
        //    GaugeFlg = true;
        //}

        if (fillImg.fillAmount == 0f)
        {
            Debug.Log("aaaaaaa");
            GaugeFlg = false;
            timeAmt = 5;
            fillImg.fillAmount = 1.0f;
            time += 5;
        }

    }
}