using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    cursor_move_type move_type;

    /*【タイトル画面用オブジェクト格納用変数】*/
    [Header("タイトル選択肢")] [SerializeField] private GameObject[] item_obj;
    [Header("カーソル")] [SerializeField] private GameObject selector_obj;

    /*【フラグ】*/
    [SerializeField] private bool push;         /*左スティックが押されたかどうか*/
    [SerializeField] private bool push_scene;   /**/
    [SerializeField] private bool press_a;      /*ゲームパッドのAボタンが押されたかどうか*/

    /*int型*/
    [SerializeField] private int menu_number;
    [SerializeField] private int count;
    [SerializeField] private int interval;

    /*画像切り替え用*/
    public RawImage Cursor;
    Color yellow = new Color(1,1,0);
    Color white = new Color(1,1,1);

    /* 【SE関連】 */
    public MenuSE menuSE; /* SEを扱うためのコンポネント */
    //public MenuSE _menuSE { get { return menuSE; } }

    bool se_flg; /* true:既にならした false:ならせます */
    void Start()
    {
        /*【オブジェクトの取得】*/
        selector_obj = GameObject.Find("Cursor");

        /*【選択番号の初期化】*/
        menu_number = 0;
        interval = 15;

        /*【フラグの初期化】*/
        push = false;
        push_scene = false;
        press_a = false;

        /* 【カーソルの色を白に初期化】 */
        Cursor.color = white;
    }

    // Update is called once per frame
    void Update()
    {
        /* 【シーン遷移を伴う決定がされていない間】 */
        if (push_scene == false)
        {
            /* 【カーソルの色を白に初期化】 */
            Cursor.color = white;
        }
        Cursor_Move();
        ChangeCursor();
        Decision();
        selector_obj.transform.position = item_obj[menu_number].transform.position;
    }

    void Cursor_Move()
    {
        if (push_scene == false)
        {
            /*もし左スティックが上に動いたら*/
            if (Gamepad.current.leftStick.y.ReadValue() > 0)
            {
                move_type = cursor_move_type.up;
            }
            /*もし左スティックが下に動いたら*/
            else if (Gamepad.current.leftStick.y.ReadValue() < 0)
            {
                move_type = cursor_move_type.down;
            }
            else
            {
                move_type = cursor_move_type.none;
            }
        }
        else
        {
            move_type = cursor_move_type.none;
        }

        switch (move_type)
        {
            case cursor_move_type.up:
                /*左スティックがニュートラルから押されたとき*/
                if (push == false)
                {
                    push = true;
                    if (--menu_number < 0) menu_number = item_obj.Length - 1;
                    menuSE.audio_source.clip = menuSE.move;
                    menuSE.audio_source.PlayOneShot(menuSE.move); /* カーソルが動く音 */
                }
                else
                {
                    count--;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (--menu_number < 0) menu_number = item_obj.Length - 1;
                        menuSE.audio_source.clip = menuSE.move;
                        menuSE.audio_source.PlayOneShot(menuSE.move); /* カーソルが動く音 */
                    }
                }
                break;

            case cursor_move_type.down:
                if (push == false)
                {
                    push = true;
                    if (++menu_number > item_obj.Length - 1) menu_number = 0;
                    menuSE.audio_source.clip = menuSE.move;
                    menuSE.audio_source.PlayOneShot(menuSE.move); /* カーソルが動く音 */
                }
                else
                {
                    count++;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (++menu_number > item_obj.Length - 1) menu_number = 0;
                        menuSE.audio_source.clip = menuSE.move;
                        menuSE.audio_source.PlayOneShot(menuSE.move); /* カーソルが動く音 */
                    }
                }
                break;

            case cursor_move_type.none:
                count = 0;
                push = false;
                break;
        }
    }

    void ChangeCursor()
    {
        switch (menu_number)
        {
            case 0: /* 【itemの表示切替と画像切り替え】 */

                /* 【選択されているitemの文字を非表示に】 */
                item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                item_obj[1].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                Cursor.texture = Resources.Load<Texture2D>("GameStartCursor");
                break;

            case 1:
                /* 【選択されているitemの文字を非表示に】 */
                item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                item_obj[0].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                Cursor.texture = Resources.Load<Texture2D>("EndGameCursor");
                break;
        }
    }


    void Decision()
    {
        if (Gamepad.current.buttonSouth.isPressed && press_a == false)
        {
            press_a = true;
            Cursor.color = yellow; /* カーソルの色を黄色に変更 */
            menuSE.audio_source.clip = menuSE.decision;
            menuSE.audio_source.PlayOneShot(menuSE.decision); /* カーソルが動く音 */
            switch (menu_number)
            {
                case 0:     /*ステージセレクト画面へ*/
                    StartCoroutine("StageSelectCoroutine");
                    break;

                case 1:     /*ゲーム終了*/
                    StartCoroutine("EndCoroutine");
                    break;
            }
        }
    }


    private IEnumerator StageSelectCoroutine() //シーンチェンジ用
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(3.0f);  //処理を待機
        SceneManager.LoadScene("StageSelect"); //おそらくタイトルに戻る実装する場合 Scene名はTitleなのでここはそのままにしてあります
        menu_number = 0; //メニュー番号を初期化
    }

    private IEnumerator EndCoroutine()
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(1.5f);
        /*エディタ上の場合*/
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        /*ビルド上の場合*/
        Application.Quit();
        #endif
    }
}
