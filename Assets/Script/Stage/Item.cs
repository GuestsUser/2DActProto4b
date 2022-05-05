using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("プレイヤー判定")] public PlayerTriggerCheck playerCheck;

    private void Update()
    {
        /*チェックがついていたら*/
        if (playerCheck.isOn)
        {
            /*ゲームマネージャーがあるかどうか（あった場合）*/
            if(GManager.instance != null)
            {
                /*コイン要素取得数に1プラス*/
                ++GManager.instance.itemNum;
                /*そのオブジェクトを消す*/
                Destroy(this.gameObject);
            }
        }

        if(GManager.instance.itemNum >= 50)
        {
            GManager.instance.AddZankiNum();
            GManager.instance.itemNum = 0;
        }
    }
}
