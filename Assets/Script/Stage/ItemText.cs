using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemText : MonoBehaviour
{
    private Text itemText = null;
    private RectTransform item_pos;     /* 画像 */
    private RectTransform itemText_pos; /* アイテムの値 */
    private RawImage item;
    private int oldItemNum = -1;

    private void Start()
    {
        itemText = GetComponent<Text>();
        item = GameObject.Find("ItemImage").GetComponent<RawImage>();

        if (GManager.instance != null)
        {
            //itemText.text = string.Format("{0:0}", GManager.instance.itemNum);
            //item_pos = item.GetComponent<RectTransform>();
            //item_pos.localPosition = new Vector3(-25, 250, 0);
            itemText.text += GManager.instance.itemNum;
            //itemText_pos = itemText.GetComponent<RectTransform>();
            //itemText_pos.localPosition = new Vector3(78, 250, 0);
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
            /*テキストに現在のアイテム数を書き込む*/
            itemText.text = string.Format("{0:0}", GManager.instance.itemNum + "/50");
            oldItemNum = GManager.instance.itemNum;
        }
    }
}
