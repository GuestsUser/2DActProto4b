using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class TestPlayerMove : Padinput
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

    /*Kuttukuで使うプレイヤーポジション*/
    //[SerializeField] GameObject player;
    //public Vector3 player_pos {get{ return player.transform.position; } }
    
    

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.2f;

    public float height;

    private void Start()
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
    }
    public override void Move()
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
    public override void MoveStop()
    {
        
        state = State.idle; /*止まっている*/
        move_x = 0;
        move = Vector3.zero;
    }
    void Update()
    {
        //rayPosition = transform.position /*+ new Vector3(0,1,0)*/;
        //player_front = new Ray(rayPosition, transform.right * ray_direction);
        //Debug.DrawRay(rayPosition, player_front.direction * rayDistance, Color.blue);

        //if(kuttuku.collider_exit == true)
        //{
        //    move_x = 5;
        //}

        if (state == State.idle)
        {
            idle = true;
        }
        else
        {
            idle = false;
        }
        ApplyAnimator();
        if (kuttuku.bool_ray_hit == false)
        {
            /*下の処理の効果：くっついた際にプレイヤーが下を向いてしまう問題を改善*/
            /*3月7日追加部分*/
            bool_left_direction = true;
            bool_right_direction = true;
            /*3月7日追加部分*/

            if (right != 0)
            {
                player_direction = Quaternion.Euler(0, 0, 0);
                transform.localRotation = player_direction;
            }
            else if (left != 0)
            {
                player_direction = Quaternion.Euler(0, 180, 0);
                transform.localRotation = player_direction;
            }
        }
        else if(kuttuku.bool_ray_hit == true)
        {
            Debug.Log("今はくっついてます");
            

            if (right != 0)
            {
                bool_left_direction = false;
                if (bool_right_direction == false)
                {
                    Quaternion rot = Quaternion.AngleAxis(180, Vector3.up);
                    Quaternion q = this.transform.localRotation;

                    this.transform.localRotation = q * rot;
                    bool_right_direction = true;

                }
                

            }
            else if (left != 0)
            {
                bool_right_direction = false;
                if (bool_left_direction == false)
                {
                    if (this.transform.localRotation.y != -90f)
                    {
                        Quaternion rot = Quaternion.AngleAxis(180, Vector3.up);
                        Quaternion q = this.transform.localRotation;

                        this.transform.localRotation = q * rot;
                        bool_left_direction = true;
                    }
                    
                }
                

            }
        }

        Debug.Log(state);/*プレイヤーの状態*/
        //Debug.Log(right);
        //Debug.Log(transform.localRotation.y);
        //Debug.Log(input_abs);
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
        run_time += Time.deltaTime;
        if (run_time < eas_time)
        {
            move_x = ExpOut(run_time, eas_time, 2f, max_move_x);
        }
        else
        {
            move_x = max_move_x;
        }
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
    void ApplyAnimator()
    {
        //animator.SetFloat("Speed", move_x, 0.1f, Time.deltaTime);
    }
}
