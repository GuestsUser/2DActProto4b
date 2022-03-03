using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //シーンマネジメントを有効にする
using UnityEngine.InputSystem;

public class StageSelect : MonoBehaviour
{

    //private int stageNumber;    /*タイトル画面のセレクト番号*/

    ////追加
    //private bool pushFlg;

    //public GameObject stagenumber_object = null;    /*仮：ステージ表記*/

    //void Start()
    //{
    //    stageNumber = 1;    /*ステージ1固定*/
    //    pushFlg = false;
    //}

    //void Update()
    //{
    //    StageSelectCursol();

    //    /*（仮表記）何ステージかわかるように*/
    //    Text stagenumber_text = stagenumber_object.GetComponent<Text>();
    //    stagenumber_text.text = "ステージ : " + stageNumber;
    //}


    //private void StageSelectCursol()
    //{
    //    Debug.Log("カーソル動きます");

    //    if (Gamepad.current.leftStick.up.ReadValue() == 1)
    //    {
    //        if (!pushFlg)
    //        {
    //            pushFlg = true;
    //            Debug.Log("1回だけ押した");
    //            if (--stageNumber < 1)
    //            {
    //                stageNumber = 3;
    //            }
    //        }
    //    }
    //    else if (Gamepad.current.leftStick.down.ReadValue() == 1)
    //    {
    //        if (!pushFlg)
    //        {
    //            pushFlg = true;
    //            Debug.Log("1回だけ押した");
    //            if (++stageNumber > 3)
    //            {
    //                stageNumber = 1;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        pushFlg = false;
    //    }

    //    /*ゲームパッドのBボタンを押したら*/
    //    if (Gamepad.current.buttonEast.wasPressedThisFrame)
    //    {
    //        SceneChenge();
    //    }
    //}

    ///*stageNumberに応じてシーンを切り替える関数*/
    //private void SceneChenge()
    //{
    //    switch (stageNumber)
    //    {
    //        case 1:
    //            /*ステージ1に切り替える*/
    //            SceneManager.LoadScene(2);
    //            break;
    //        case 2:
    //            /*ステージ2に切り替える*/
    //            SceneManager.LoadScene(3);
    //            break;
    //    }
    //}


    /*new*/
    /*[SerializeField]用*/
    [SerializeField] Fade fade;

    private void Start()
    {

    }
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "PreStage1")
        {
            /*ステージ1に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(2));
        }
        else if (collision.gameObject.name == "Stage2")
        {
            /*ステージ2に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(3));
        }
        else if (collision.gameObject.name == "Stage3")
        {
            /*ステージ3に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(4));
        }
    }
}
