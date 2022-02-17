using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Padinput : MonoBehaviour
{
    /*PlayerInput型の変数を用意。入力をとれる*/
    PlayerInput input;
    
    public PlayerInput _input { get{ return input; } }

    void Awake()
    {
        TryGetComponent(out input);
    }
    
    void OnEnable() /*オブジェクトがアクティブになったとき*/
    {
        /*[]中は『Actions名』を入れる*/
        input.actions["Jump"].started += OnJump;
        input.actions["Skill"].started += OnSkill;
        input.actions["Move"].performed += OnMove;
        input.actions["Move"].canceled += OnMoveStop;
        input.actions["Change"].started += OnChange;
    }
    void OnDisable()
    {
        /*[]中は『Actions名』を入れる*/
        input.actions["Jump"].started -= OnJump;
        input.actions["Skill"].started -= OnSkill;
        input.actions["Move"].performed -= OnMove;
        input.actions["Move"].canceled -= OnMoveStop;
        input.actions["Change"].started -= OnChange;
    }

    /*継承先でoverrideして動作を設定*/
    public virtual void Jump() {; }
    public virtual void Skill() {; }
    public virtual void Move() {; }
    public virtual void MoveStop() {; }
    public virtual void Change() {; } /*選択しているくつの切り替え用*/

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump(); 
        Debug.Log("Aボタン入力しました");
    }
    public void OnSkill(InputAction.CallbackContext context)
    {
        Skill();
        Debug.Log("Xボタン入力しました");
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        Move();
        //Debug.Log("通ってます");
        /*↓これで右入力か左入力かをとれるよ(右入力:1.0,0.0 左入力:-1.0,0.0)*/　
        //var value = context.ReadValue<Vector2>();
        //Debug.Log(value);
    }
    public void OnMoveStop(InputAction.CallbackContext context)
    {
        MoveStop();
    }

    public void OnChange(InputAction.CallbackContext context)
    {
        Change();
    }
}
