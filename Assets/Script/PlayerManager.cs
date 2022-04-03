using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private int itemCount;      /*アイテム取得数保持変数*/
    private int zanki;          /*（仮）残機*/
    public Text itemCountText;  /*アイテム取得数表示用テキスト*/

    private void Start()
    {
        itemCount = 0;
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
    }
}
