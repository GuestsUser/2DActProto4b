using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum State /*プレイヤーの移動状態*/
{
    idle,
    walk,
    run,
}

public class PlayerMove : Padinput
{
    [SerializeField] Kuttuku kuttuku;
    /*プレイヤーの移動状態*/
    public State state;
    public bool idle;

    /*左右の入力値*/
    public float right;
    public float left;

    /*X軸の入力値*/
    float input_xAxis { get { return right + left; } }

    /*x軸の入力絶対値*/
    float input_abs { get { return System.Math.Abs(input_xAxis); } }

    /*実際にプレイヤーにかけるx値*/
    float move_x;
    /*move_xの最大値*/
    [SerializeField] private float max_move_x; /*通常時13fがいいかも 速くして15f～20f*/

    /*動きベクトルにかけるスピード値*/
    float speed = 5f;
    //float max_speed = 50f;

    /*動きのベクトル*/
    Vector3 move;

    Quaternion player_direction; //プレイヤーの方向
    bool bool_left_direction; //true:左に向いています
    bool bool_right_direction; //true:右に向いています

    /*イージング用*/
    [SerializeField] private float eas_time = 2f;/*イージングをかける時間*/
    private float run_time;/*走り始めてからの時間*/

    //アニメーション用
    [SerializeField] Animator animator;
    float anim_speed;

    /*Kuttukuで使うプレイヤーポジション*/
    //[SerializeField] GameObject player;
    //public Vector3 player_pos {get{ return player.transform.position; } }
    
    

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.2f;

    public float height;

    private void Start() /*初期化*/
    {
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);
        max_move_x = 13f;

        /*falseだとくっついた時にプレイヤーが下を向いてしまう*/
        bool_left_direction = true;
        bool_right_direction = true;
        /*falseだとくっついた時にプレイヤーが下を向いてしまう*/
        //move = Vector3.zero;
        idle = true;
        height = 0;

