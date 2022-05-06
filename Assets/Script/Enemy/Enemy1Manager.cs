//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Enemy1Manager : MonoBehaviour
//{

//    /*方向転換用*/
//    public enum MOVE_DIRECTION
//    {
//        STOP,
//        LEFT,
//        RIGHT,
//    }

//    [SerializeField] private MOVE_DIRECTION moveDirection = MOVE_DIRECTION.LEFT;

//    /*コンポーネント*/
//    private Rigidbody rb;
//    private Animator animator = null;
//    private Renderer targetRenderer = null;
//    private BoxCollider col = null;
//    private EnemyObjectCollision eoc = null;

//    /*インスペクターで設定する*/
//    [Header("移動速度")] [SerializeField] private float speed;
//    [Header("画面外でも行動するかどうか")] [SerializeField] private bool nonVisible;
//    [Header("接触判定")] [SerializeField] private EnemyCollisionCheck checkCollision;
//    [Header("敵を踏んで倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;
//    [Header("敵を疾走で倒したかどうか")] [SerializeField] private DashSystemN enemyDashDestroy;
//    [Header("trueなら右にfalseなら左に進む")] [SerializeField] private bool RightToLeft;

//    [SerializeField] private LayerMask stageLayer;

//    /*trueのとき右に進むfalseのとき左に進む*/
//    //private bool RightToLeft = false;

//    /*方向転換用*/
//    private int xVector;
//    private bool isDead;

//    private void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        animator = GetComponent<Animator>();
//        targetRenderer = GetComponent<Renderer>();
//        col = GetComponent<BoxCollider>();
//        eoc = GetComponent<EnemyObjectCollision>();
//    }

//    private void FixedUpdate()
//    {
//        if (!eoc.playerSteoOn && !eoc.playerDash)
//        {
//            /*画面内に映っていたら*/
//            if (targetRenderer.isVisible || nonVisible)
//            {
//                Debug.Log("画面内にいる");
//                /*歩きアニメーションをする*/
//                animator.SetBool("Walk", true);

//                /*移動用*/
//                switch (moveDirection)
//                {
//                    case MOVE_DIRECTION.STOP:
//                        xVector = 0;
//                        speed = 0;
//                        break;
//                    case MOVE_DIRECTION.RIGHT:
//                        xVector = 1;
//                        break;
//                    case MOVE_DIRECTION.LEFT:
//                        xVector = -1;
//                        break;
//                }

//                /*敵か、壁に当たったら　または　Rayが地面と接触していなければ*/
//                if (checkCollision.isOn || !IsGround())
//                {
//                    /*左右反転する*/
//                    RightToLeft = !RightToLeft;
//                }

//                /*右の場合*/
//                if (RightToLeft)
//                {
//                    moveDirection = MOVE_DIRECTION.RIGHT;
//                    transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
//                }
//                else
//                {
//                    /*左の場合*/
//                    moveDirection = MOVE_DIRECTION.LEFT;
//                    transform.localScale = new Vector3(0.7f, 0.7f, -0.7f);
//                }

//                /*敵を横移動させる*/
//                rb.velocity = new Vector2(xVector * speed, rb.velocity.y);
//            }
//            else
//            {
//                //RigidBodyを停止させる（勝手に起きる）
//                rb.Sleep();
//                /*歩きを止めてIdleアニメーションをする*/
//                animator.SetBool("Walk", false);
//            }
//        }
//        else
//        {
//            if (!isDead)
//            {
//                animator.SetTrigger("Die");
//                isDead = true;
//                enemyDestroy.isStepOnDead = false;
//                enemyDashDestroy.isDashDead = false;
//                col.enabled = false;
//                Destroy(gameObject, 0.8f);
//            }
//        }
//    }

//    private bool IsGround()
//    {
//        /*Rayの作成*/
//        Vector3 startVec = transform.up * 0.25f + transform.position + transform.forward * 0.65f * transform.localScale.z;
//        Vector3 endVec = startVec - transform.up * 0.5f;
//        Vector3 edVec = endVec - transform.forward * 0.2f;
//        Debug.DrawLine(startVec, endVec, Color.red);
//        Debug.DrawLine(endVec, edVec, Color.red);
//        return Physics.Linecast(startVec, endVec, stageLayer)
//            || Physics.Linecast(endVec,edVec, stageLayer);
//    }
//}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1Manager : MonoBehaviour
{

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
    [Header("敵を踏んで倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;
    [Header("敵を疾走で倒したかどうか")] [SerializeField] private DashSystemN enemyDashDestroy;
    [Header("trueなら右にfalseなら左に進む")] [SerializeField] private bool RightToLeft;

    [SerializeField] private LayerMask stageLayer;

    /*trueのとき右に進むfalseのとき左に進む*/
    //private bool RightToLeft = false;

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

    private void FixedUpdate()
    {
        if (!eoc.playerSteoOn && !eoc.playerDash)
        {
            /*画面内に映っていたら*/
            if (targetRenderer.isVisible || nonVisible)
            {
                //Debug.Log("画面内にいる");
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
                enemyDestroy.isStepOnDead = false;
                enemyDashDestroy.isDashDead = false;
                col.enabled = false;
                Destroy(gameObject, 0.8f);
            }
        }
    }

    private bool IsGround()
    {
        /*Rayの作成*/
        Vector3 startVec = transform.up * 0.25f + transform.position + transform.forward * 0.65f * transform.localScale.z;
        Vector3 endVec = startVec - transform.up * 0.5f;
        Vector3 edVec = endVec - transform.forward * 0.2f;
        Debug.DrawLine(startVec, endVec, Color.red);
        Debug.DrawLine(endVec, edVec, Color.red);
        return Physics.Linecast(startVec, endVec, stageLayer)
            || Physics.Linecast(endVec, edVec, stageLayer);
    }
}
