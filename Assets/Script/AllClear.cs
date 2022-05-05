using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class AllClear : MonoBehaviour
{
    int WaitTime = 2;

    public MenuSE menuse;
    private bool se = false;

    void Start()
    {
        se = false;
    }


    void Update()
    {
        
        /*ゲームパッドのAボタンが押されたら*/
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (!se)
            {
                se = true;
                menuse.audio_source.clip = menuse.decision;
                menuse.audio_source.PlayOneShot(menuse.decision);

            }
            StartCoroutine("BacktoTitle");
        }

    }
    private IEnumerator BacktoTitle() //シーンチェンジ用
    {
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        SceneManager.LoadScene("Title"); //タイトルに戻る実装する場合 Scene名はTitleなのでここはそのままにしてあります
        
    }
}
