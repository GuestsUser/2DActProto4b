using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    /*アイテム用*/
    private int itemCount;      /*アイテム取得数保持変数*/
    private int zanki;          /*（仮）残機*/
    public Text itemCountText;  /*アイテム取得数表示用テキスト*/

    /*リトライ用*/
    [Header("プレイヤーに付ける復活システム")]
    [Tooltip("リトライ時の開始位置指示用オブジェクト、リトライ時この位置から開始、最初にここに入ってるオブジェクトの位置からステージ開始")] 
    [SerializeField] GameObject retryPoint;

    /*FallDownSystem用*/
    [Header("落下死ステージのプレイヤーに付ける")]
    [Tooltip("落下死地点集、x座標の整数部が重ならないようお願いしたい")]
    [SerializeField] private List<GameObject> fallPoint;

    private void Start()
    {
        /*アイテム取得数を初期化する*/
        itemCount = 0;
        /* 1000掛ける事で第3位までの小数点を整数部に持ってくる */
        fallPoint.Sort((a, b) => (int)((a.transform.position.x - b.transform.position.x) * 1000));
        Retry();
    }

    private void Update()
    {
        /*アイテム獲得数表示更新*/
        itemCountText.text = "コイン数：" + itemCount.ToString() + " / 100";

        /*アイテム取得数が指定された数になったら*/
        if(itemCount >= 2)
        {
            /*残機+1*/
            zanki++;
            /*アイテム取得数を初期化する*/
            itemCount = 0;
        }
    }

    private void FixedUpdate()
    {
        int useSub = fallPoint.Count - 1;
        for (int i = 0; i < fallPoint.Count; i++)
        {
            if (transform.position.x <= fallPoint[i].transform.position.x)
            {
                if (i == 0) { return; } /* 初めの位置より小さい位置にプレイヤーがいる場合終了 */
                else
                {
                    useSub = i - 1; /* プレイヤーより小さく、最も近い位置の添え字を記録 */
                    break;
                }
            }
        }
        if (transform.position.y <= fallPoint[useSub].transform.position.y) /* プレイヤー位置が落下死ポイントのy座標以下の場合即死判定 */
        {
            Retry();
        }
    }

    private void Retry()
    {
        /*プレイヤーの位置をリトライ位置に戻す*/
        gameObject.transform.position = retryPoint.transform.position;
    }

    private void RetryAreaUpdate(GameObject newRetryPoint)
    {
        /* newRetryPointに新しいリトライ位置のゲームオブジェクトを入れる事でリトライ位置を更新する */
        retryPoint = newRetryPoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*プレイヤーが触れたオブジェクトのタグがItemなら*/
        if (other.gameObject.tag == "Item")
        {
            /*Itemタグが付いているオブジェクトを消す*/
            other.gameObject.GetComponent<ItemManager>().GetItem();
            /*アイテムカウント+1*/
            itemCount++;
        }

        /* こっちの条件文はプレイヤーが完成次第プレイヤーにtag付けしてそれも条件文に追加する予定 */
        if (other.gameObject.tag == "SavePoint" && !other.gameObject.GetComponent<SavePointUseFlag>().useBool)
        {
            RetryAreaUpdate(other.gameObject); /* リトライ位置をこのセーブポイントの位置に更新 */
            other.gameObject.GetComponent<SavePointUseFlag>().useBool = true; /* セーブポイントを使用済みにする */
            other.gameObject.SetActive(false); /* 使用済みで非表示化(後々演出挿入でこの処理は消えるかも) */
        }
    }
}
