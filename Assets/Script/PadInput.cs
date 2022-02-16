using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestJump : MonoBehaviour
{
    /*PlayerInput型の変数を用意。入力をとれる*/
    PlayerInput input;

    void Awake()
    {
        TryGetComponent(out input);
    }
    
    void OnEnable() /*オブジェクトがアクティブになったとき*/
    {
        /*[]中は『Actions名』を入れる*/
        input.actions["Jump"].started += OnJump;
        input.actions["Special"].started += OnSpecial;
        input.actions["PlayerMove"].started += OnPlayerMove;
    }
    void OnDisable()
    {
        /*[]中は『Actions名』を入れる*/
        input.actions["Jump"].started -= OnJump;
        input.actions["Special"].started -= OnSpecial;
        input.actions["PlayerMove"].started -= OnPlayerMove;
    }

    /*継承先でoverrideして動作を設定*/
    public virtual void Jump() {; }
    public virtual void Special() {; }
    public virtual void PlayerMove() {; }
    public virtual void CursorMove() {; } /*選択しているくつの切り替え用*/

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump(); 
        //Debug.Log("Aボタン入力しました");
    }
    public void OnSpecial(InputAction.CallbackContext context)
    {
        Special();
        //Debug.Log("Xボタン入力しました");
    }
    public void OnPlayerMove(InputAction.CallbackContext context)
    {
        PlayerMove();

        /*↓これで右入力か左入力かをとれるよ(右入力:1.0,0.0 左入力:-1.0,0.0)*/　
        //var value = context.ReadValue<Vector2>();
        //Debug.Log(value);
    }
}
