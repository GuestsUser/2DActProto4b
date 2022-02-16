using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //シーンマネジメントを有効にする
using UnityEngine.InputSystem;

public class StageSelect : MonoBehaviour
{

    private int stageNumber;    /*タイトル画面のセレクト番号*/
    private bool pushFlag;      /*縦に押したときの判定*/

    public GameObject stagenumber_object = null;    /*仮：ステージ表記*/

    void Start()
    {
        stageNumber = 1;    /*ステージ1固定*/
        pushFlag = false;
    }

    void Update()
    {
        /*ゲームパッドのAボタンが押されたら*/
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (pushFlag == false)
            {
                /*押した判定にする*/
                pushFlag = true;
                Debug.Log("下を押した");
                /*選択が一番下に来たら*/
                if (++stageNumber > 2)
                    /*選択を一番上に戻す*/
                    stageNumber = 1;
            }
        }
        /*ゲームパッドのYボタンが押されたら*/
        else if (Gamepad.current.buttonNorth.wasPressedThisFrame)
        {

            if (pushFlag == false)
            {
                /*押した判定にする*/
                pushFlag = true;
                Debug.Log("上を押した");
                /*選択が一番上に来たら*/
                if (--stageNumber < 1)
                    /*選択を一番下に戻す*/
                    stageNumber = 2;
            }
        }
        /*上記2つが押されてないなら*/
        else
        {
            pushFlag = false;
        }

        /*ゲームパッドのBボタンを押したら*/
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            SceneChenge();
        }




        /*（仮表記）何ステージかわかるように*/
        Text stagenumber_text = stagenumber_object.GetComponent<Text>();
        stagenumber_text.text = "ステージ : " + stageNumber;
    }

    /*stageNumberに応じてシーンを切り替える関数*/
    private void SceneChenge()
    {
        switch (stageNumber)
        {
            case 1:
                /*ステージ1に切り替える*/
                SceneManager.LoadScene(2);
                break;
            case 2:
                /*ステージ2に切り替える*/
                SceneManager.LoadScene(3);
                break;
        }
    }
}
