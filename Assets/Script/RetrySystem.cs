using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Player))] /* 残機の概念が完成したら残機スクリプトを必須化予定 */
/* プレイヤーに付けるスクリプトに変更 */
public class RetrySystem : MonoBehaviour
{
    [Header("プレイヤーに付ける復活システム")]
    [Tooltip("リトライ時の開始位置指示用オブジェクト、リトライ時この位置から開始、最初にここに入ってるオブジェクトの位置からステージ開始")] [SerializeField] GameObject retryPoint;
    // Start is called before the first frame update
    void Start()
    {
        Retry();
    }

    public void Retry()/* リトライさせたいタイミングで呼び出してほしい */
    {
        gameObject.transform.position = retryPoint.transform.position; /* 位置をリトライオブジェクトに揃える */
        GManager.instance.SubZankiNum();
    }

    public void RetrayAreaUpdate(GameObject newRetryPoint)/* newRetryPointに新しいリトライ位置のゲームオブジェクトを入れる事でリトライ位置を更新する */
    {
        retryPoint = newRetryPoint;
    }
}
