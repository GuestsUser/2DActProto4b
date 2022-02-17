using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : Padinput
{
    float right;
    float left;
    float x {get{ return right + left; } }
    Vector3 move;
    public override void Move()
    {
        Debug.Log("通ってます");
        if (Gamepad.current.leftStick.x.ReadValue() == 1)
        {
            right = 1;
        }
        else
        {
            right = 0;
        }
        if (Gamepad.current.leftStick.x.ReadValue() == -1)
        {
            left = 1;
        }
        else
        {
            left = 0;
        }

        move = Gamepad.current.leftStick.ReadValue();
       
    }
    void Update()
    {
        const float Speed = 3f;
        transform.Translate(move * Speed * Time.deltaTime);
        
    }
}
