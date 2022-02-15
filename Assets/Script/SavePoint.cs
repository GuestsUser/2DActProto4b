using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    [SerializeField] private GameObject retrySystem; /* リトライシステムが付いてるオブジェクト */
    private bool useBool; /* このセーブポイントが使用済みか否か、trueで使用済み */
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        /* こっちの条件文はプレイヤーが完成次第プレイヤーにtag付けしてそれも条件文に追加する予定 */
        if (!useBool)
        {
            retrySystem.GetComponent<RetrySystem>().RetrayAreaUpdate(this.gameObject); /* リトライ位置をこのセーブポイントの位置に更新 */
            useBool = true; /* セーブポイントを使用済みにする */
            gameObject.SetActive(false); /* 使用済みで非表示化(後々演出挿入でこの処理は消えるかも) */
        }
    }
}
