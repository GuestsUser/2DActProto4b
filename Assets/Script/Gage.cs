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
        

            if (Gamepad.current.buttonWest.wasPressedThisFrame)//追加
            {
                GaugeFlg = true;
            }

          
        if (GaugeFlg)
        {
            fillImg.fillAmount = 0.0f;
            time -= Time.deltaTime;
            fillImg.fillAmount = time / timeAmt;
        }

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