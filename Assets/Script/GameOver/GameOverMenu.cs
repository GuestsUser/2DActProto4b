using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    cursor_move_type move_type;

    /* 【PauseMenuのオブジェクト格納用変数】 */
    [Header("ゲームオーバーメニュー")] [SerializeField] private GameObject gameover_menu;
    [Header("ゲームオーバー選択肢")] [SerializeField] private GameObject[] item_obj;
    [Header("カーソル")] [SerializeField] private GameObject _selector_obj;

    /* 【フラグ】 */
    [SerializeField] public bool show_menu;     /* true:表示 false:非表示 */
    [SerializeField] private bool push;         /* true:左スティックを押しています false:押していません */
    [SerializeField] private bool press_a;      /*ゲームパッドのAボタンが押されたかどうか*/
    [SerializeField] private bool push_scene;   /*シーン遷移を伴う決定をしたかどうか（なくせるかも）*/

    /* int型 */
    [Tooltip("メニューの上から何番目か")] [SerializeField] int menu_number;
    [Tooltip("左スティックがどれだけ長押しされているかカウントする")] [SerializeField] int count;
    [Tooltip("countと比較するため")] [SerializeField] int interval;
    int WaitTime = 3;   /*待ち時間*/

    /*画像切り替え用*/
    [SerializeField] private RawImage Cursor;

    void Start()
    {
        gameover_menu.SetActive(false); /*ゲームオーバーメニューは最初表示しない*/

        /* 【選択番号の初期化】 */
        menu_number = 0;
        interval = 15;

        /* 【フラグの初期化】 */
        show_menu = false;
        push = false;
        push_scene = false;
        push = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (show_menu)
        {
            Time.timeScale = 0;
            gameover_menu.SetActive(true);

            Cursor_Move();
            ChangeCursor();
            Decision();
            _selector_obj.transform.position = item_obj[menu_number].transform.position;
        }
        else
        {
            Time.timeScale = 1;
            gameover_menu.SetActive(false);
        }
    }
 
    void Cursor_Move()
    {
        if (push_scene == false)
        {
            if (Gamepad.current.leftStick.y.ReadValue() > 0)
            {
                move_type = cursor_move_type.up;
            }
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
                /* 【スティックがニュートラルから押された時】 */
                if (push == false)
                {
                    push = true;
                    if (--menu_number < 0) menu_number = item_obj.Length - 1;
                }
                else
                {
                    count--;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (--menu_number < 0) menu_number = item_obj.Length - 1;
                    }
                }

                break;

            case cursor_move_type.down:
                if (push == false)
                {
                    push = true;
                    if (++menu_number > item_obj.Length - 1) menu_number = 0;
                }
                else
                {
                    count++;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (++menu_number > item_obj.Length - 1) menu_number = 0;
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
                Cursor.texture = Resources.Load<Texture2D>("Cursor1");
                break;

            case 1:
                /* 【選択されているitemの文字を非表示に】 */
                item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                item_obj[0].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                Cursor.texture = Resources.Load<Texture2D>("BacktoTitle");
                break;
        }
    }

    /*シーン切り替え用*/
    void Decision()
    {
        if (Gamepad.current.buttonSouth.isPressed && press_a == false)
        {
            press_a = true;
            switch (menu_number)
            {
                case 0: /* ゲームを続ける */
                    StartCoroutine("BacktoStageSelect");
                    break;

                case 1: /* ゲームを終わる */
                    StartCoroutine("BacktoTitle");
                    break;
            }
        }

    }

    /*ステージセレクト画面に戻る*/
    private IEnumerator BacktoStageSelect()
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        SceneManager.LoadScene("StageSelect"); //おそらくタイトルに戻る実装する場合 Scene名はTitleなのでここはそのままにしてあります
        menu_number = 0; //メニュー番号を初期化。
        Time.timeScale = 1; //タイムスケールを初期化
    }

    /*タイトル画面に戻る*/
    private IEnumerator BacktoTitle()
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(WaitTime);

        SceneManager.LoadScene("Title");
        menu_number = 0;
        Time.timeScale = 1;
    }
}
