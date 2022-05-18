using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RetrySystem))] /* リトライスクリプト必須化 */
/* プレイヤーに付けてセーブポイントと当たり判定を取る事で必要変数を削減 */
public class SaveSystem : MonoBehaviour
{

    /*セーブポイントSE用変数*/
    AudioSource saveSouce;
    [SerializeField] AudioClip saveSe;
    /*セーブポイントSE用変数*/

    // Start is called before the first frame update
    void Start()
    {

        saveSouce = GetComponent<AudioSource>();

    }
    void OnTriggerEnter(Collider other)
    {
        
        /* こっちの条件文はプレイヤーが完成次第プレイヤーにtag付けしてそれも条件文に追加する予定 */
        if (other.gameObject.tag == "SavePoint"  && !other.gameObject.GetComponent<SavePointUseFlag>().useBool)
        {
            GetComponent<RetrySystem>().RetrayAreaUpdate(other.gameObject); /* リトライ位置をこのセーブポイントの位置に更新 */
            other.gameObject.GetComponent<SavePointUseFlag>().useBool = true; /* セーブポイントを使用済みにする */
            other.gameObject.SetActive(false); /* 使用済みで非表示化(後々演出挿入でこの処理は消えるかも) */

            saveSouce.PlayOneShot(saveSe);  /*音を鳴らす*/
        }
    }
}
