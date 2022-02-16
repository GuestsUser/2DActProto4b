using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StageSelectBack : MonoBehaviour
{
    /*仮：ステージセレクトに戻るとき表記するテキスト入れるオブジェクト*/
    public GameObject stageselectback_object = null;

    void Start()
    {

    }


    void Update()
    {
        /*ゲームパッドのBボタンが押されたら*/
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            /*ステージセレクト画面に切り替える*/
            SceneManager.LoadScene(1);
        }

        /*操作ボタンが分かるように仮表示*/
        Text stageselectback_text = stageselectback_object.GetComponent<Text>();
        stageselectback_text.text = "Bボタンでステージセレクト画面に戻ります";
    }
}
