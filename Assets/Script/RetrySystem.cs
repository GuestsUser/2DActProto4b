using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrySystem : MonoBehaviour
{
    [SerializeField] private GameObject player;/* 自機オブジェクトをインスペクターから入れる */
    [SerializeField] private GameObject retryPoint; /* リトライ時の開始位置指示用オブジェクト、リトライ時この位置から開始、最初にここに入ってるオブジェクトの位置からステージ開始 */
    // Start is called before the first frame update
    void Start()
    {
        Retry();
    }

    public void Retry()/* リトライさせたいタイミングで呼び出してほしい */
    {
        player.transform.position = retryPoint.transform.position; /* 位置をリトライオブジェクトに揃える */
    }

    public void RetrayAreaUpdate(GameObject newRetryPoint)/* newRetryPointに新しいリトライ位置のゲームオブジェクトを入れる事でリトライ位置を更新する */
    {
        retryPoint = newRetryPoint;
    }
}
