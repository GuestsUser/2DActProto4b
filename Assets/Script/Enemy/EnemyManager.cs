using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    //public enum MOVE_DIRECTION
    //{
    //    STOP,
    //    LEFT,
    //    RIGHT,
    //}
    //MOVE_DIRECTION moveDirection = MOVE_DIRECTION.STOP;


    ///*コンポーネント*/
    //private Rigidbody rb;
    //private Animator animator = null;
    //private Renderer targetRenderer = null;
    //private BoxCollider col = null;

    ///*インスペクターで設定する*/
    //[Header("移動速度")] [SerializeField] private float speed;
    //[Header("画面外でも行動するかどうか")] [SerializeField] private bool nonVisible;
    //[Header("接触判定")] [SerializeField] private EnemyCollisionCheck checkCollision;
    //[Header("敵を倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;

    ///*trueのとき右に進むfalseのとき左に進む*/
    //private bool RightToLeft = false;
    //private bool Dead = false;

    //private void Start()
    //{
    //    rb = GetComponent<Rigidbody>();
    //    animator = GetComponent<Animator>();
    //    targetRenderer = GetComponent<Renderer>();
    //    col = GetComponent<BoxCollider>();
    //}

    //private void Update()
    //{
    //    if (enemyDestroy.isDead)
    //    {
    //        animator.SetTrigger("Die");
    //        speed = 0;
    //        Destroy(gameObject, 0.7f);
    //        enemyDestroy.isDead = false;
    //        Dead = true;
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    /*画面内に映っていたら*/
    //    if (targetRenderer.isVisible || nonVisible)
    //    {
    //        /*敵か、壁に当たったら*/
    //        if (checkCollision.isOn)
    //        {
    //            /*左右反転する*/
    //            RightToLeft = !RightToLeft;
    //        }

    //        //Debug.Log("画面に映っている");

    //        /*歩きアニメーションをする*/
    //        animator.SetBool("Walk", true);

    //        /*左用*/
    //        int xVector = -1;

    //        if (RightToLeft)
    //        {
    //            /*右用*/
    //            xVector = 1;
    //            transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
    //        }
    //        else
    //        {
    //            transform.localScale = new Vector3(0.7f, 0.7f, -0.7f);
    //        }
    //        /*敵を横移動させる*/
    //        rb.velocity = new Vector2(xVector * speed, rb.velocity.y);
    //    }
    //    else
    //    {
    //        //RigidBodyを停止させる（勝手に起きる）
    //        rb.Sleep();
    //        /*歩きを止めてIdleアニメーションをする*/
    //        animator.SetBool("Walk", false);
    //    }

    //    /*もし死んだとき*/
    //    if (Dead)
    //    {
    //        /*コライダーの消去*/
    //        col.enabled = false;
    //        /*フラグ戻す*/
    //        Dead = false;
    //    }
    //}

    //private bool IsGround()
    //{
    //    return true;
    //}

    //void ChangeDirection()
    //{
    //    if ()
    //    {

    //    }
    //    //右に移動

    //    //左に移動
    //}



    /*方向転換用*/
    public enum MOVE_DIRECTION
    {
        STOP,
        LEFT,
        RIGHT,
    }

    [SerializeField] private MOVE_DIRECTION moveDirection = MOVE_DIRECTION.LEFT;


    /*コンポーネント*/
    private Rigidbody rb;
    private Animator animator = null;
    private Renderer targetRenderer = null;
    private BoxCollider col = null;
    private EnemyObjectCollision eoc = null;

    /*インスペクターで設定する*/
    [Header("移動速度")] [SerializeField] private float speed;
    [Header("画面外でも行動するかどうか")] [SerializeField] private bool nonVisible;
    [Header("接触判定")] [SerializeField] private EnemyCollisionCheck checkCollision;
    [Header("敵を倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;

    [SerializeField] private LayerMask stageLayer;

    /*trueのとき右に進むfalseのとき左に進む*/
    private bool RightToLeft = false;

    /*方向転換用*/
    private int xVector;
    private bool isDead;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        targetRenderer = GetComponent<Renderer>();
        col = GetComponent<BoxCollider>();
        eoc = GetComponent<EnemyObjectCollision>();
    }

    private void Update()
    {
        //if (enemyDestroy.isDead) {
        //    //animator.SetTrigger("Die");
        //    moveDirection = MOVE_DIRECTION.STOP;

        //    Destroy(this.gameObject, 0.7f);
        //    enemyDestroy.isDead = false;
        //}
    }

    private void FixedUpdate()
    {
        if (!eoc.playerSteoOn)
        {
            /*画面内に映っていたら*/
            if (targetRenderer.isVisible || nonVisible)
            {
                /*歩きアニメーションをする*/
                animator.SetBool("Walk", true);

                /*移動用*/
                switch (moveDirection)
                {
                    case MOVE_DIRECTION.STOP:
                        xVector = 0;
                        speed = 0;
                        break;
                    case MOVE_DIRECTION.RIGHT:
                        xVector = 1;
                        break;
                    case MOVE_DIRECTION.LEFT:
                        xVector = -1;
                        break;
                }

                    /*敵か、壁に当たったら　または　Rayが地面と接触していなければ*/
                    if (checkCollision.isOn || !IsGround())
                    {
                        /*左右反転する*/
                         RightToLeft = !RightToLeft;
                    }

                    /*右の場合*/
                    if (RightToLeft)
                    {
                        moveDirection = MOVE_DIRECTION.RIGHT;
                        transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    }
                    else
                    {
                    /*左の場合*/
                    moveDirection = MOVE_DIRECTION.LEFT;
                    transform.localScale = new Vector3(0.7f, 0.7f, -0.7f);
                }

                /*敵を横移動させる*/
                rb.velocity = new Vector2(xVector * speed, rb.velocity.y);
            }
            else
            {
                //RigidBodyを停止させる（勝手に起きる）
                rb.Sleep();
                /*歩きを止めてIdleアニメーションをする*/
                animator.SetBool("Walk", false);
            }
        }
        else
        {
            if (!isDead)
            {
                animator.SetTrigger("Die");
                isDead = true;
                enemyDestroy.isDead = false;
                col.enabled = false;
                Destroy(gameObject, 0.8f);
            }
        }
    }

    private bool IsGround()
    {
        /*Rayの作成*/
        Vector3 startVec = transform.up * 0.25f + transform.position + transform.forward * 0.8f * transform.localScale.z;
        Vector3 endVec = startVec - transform.up * 0.5f;
        Debug.DrawLine(startVec, endVec,Color.red);
        return Physics.Linecast(startVec,endVec,stageLayer);
    }
}
