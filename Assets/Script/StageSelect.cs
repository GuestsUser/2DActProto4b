using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  //シーンマネジメントを有効にする
using UnityEngine.InputSystem;
using Cinemachine;

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
    [Tooltip("追従対象切り替え用")] [SerializeField] CinemachineVirtualCamera vcam;

    private void Start()
    {
        //fade = GetComponent<Fade>();
    }
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Stage1")
        {
            ControlStop(); /* 入力受付終了 */
            /*ステージ1に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(2));
        }
        else if (collision.gameObject.name == "Stage2")
        {
            ControlStop();
            /*ステージ2に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(9));
        }
        else if (collision.gameObject.name == "Stage3")
        {
            ControlStop();
            /*ステージ3に切り替える*/
            fade.FadeIn(1.5f, () => SceneManager.LoadScene(3));
        }

        void ControlStop() /* 入力を処理するコンポーネント全停止で入力受付終了とする */
        {
            vcam.Follow = collision.gameObject.transform; /* カメラの追跡対象をワープゾーンに切り替える */
            ObjAllInVisible(gameObject); /* スクリプトを付けたオブジェクトの子から孫まで全て不可視化 */

            /* 入力を処理してるコンポーネント全停止 */
            GetComponent<PlayerMove>().enabled = false;
            GetComponent<JumpSystem>().enabled = false;
            GetComponent<DashSystem>().enabled = false;
            GetComponent<Kuttuku>().enabled = false;
            GetComponent<ChangeShoes>().enabled = false;
        }
        void ObjAllInVisible(GameObject obj) /* objに入れたオブジェクトの子から孫まで全てを不可視化する関数 */
        {
            foreach (Transform child in obj.transform) /* objの子全てに処理(孫まで検知しないので再起式を用いて孫の処理をする) */
            {
                if (child.GetComponent<Renderer>() != null) { child.GetComponent<Renderer>().enabled = false; } /* レンダラーコンポーネントが付いてたらfalseにして不可視化 */
                ObjAllInVisible(child.gameObject); /* 再帰式にする事で子から孫まで全てを取得できる */
            }
        }
    }
}
