using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    /*コンポーネント*/
    private Rigidbody rb;
    private Animator animator = null;
    private Renderer targetRenderer = null;
    private BoxCollider col = null;

    /*インスペクターで設定する*/
    [Header("移動速度")] [SerializeField] private float speed;
    [Header("画面外でも行動するかどうか")] [SerializeField] private bool nonVisible;
    [Header("接触判定")] [SerializeField] private EnemyCollisionCheck checkCollision;
    [Header("敵を倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;

    /*trueのとき右に進むfalseのとき左に進む*/
    private bool RightToLeft = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        targetRenderer = GetComponent<Renderer>();
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (enemyDestroy.isDead)
        {
            animator.SetTrigger("Die");
            speed = 0;
            Destroy(this.gameObject, 1f);
            enemyDestroy.isDead = false;
        }
    }

    private void FixedUpdate()
    {
        if (targetRenderer.isVisible || nonVisible)
        {
            if (checkCollision.isOn)
            {
                RightToLeft = !RightToLeft;
            }
            Debug.Log("画面に映っている");
            animator.SetBool("Walk", true);
            int xVector = -1;
            if (RightToLeft)
            {
                xVector = 1;
                transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                transform.localScale = new Vector3(0.7f, 0.7f, -0.7f);
            }
            /*敵を横移動させる*/
            rb.velocity = new Vector2(xVector * speed, rb.velocity.y);
        }
        else
        {
            //RigidBodyを停止させる（勝手に起きる）
            rb.Sleep();
            animator.SetBool("Walk", false);
        }
    }
}
