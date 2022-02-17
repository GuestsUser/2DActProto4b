using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : Padinput
{
    float right;
    float left;
    /*PlayerのX値*/
    float x {get{ return right + left; } }
    Vector3 move;
    public override void Move()
    {
        Debug.Log("通ってます");
        if (Gamepad.current.leftStick.x.ReadValue() == 1 || (Gamepad.current.dpad.x.ReadValue() == 1))
        {
            right = 1;
        }
        else
        {
            right = 0;
        }
        if (Gamepad.current.leftStick.x.ReadValue() == -1 || (Gamepad.current.dpad.x.ReadValue() == -1))
        {
            left = -1;
        }
        else
        {
            left = 0;
        }

       
        move = new Vector3(x,0,0);

    }
    public override void MoveStop()
    {
        move = Vector3.zero;
    }
    void Update()
    {
        const float Speed = 3f;
        transform.Translate(move * Speed * Time.deltaTime);
        //Debug.Log(Gamepad.current.dpad.x.ReadValue());
    }
}
