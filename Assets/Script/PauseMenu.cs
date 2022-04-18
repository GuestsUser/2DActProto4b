using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] private bool press_a;           /**/
    [SerializeField] private bool push_scene;
    [SerializeField] private bool show_ope;
    private bool fade_in;
    private bool fade_out;
    [SerializeField] private bool check_scene; /* ポーズメニューのみ必要 true:ステージセレクトシーン false:ゲームシーン */
    [SerializeField] private bool full_screen; /* true:フルスクリーン false:ウィンドウモード */

    /* int型 */
    [SerializeField] int menu_number;
    [SerializeField] int count;
    [SerializeField] int interval;　　　   /* 入力長押し時のカーソルの動く速さが変わる(値が大きいほど遅くなる) */
    int WaitTime = 3;                      /* シーン遷移前の待機時間 */

    /* float型 */
    [SerializeField] float fade_intime;                       /* fadeの進行時間 */
    [SerializeField] float fade_outtime;
    [SerializeField] float eas_time;                        /* イージングにかける時間 */
    [SerializeField] float opacity;                           /* 透明度 */
    float max_opacity = 100f;                       /* 透明度のMAX値 */
    float min_opacity = 0;　　　　　　　　 /* 透明度の最低値 */
    
    /* 画像切り替え用 */
    //[SerializeField] private Image[] item_image;
    public RawImage Cursor;
    Image color;
    //Vector3 yellow = new Vector3(1,1,0);
    //Vector3 white = new Vector3(1, 1, 1);
    Color yellow = new Color(1, 1, 0);
    Color white = new Color(1, 1, 1);
    /* ゲーム画面を暗くする用 */
    [SerializeField] Image fade_panel;



    // Start is called before the first frame update
    void Start()
    {
        /* 【オブジェクトの取得】 */
        pause_menu = GameObject.Find("PauseMenu");
        _selector_obj = GameObject.Find("Cursor");
        operation = GameObject.Find("Operation");
        fade_panel = GameObject.Find("FadePanel").GetComponent<Image>();
        _item_obj[0] = GameObject.Find("Continue_the_game");
        _item_obj[1] = GameObject.Find("Operation_explanation");
        _item_obj[2] = GameObject.Find("Return_to_StageSelect");

        /* 【オブジェクトの非表示化】 */
        pause_menu.SetActive(false);
        operation.SetActive(false);

        /* 【カーソルの取得】 */
        Cursor = _selector_obj.GetComponent<RawImage>();
        Cursor.color = white;

        /* 【選択番号の初期化】 */
        menu_number = 0;
        interval = 15;
        opacity = 0;

        /* 【フラグの初期化】 */
        show_menu = false;
        push_scene = false;
        push = false;
        show_ope = false;
        fade_in = false;
        fade_out = false;

        /* 【時間系の初期化】 */
        eas_time = 7f;
        fade_intime = 0;
        fade_outtime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name == "StageSelect")
        {
            check_scene = true;
        }
        if (check_scene)
        {
            Text item2 = _item_obj[2].GetComponent<Text>();
            item2.text = "タイトルにもどる";
        }
        else
        {
            Text item2 = _item_obj[2].GetComponent<Text>();
            item2.text = "ステージセレクトにもどる";
        }
        /* 【FadePanelの透明度を変更する処理】 */
        Fade();

        /* 【メイン処理】 */
        if (show_menu) /* 表示判定がtrueの時 */
        {
            if (press_a == false)
            {
                Cursor.color = white;
            }

            Time.timeScale = 0; /* unity内の時間を止める */
            if (show_ope == false) /* 操作説明が非表示の状態なら */
            {
                pause_menu.SetActive(true); /* ポーズメニューを表示させる */

                if(push_scene == false) /* シーン遷移を伴う決定が押されていなければ */
                {
                    press_a = false; /* aボタンを押せるように初期化 */
                }

                /* 【カーソル処理】 */
                Cursor_Move(); /* カーソルを動かす処理 */
                ChangeCursor(); /* カーソルの画像を切り替える処理 */

                /* 【決定が押された時の処理】 */
                Decision();

                /* スクリーンモードの切り替え */
                if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                {
                    if (full_screen == false)
                    {
                        full_screen = true;
                        Screen.SetResolution(1920, 1080, true);
                    }
                    else
                    {
                        full_screen = false;
                        Screen.SetResolution(1920, 1080, false);
                    }
                }
            }
            else
            {
                pause_menu.SetActive(false); /* ポーズメニューの非表示化 */
            }

            /* 【操作説明が表示状態でBボタンが押された時】 */
            if (show_ope == true && Gamepad.current.buttonEast.isPressed)
            {
                /* 【フラグ判定切り替え】 */
                show_ope = false; 
                operation.SetActive(false); /* 操作説明パネル非表示化 */
            }

            /* 【カーソル位置を移動する為の処理】 */
            _selector_obj.transform.position = _item_obj[menu_number].transform.position;
        }
        else　/* 【表示判定がfalseの時】 */
        {
            Time.timeScale = 1; /* unityの時間を進める */
            pause_menu.SetActive(false); /* ポーズメニュー非表示化 */
        }
    }
    public override void Pause() /* スタートボタンが押された時に処理に入ります */
    {
        /* 【フラグ判定切り替え】 */
        if(show_menu == false) /* ポーズメニューの表示判定がfalseなら */
        {
            show_menu = true; /* trueに変更 */
        }
        else if(show_menu == true && show_ope == false) /* ポーズメニューが表示状態かつ操作説明が非表示状態なら */
        {
            show_menu = false; /* falseに変更 */
        }
    }
    void Cursor_Move()
    {
        if(push_scene == false) /* シーン遷移を伴う決定がされていない間 */
        {
            if (Gamepad.current.leftStick.y.ReadValue() > 0) /*Lスティック入力　↑方向時*/
            {
                move_type = cursor_move_type.up; /* move_type を up に設定 */
            }
            else if (Gamepad.current.leftStick.y.ReadValue() < 0) /*Lスティック入力　↓方向時*/
            {
                move_type = cursor_move_type.down; /* move_type を down に設定 */
            }
            else /* スティック入力がされていないとき */
            {
                move_type = cursor_move_type.none; /* move_type を none に設定 */
            }
        }
        else /* シーン遷移されるとき */
        {
            move_type = cursor_move_type.none; /* move_type を none に設定 */
        }
        

        switch (move_type)
        {
            case cursor_move_type.up: /* move_typeがupの時 */

                /* 【スティックがニュートラルから押された時】 */
                if (push == false) 
                {
                    /* 【フラグ判定切り替え】 */
                    push = true;

                    /* 【メニューナンバーを引く処理】(引くことによりカーソルが上に移動する 理由:127行目でtransformの代入でitem[menu_number]を使用している) */
                    if (--menu_number < 0) menu_number = _item_obj.Length - 1;
                }
                else
                {
                    count--;
                    if (Mathf.Abs(count) % interval == 0)
                    {
                        /* 【メニューナンバーを引く処理】(引くことによりカーソルが上に移動する 理由:127行目でtransformの代入でitem[menu_number]を使用している) */
                        if (--menu_number < 0) menu_number = _item_obj.Length - 1;
                    }
                }
                
                break;

            case cursor_move_type.down: /* move_typeがdownの時 */
                        if (push == false)
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

            case cursor_move_type.none: /* move_typeがnoneの時 */
                count = 0;
                push = false;
                break;
        }

    }
    void Decision()
    {
        if (Gamepad.current.buttonSouth.isPressed && press_a == false && show_ope == false)
        {
            press_a = true;
            //_selector_obj.GetComponent<Image>().color = new Color(1,1,0);
            Cursor.color = yellow; 
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
                    if (check_scene)
                    {
                        StartCoroutine("BacktoTitle");
                    }
                    else
                    {
                        StartCoroutine("BacktoStageSelect");
                    }
                    break;
            }
        }
        
    }
    private void Initialize()
    {
       
    }
    void ChangeCursor()
    {
        switch(menu_number)
        {
            case 0: /* 【itemの表示切替と画像切り替え】 */

                /* 【選択されているitemの文字を非表示に】 */
                _item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                _item_obj[1].SetActive(true);
                _item_obj[2].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                Cursor.texture = Resources.Load<Texture2D>("Cursor1");
                break;

            case 1:
                /* 【選択されているitemの文字を非表示に】 */
                _item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                _item_obj[0].SetActive(true);
                _item_obj[2].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                Cursor.texture = Resources.Load<Texture2D>("Cursor2");
                
                break;

            case 2:
                /* 【選択されているitemの文字を非表示に】 */
                _item_obj[menu_number].SetActive(false);

                /* 【選択されているitem以外の文字を表示する処理】 */
                _item_obj[0].SetActive(true);
                _item_obj[1].SetActive(true);

                /* 【カーソルの画像切り替え】 */
                if (check_scene)
                {
                    Debug.Log("タイトル画像になるはず");
                    Cursor.texture = Resources.Load<Texture2D>("BacktoTitle");
                }
                else if (check_scene == false)
                {
                    Cursor.texture = Resources.Load<Texture2D>("Cursor3");
                }
                break;
        }
    }
    void Fade()
    {
        fade_panel.color = new Color(0, 0, 0, opacity / max_opacity);
        if (show_menu && opacity < max_opacity /4)
        {

            fade_outtime = 0;
            fade_intime += 0.3333333f; /* フェード時間を進める */
            if(fade_intime < eas_time)
            {
                Debug.Log("フェードイン");
                opacity = ExpOut(fade_intime, eas_time, min_opacity, max_opacity / 4);
            }
            else
            {
                opacity = max_opacity / 4;
            }
        }
        else if(show_menu == false && opacity > min_opacity)
        {

            fade_intime = 0;
            fade_outtime += 0.3333333f; /* フェード時間を進める */
            if (fade_outtime < eas_time)
            {

                Debug.Log("フェードアウト");
                opacity = ExpOut(fade_outtime, eas_time, max_opacity / 4, min_opacity);
            }
            else
            {
                opacity = min_opacity;
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
    private IEnumerator BacktoTitle() //シーンチェンジ用
    {
        push_scene = true;
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        SceneManager.LoadScene("Title"); //おそらくタイトルに戻る実装する場合 Scene名はTitleなのでここはそのままにしてあります
        menu_number = 0; //メニュー番号を初期化。
        Time.timeScale = 1; //タイムスケールを初期化
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速処理に使う関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
}
