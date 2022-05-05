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
    public MenuSE menuse;
    private bool se = false;

    int WaitTime = 1;

    void Start()
    {
        se = false;
    }


    void Update()
    {
        ///*ゲームパッドのAボタンが押されたら*/
        //if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        //{
        //    /*ステージセレクト画面に切り替える*/
        //    SceneManager.LoadScene(1);
        //}
        /*ゲームパッドのAボタンが押されたら*/
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (!se)
            {
                se = true;
                menuse.audio_source.clip = menuse.decision;
                menuse.audio_source.PlayOneShot(menuse.decision);

            }
            ///*ステージセレクト画面に切り替える*/
            //SceneManager.LoadScene(1);

            StartCoroutine("BacktoTitle");
        }

        /*操作ボタンが分かるように仮表示*/
        Text stageselectback_text = stageselectback_object.GetComponent<Text>();
        stageselectback_text.text = "Aボタンでステージセレクト画面に戻ります";
    }

    private IEnumerator BacktoTitle() //シーンチェンジ用
    {
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        /*ステージセレクト画面に切り替える*/
        SceneManager.LoadScene(1);

    }


}
