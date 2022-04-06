using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("プレイヤー判定")] public PlayerTriggerCheck playerCheck;

    private void Update()
    {
        if (playerCheck.isOn)
        {
            if(GManager.instance != null)
            {
                ++GManager.instance.itemNum;
                Destroy(this.gameObject);
            }
        }
    }
}
