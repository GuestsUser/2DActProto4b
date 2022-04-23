using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowPause : Padinput
{
    [SerializeField] private PauseMenu pause_script;

    /* 【PauseMenuのオブジェクト格納用変数】 */
    [SerializeField] private GameObject pause_menu;

    /* 【フラグ】 */
    public bool show_menu;         /* true:表示 false:非表示 */
    //public bool _show_menu { get { return show_menu; } }
    // Start is called before the first frame update
    void Start()
    {
        pause_script = GameObject.Find("PauseSystem").GetComponent<PauseMenu>();

        /* 【オブジェクトの取得】 */
        pause_menu = GameObject.Find("PauseMenu");

        /* 【オブジェクトの非表示化】 */
        pause_menu.SetActive(false);

        /* 【フラグの初期化】 */
        show_menu = false;
    }

    // Update is called once per frame
    
    public override void Pause() /* スタートボタンが押された時に処理に入ります */
    {
        /* 【フラグ判定切り替え】 */
        if (show_menu == false) /* ポーズメニューの表示判定がfalseなら */
        {
            show_menu = true; /* trueに変更 */
            //menuSE.audio_source.PlayOneShot(menuSE.open_menu); /* メニューを表示する音 */
        }
        else if (show_menu == true && pause_script._show_ope == false) /* ポーズメニューが表示状態かつ操作説明が非表示状態なら */
        {
            show_menu = false; /* falseに変更 */
            //menuSE.audio_source.PlayOneShot(menuSE.close_menu); /* メニューを閉じる音 */
        }
    }
}
