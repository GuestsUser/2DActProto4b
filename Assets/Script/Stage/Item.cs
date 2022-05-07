using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("プレイヤー判定")] public PlayerTriggerCheck playerCheck;

    /*コイン取得時の音*/
    //private AudioSource audioSource;
    public AudioClip coinSound;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        /*チェックがついていたら*/
        if (playerCheck.isOn)
        {
            /*ゲームマネージャーがあるかどうか（あった場合）*/
            if (GManager.instance != null)
            {
                /*音を鳴らす*/
                AudioSource.PlayClipAtPoint(coinSound, this.transform.position);
                Debug.Log("音が鳴りました");
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
