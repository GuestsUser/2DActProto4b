using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*プレイヤーがこのコライダーに触れたらプレイヤーが死ぬ（ステージ3とげ亀用スクリプト）*/
public class EnemyPlayerCollision : MonoBehaviour
{
    public bool isOn = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isOn = false;
        }
    }
}
