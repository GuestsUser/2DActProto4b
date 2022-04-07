using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemText : MonoBehaviour
{
    private Text itemText = null;
    private int oldItemNum = 0;

    private void Start()
    {
        itemText = GetComponent<Text>();

        if (GManager.instance != null)
        {
            itemText.text = "コイン要素：" + GManager.instance.itemNum + " / 2";
        }
        else
        {
            Debug.Log("ゲームマネージャーの置き忘れ！");
            Destroy(this);
        }
    }

    private void Update()
    {

        /*アイテム数が変わった時だけ更新*/
        if (oldItemNum != GManager.instance.itemNum)
        {
            itemText.text = "コイン要素：" + GManager.instance.itemNum + " / 2";
            oldItemNum = GManager.instance.itemNum;
        }
    }
}
