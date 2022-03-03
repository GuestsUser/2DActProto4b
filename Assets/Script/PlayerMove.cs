using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

enum State /*プレイヤーの移動状態*/
{
    idle,
    walk,
    run,
}

public class PlayerMove : Padinput
{

    /*プレイヤーの移動状態*/
    State state;

    /*左右の入力値*/
    float right;
    float left;

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

    /*イージング用*/
    [SerializeField] private float eas_time = 2f;/*イージングをかける時間*/
    private float run_time;/*走り始めてからの時間*/

    private void Start()
    {
        max_move_x = 13f;
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

            //if(right != 0)
            //{
            //    transform.rotation = Quaternion.Euler(0, 0, 0);
            //}
            //else if(left != 0)
            //{
            //    transform.rotation = Quaternion.Euler(0, 180, 0);
            //}
            
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
        if (right != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (left != 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        /*追加部分*/
        //if (Gamepad.current.leftStick.x.ReadValue() > 0)
        //{
        //    //Quaternion.Lerp();

        //}
        //else if (Gamepad.current.leftStick.x.ReadValue() < 0)
        //{
        //    //transform.Rotate(new Vector3(0, 180, 0));
        //    transform.rotation = Quaternion.Euler(0, 180, 0);
        //}
        /*追加部分*/

        Debug.Log(state);/*プレイヤーの状態*/
        //Debug.Log(right);
        //Debug.Log(Gamepad.current.leftStick.x.ReadValue());
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
        transform.Translate((move/10) * speed * Time.deltaTime);
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
        /*追加部分*/

        //if (right != 0)
        //{
        //    run_time += Time.deltaTime;
        //    if (run_time < eas_time)
        //    {
        //        move_x = ExpOut(run_time, eas_time, 2f, max_move_x);
        //    }
        //    else
        //    {
        //        move_x = max_move_x;
        //    }
            
        //}
        //else if (left != 0)
        //{
        //    run_time += Time.deltaTime;
        //    if (run_time < eas_time)
        //    {
        //        move_x = ExpOut(run_time, eas_time, -2f, -max_move_x);
        //    }
        //    else
        //    {
        //        move_x = -max_move_x;
        //    }
        //}
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
}
