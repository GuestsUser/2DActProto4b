using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private int count;
    private int zanki;
    public Text itemCountText;

    private void Start()
    {
        count = 0;
    }

    private void Update()
    {
        /*アイテム獲得数表示更新*/
        itemCountText.text = "コイン数：" + count.ToString() + " / 100";

        if(count >= 2)
        {
            zanki++;
            Debug.Log(zanki);
            count = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*プレイヤーが触れたオブジェクトのタグがItemなら*/
        if (other.gameObject.tag == "Item")
        {
            /*Itemタグが付いているオブジェクトを消す*/
            other.gameObject.GetComponent<ItemManager>().GetItem();
            /*カウント+1*/
            count++;
        }
    }
}
