using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーンマネジメントを有効にする
using UnityEngine.InputSystem;

public class Title : MonoBehaviour
{

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
    }
}