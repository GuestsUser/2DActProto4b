using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("プレイヤー判定")] public PlayerTriggerCheck playerCheck;

    [SerializeField] private ParticleSystem ps;
    [SerializeField] private GameObject coin_model;

    /*コイン取得時の音*/
    //private AudioSource audioSource;
    public AudioClip coinSound;

    private void Start()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {

        /*回転させる*/
        transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);

        /*チェックがついていたら*/
        if (playerCheck.isOn)
        {
            /*ゲームマネージャーがあるかどうか（あった場合）*/
            if (coin_model)
            {
                /*音を鳴らす*/
                AudioSource.PlayClipAtPoint(coinSound, this.transform.position);
                Debug.Log("音が鳴りました");
                /*コイン要素取得数に1プラス*/
                ++GManager.instance.itemNum;
                /*そのオブジェクトを消す*/
                Destroy(coin_model);
                StartCoroutine(particleStartToEnd());
            }
        }

        if(GManager.instance.itemNum >= 50)
        {
            GManager.instance.AddZankiNum();
            GManager.instance.itemNum = 0;
        }
    }

    private IEnumerator particleStartToEnd()
    {
        ps.Play();
        yield return new WaitForSeconds(0);
        /*そのオブジェクトを消す*/
        //Destroy(coin_model);
        ps.Stop();
    }
}
