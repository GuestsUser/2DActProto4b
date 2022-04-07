using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

enum cursor_move_type
{
    up,
    down,
    none
}
public class PauseMenu : Padinput
{
    cursor_move_type move_type;
    /* 【PauseMenuのオブジェクト格納用変数】 */
    [SerializeField] private GameObject pause_menu;
    [SerializeField] private GameObject[] _item_obj;
    [SerializeField] private GameObject _selector_obj;
    [SerializeField] private GameObject operation;

    /* 【フラグ】 */
    [SerializeField] private bool show_menu;         /* true:表示 false:非表示 */
    public bool _show_menu {get { return show_menu; } }
    [SerializeField] private bool push;              /* true:左スティックを押しています false:押していません */
    [SerializeField] private bool press_a;
    [SerializeField] private bool push_scene;
    [SerializeField] private bool show_ope;

    /* int型 */
    [SerializeField] int menu_number;
    [SerializeField] int count;
    [SerializeField] int interval;
    int WaitTime = 3;


    // Start is called before the first frame update
    void Start()
    {
        /* 【オブジェクトの取得】 */
        pause_menu = GameObject.Find("PauseMenu");
        _selector_obj = GameObject.Find("Cursor");
        operation = GameObject.Find("Operation");

        pause_menu.SetActive(false);
        operation.SetActive(false);

        /* 【選択番号の初期化】 */
        menu_number = 0;
        interval = 10;

        /* 【フラグの初期化】 */
        show_menu = false;
        push_scene = false;
        push = false;
        show_ope = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (show_menu)
        {
            Time.timeScale = 0;
            if(show_ope == false)
            {
                pause_menu.SetActive(true);
                press_a = false;

                Cursor_Move();
                Decision();
            }
            else
            {
                pause_menu.SetActive(false);
            }

            

            if (show_ope == true && Gamepad.current.buttonEast.isPressed)
            {
                show_ope = false;
                operation.SetActive(false);
            }

            _selector_obj.transform.position = _item_obj[menu_number].transform.position;
        }
        else
        {
            Time.timeScale = 1;
            pause_menu.SetActive(false);
        }
    }
    public override void Pause() /* スタートボタンが押された時に処理に入ります */
    {
        /* 【フラグ判定切り替え】 */
        if(show_menu == false)
        {
            show_menu = true;
        }
        else
        {
            show_menu = false;
        }
    }
    void Cursor_Move()
    {
        if(push_scene == false)
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
                    if (--menu_number < 0) menu_number = _item_obj.Length - 1;
                }
                else
                {
                    count--;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (--menu_number < 0) menu_number = _item_obj.Length - 1;
                    }
                }
                
                break;

            case cursor_move_type.down:
                if(push == false)
                {
                    push = true;
                    if (++menu_number > _item_obj.Length - 1) menu_number = 0;
                }
                else
                {
                    count++;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        if (++menu_number > _item_obj.Length - 1) menu_number = 0;
                    }
                }
                break;

            case cursor_move_type.none:
                count = 0;
                push = false;
                break;
        }

    }
    void Decision()
    {
        if (Gamepad.current.buttonSouth.isPressed && press_a == false)
        {
            press_a = true;
            switch (menu_number)
            {
                case 0: /* ゲームを続ける */

                    press_a = false;
                    show_menu = false;

                    break;

                case 1: /* 操作説明 */
                    operation.SetActive(true);
                    show_ope = true;
                    break;

                case 2: /* ステージ選択に戻る */
                    StartCoroutine("BacktoStageSelect");
                    break;
            }
        }
        
    }
    private IEnumerator BacktoStageSelect() //シーンチェンジ用
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        SceneManager.LoadScene("StageSelect"); //おそらくタイトルに戻る実装する場合 Scene名はTitleなのでここはそのままにしてあります
        menu_number = 0; //メニュー番号を初期化。
        Time.timeScale = 1; //タイムスケールを初期化
    }
}
