using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectStage_Player : MonoBehaviour
{

    /*InputActionを格納するための変数*/
    private SelectStagePlayer playerActionsAsset;

    /*Moveを格納するための変数*/
    private InputAction move;

    /*Rigidbodyを格納する変数*/
    private Rigidbody rb;

    /*プレイヤーの最高速度*/
    [SerializeField] private float maxSpeed = 8f;

    /*プレイヤーが動く方向*/
    private Vector3 forceDirection = Vector3.zero;

    /*カメラ制御*/
    [SerializeField] private Camera playerCamera;

    /*アニメーター*/
    private Animator animator;

    /*プレイヤーの回転制御*/
    private Quaternion targetRotation;

    //trueなら動ける falseなら何も操作を受け付けない
    private bool canMove;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        /*初期化*/
        playerActionsAsset = new SelectStagePlayer ();

        /*動けるようにしておく*/
        canMove = true;
        /*現在の回転を取得*/
        targetRotation = transform.rotation;
    }

    /*ボタン入力と関数を結びつける*/
    /*このオブジェクトが表示になった時呼び出される*/
    private void OnEnable()
    {
        /*アタックボタンが押されたときに*/
        //playerActionsAsset.Player.Attack.started += Attack;
        /*ブロックボタンが押されたときに*/
        //playerActionsAsset.Player.Block.started += Block;
        /*ブロックボタンが離されたとき*/
       // playerActionsAsset.Player.Block.canceled += Block;

        /*変数にMoveアクションを格納*/
        move = playerActionsAsset.Player.Move;
        /*有効にする*/
        playerActionsAsset.Player.Enable();
    }

    /*このオブジェクトが非表示になった時呼び出される*/
    private void OnDisable()
    {
        /*プレイヤーがなくなると呼び出されなくなる*/
        //playerActionsAsset.Player.Attack.started -= Attack;
       // playerActionsAsset.Player.Block.started -= Block;
        //playerActionsAsset.Player.Block.canceled -= Block;

        /*無効にする*/
        playerActionsAsset.Player.Disable();

    }

    private void Update()
    {
        /*現在の速度と最大値を割ってアニメーションを切り替える*/
        animator.SetFloat("speed", rb.velocity.sqrMagnitude / maxSpeed);
    }

    private void FixedUpdate()
    {
        SpeedCheck();

        if (!canMove)
        {
            return;
        }

        /*移動用*/
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera);
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera);

        /*力を加える*/
        rb.AddForce(forceDirection, ForceMode.Impulse);
        /*↑力が加え続けられるので一回一回0にする必要がある*/
        forceDirection = Vector3.zero;

        LookAt();
    }



    private void Block(InputAction.CallbackContext obj)
    {
        /*プレイヤーのスピードと回転を0にする*/
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        /*アニメーションを切り替える*/
        animator.SetBool("block", !animator.GetBool("block"));
        Change();
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        /*プレイヤーのスピードと回転を0にする*/
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        /*アニメーションを切り替える*/
        animator.SetTrigger("attack");

    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        /*取得したポジションを格納する*/
        Vector3 right = playerCamera.transform.right;
        right.y = 0;

        /*ベクトルの正規化*/
        return right.normalized;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        /*取得したポジションを格納する*/
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;

        /*ベクトルの正規化*/
        return forward.normalized;
    }

    private void SpeedCheck()
    {
        /*現在のスピードの値を格納する*/
        Vector3 playerVelocity = rb.velocity;
        /*上下に動かないので0にする*/
        playerVelocity.y = 0;

        /*判定*/
        if (playerVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            /*スピードの制限をする*/
            rb.velocity = playerVelocity.normalized * maxSpeed;
        }
    }

    private void LookAt()
    {
        /*現在の速度を取得する*/
        Vector3 direction = rb.velocity;
        direction.y = 0;

        /*現在のスピードが一定以上かどうか判定*/
        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
        {
            /*回転するべき方向を入れる*/
            targetRotation = Quaternion.LookRotation(direction);
            /*実際の回転に入れてあげる*/
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, 900 * Time.deltaTime);
        }
        else
        {
            /*プレイヤーが止まっているときに回転しないようにする*/
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void Change()
    {
        /*移動ができるのか*/
        canMove = !canMove;
    }
}
