using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public void GetItem()
    {
        /*このオブジェクトを消す*/
        Destroy(this.gameObject);
    }
}
