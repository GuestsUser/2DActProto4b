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
        if (Gamepad.current.leftStick.x.ReadValue() > 0 || (Gamepad.current.dpad.x.ReadValue() == 1))
        {
            right = Gamepad.current.leftStick.x.ReadValue();
        }
        else
        {
            right = 0;
        }

        if (Gamepad.current.leftStick.x.ReadValue() < 0 || (Gamepad.current.dpad.x.ReadValue() == -1))
        {
            left = Gamepad.current.leftStick.x.ReadValue();
        }
        else
        {
            left = 0;
        }

        
        if (input_abs <= 0.5f)
        {
            state = State.walk; /*歩き*/
            if(right != 0)
            {
                move_x = 2f;
            }
            else if(left != 0)
            {
                move_x = -2f;
            }
            
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
        
        Debug.Log(state);/*プレイヤーの状態*/
        Debug.Log(move_x);
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
        if (right != 0)
        {
            run_time += Time.deltaTime;
            if (run_time < eas_time)
            {
                move_x = Easing.ExpOut(run_time, eas_time, 2f, max_move_x);
            }
            else
            {
                move_x = max_move_x;
            }
            
        }
        else if (left != 0)
        {
            run_time += Time.deltaTime;
            if (run_time < eas_time)
            {
                move_x = ExpOut(run_time, eas_time, -2f, -max_move_x);
            }
            else
            {
                move_x = -max_move_x;
            }
        }
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
}
