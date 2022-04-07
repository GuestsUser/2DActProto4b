using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTriggerCheck : MonoBehaviour
{
    /*判定内にプレイヤーがいるかどうか*/
    [HideInInspector] public bool isOn = false;

    private void OnTriggerEnter(Collider other)
    {
        /*もしPlayerタグを持ったオブジェクトに触れたら*/
        if (other.gameObject.tag == "Player")
        {
            /*チェックする*/
            isOn = true;
            Debug.Log("プレイヤーと接触しました");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /*もしPlayerタグを持ったオブジェクトから離れたら*/
        if (other.gameObject.tag == "Player")
        {
            /*チェック外す*/
            isOn = false;
        }
    }
}