        anim_speed = 0.1f;
    }
    
    /*オーバーライド関数(自動で呼び出される 呼び出しタイミングはコントローラー割り当てがされているボタン、スティックが入力された時)*/
    public override void Move() /*動かす時の左スティック入力状態をここで取得*/
    {
        if (Gamepad.current.leftStick.x.ReadValue() > 0)
        {
            right = Gamepad.current.leftStick.x.ReadValue();
            left = 0;
        }
        else
        {
            right = 0;
        }

        if (Gamepad.current.leftStick.x.ReadValue() < 0)
        {
            left = Gamepad.current.leftStick.x.ReadValue();
            right = 0;
        }
        else
        {
            left = 0;
        }

        
        if (input_abs <= 0.5f)
        {
            state = State.walk; /*歩き*/

            move_x = 2f; /*プレイヤーを回転させれば符号を変える必要はない*/
            

        }
        else if(input_abs > 0.5f)
        {
            state = State.run; /*走り*/
            
        }
        

    }
    public override void MoveStop()/*プレイヤーの動きを止める処理*/
    {
        
        state = State.idle; /*止まっている*/
        move_x = 0;
        move = Vector3.zero;

        /*歩きアニメーションに戻すための処理*/
        anim_speed = 0.1f;
    }
    /*オーバーライド関数*/

    void Update() /*常に処理する内容*/
    {

        if (state == State.idle)
        {
            idle = true;
        }
        else
        {
            idle = false;
        }

        ApplyAnimator();
        RunAndTurn();
    }
    
    private void RunAndTurn() /*走る+振り向き処理*/
    {
        if (kuttuku.bool_ray_hit == false) /*くっつき状態ではない場合*/
        {
            /*下の処理の効果：くっついた際にプレイヤーが下を向いてしまう問題を改善*/
            bool_left_direction = true;
            bool_right_direction = true;

            if (right != 0)/*左スティックが右に入力されているとき*/
            {
                /*プレイヤーの向きを右に向いている状態にする処理*/
                player_direction = Quaternion.Euler(0, 0, 0); /*Quaternion.Eulerで向きを3軸(xyz)まとめて値を指定したものをプレイヤーの向きを入れる変数に代入*/
                transform.localRotation = player_direction; /*プレイヤーの向きをlocalRotationに代入して回転させる*/
            }
            else if (left != 0)/*左スティックが左に入力されているとき*/
            {
                /*プレイヤーの向きを左に向いている状態にする処理*/
                player_direction = Quaternion.Euler(0, 180, 0); /*Quaternion.Eulerで向きを3軸(xyz)まとめて値を指定したものをプレイヤーの向きを入れる変数に代入*/
                transform.localRotation = player_direction; /*プレイヤーの向きをlocalRotationに代入して回転させる*/
            }
        }
        else if (kuttuku.bool_ray_hit == true)/*くっつき状態の場合*/
        {
            if (right != 0) /*左スティックが右に入力されている場合*/
            {
                bool_left_direction = false; /*左に振り向く時に使うフラグをfalseに*/
                if (bool_right_direction == false) /*右に振り向く時に必要なフラグがfalseの場合*/
                {
                    /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
                    Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定 ※x軸:Vector3.right y軸:Vector3.up z軸:Vector3.foward*/
                    Quaternion q = this.transform.localRotation; /*これがないと現在の値から+〇度回転させた値にするということが出来ない*/
                    /*【回転させる準備】*/

                    /*localRotationに値を代入し、実際に回転させる*/
                    this.transform.localRotation = q * rot;/*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
                    bool_right_direction = true; /*プレイヤーが右に向いているときのフラグをtrueに*/

                }


            }
            else if (left != 0)/*左スティックが左に入力されている場合*/
            {
                bool_right_direction = false; /*プレイヤーが右に向いている時にtrueになるフラグをfalseに*/
                if (bool_left_direction == false) /*プレイヤーが左に向いているフラグがfalseの時*/
                {
                    if (this.transform.localRotation.y != -90f) /*もし、localRotation.yの値が-90fじゃない時*/
                    {
                        /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
                        Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定*/
                        Quaternion q = this.transform.localRotation; /*現在の値を保持*/
                        /*【回転させる準備】*/

                        /*localRotationに値を代入し、実際に回転させる*/
                        this.transform.localRotation = q * rot; /*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
                        bool_left_direction = true;/*プレイヤーが左に向いているときのフラグをtrueに*/
                    }

                }


            }
        }

        Debug.Log(state);/*プレイヤーの状態*/

        switch (state)
        {
            case State.idle: /*止まっている時*/
                run_time = 0;
                move_x = 0;
                break;

            case State.walk: /*歩き*/
                run_time = 0;
                break;

            case State.run: /*走り*/
                RunAccel();
                break;
        }

        move = new Vector3(move_x, 0, 0);

        if (state == State.idle)
        {
            move = Vector3.zero;
        }
        transform.Translate((move / 10) * speed * Time.deltaTime);


    }

    private void RunAccel() /*走る時の加速処理*/
    {
        /*追加部分*/ /*プレイヤーを回転させる場合これのみでok*/
        run_time += Time.deltaTime; /*run_timeを少しずつ動かす*/
        if (run_time < eas_time) /*緩急をつけるトータルの時間よりrun_timeが小さければまだ加速処理*/
        {
            move_x = ExpOut(run_time, eas_time, 2f, max_move_x); /*move_xに2f～max_move_xの値を入れる処理(加速)*/
            
            /*↓プレイヤーの加速処理とアニメーションを合わせる為に追加した処理*/
            anim_speed = ExpOut(run_time, eas_time, 0.1f, 1f); /*ブレンドツリーに代入するスピードの値を0.1f～１fで加速させる処理*/
        }
        else /*緩急をつける時間を越えたら*/
        {
            move_x = max_move_x; /*最大値を維持*/
            
            /*↓プレイヤーの加速処理とアニメーションを合わせる為に追加した処理*/
            anim_speed = 1f; /*走りのアニメーションを維持するために1を代入*/
        }
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速処理に使う関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
    void ApplyAnimator()
    {
        //var speed = Mathf.Abs(input_abs);
        //animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed", anim_speed, 0.1f, Time.deltaTime);

        //if (speed == 0)
        //{
        //    state = State.idle;
        //    Debug.Log("止まり状態");
        //}
        //else if (speed <= 0.7f)
        //{
        //    state = State.walk;
        //    Debug.Log("歩き状態");
        //}
        //else if (speed > 0.7f)
        //{
        //    state = State.run;
        //    Debug.Log("走り状態");
        //}
    }
}
