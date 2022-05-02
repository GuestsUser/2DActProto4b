using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyEnemyMove : MonoBehaviour
{
    /*インスペクターで設定する*/
    [Header("反復移動する")]
    [Tooltip("この値分移動する")] [SerializeField] Vector3 move = Vector3.zero;
    [Tooltip("move分移動して戻ってくるまでの時間、秒指定")] [SerializeField] float limit = 0;
    [Tooltip("trueにすると初期地点を0とし0、move、0、-move、0(以下ループ)と移動する")] [SerializeField] bool inverse = false;

    [Header("画面外でも行動するかどうか")] [SerializeField] private bool nonVisible;
    [Header("敵を倒したかどうか")] [SerializeField] private BoxCastEnemyDestroy enemyDestroy;

    /*コンポーネント*/
    private Animator animator = null;
    private Renderer targetRenderer = null;
    private BoxCollider col = null;
    private EnemyObjectCollision eoc = null;

    /*変数*/
    private Vector3 iniPos;      /* 初期位置記録 */
    private float iniAngle;
    private bool isDead;         /* 死んだかどうか */
    [SerializeField] private bool isCheck = false;/*判定*/

    void Start()
    {
        animator = GetComponent<Animator>();
        targetRenderer = GetComponent<Renderer>();
        col = GetComponent<BoxCollider>();
        eoc = GetComponent<EnemyObjectCollision>();

        iniPos = transform.position;
        iniAngle = 0;

        //if (!inverse)
        //{
        //    move /= 2; /* 移動量を半分に */
        //    iniPos += move; /* 記録上の初期位置に移動量の半分を加算 */
        //    iniAngle = -90; /* 初期移動状態を-1とすることで現在位置から始めることができる */
        //}
        //StartCoroutine(Repeat(iniAngle));
    }


    private void FixedUpdate()
    {
        if (!eoc.playerSteoOn)
        {
            /*画面内に映っていたら*/
            if (targetRenderer.isVisible || nonVisible)
            {
                if (!isCheck)
                {
                    isCheck = true;
                    Debug.Log("画面内にいる");

                    if (!inverse)
                    {
                        move /= 2; /* 移動量を半分に */
                        iniPos += move; /* 記録上の初期位置に移動量の半分を加算 */
                        iniAngle = -90; /* 初期移動状態を-1とすることで現在位置から始めることができる */
                    }
                    StartCoroutine(Repeat(iniAngle));
                }
            }
            else
            {
                isCheck = false;
                Debug.Log("画面外にいる");
            }
        }
        else
        {
            if (!isDead)
            {
                animator.SetTrigger("Die");
                isDead = true;
                enemyDestroy.isStepOnDead = false;
                col.enabled = false;
                move = Vector3.zero;
                Destroy(gameObject, 0.8f);
            }
        }
    }

    IEnumerator Repeat(float iniAngle)
    {
        float count = 0;
        while (true)
        {
            transform.position = iniPos + move * Mathf.Sin((float)((360 / limit * count + iniAngle) * Mathf.Deg2Rad));
            count = (count + Time.deltaTime) % limit;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

    }
}
