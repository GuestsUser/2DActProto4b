using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZankiText : MonoBehaviour
{
    private Text zankiText = null;
    private RectTransform zanki_pos; /* 画像 */
    private RectTransform Text_pos; /* 残機の値 */
    private RawImage zanki;
    private int oldZankiNum = 0;

    private void Start()
    {
        zankiText = GetComponent<Text>();
        zanki = GameObject.Find("ZankiImage").GetComponent<RawImage>();

        if (GManager.instance != null)
        {
            zanki_pos = zanki.GetComponent<RectTransform>();
            zanki_pos.localPosition = new Vector3(-25,320,0);
            zankiText.text += GManager.instance.zankiNum;
            Text_pos = zankiText.GetComponent<RectTransform>();
            Text_pos.localPosition = new Vector3(25, 320, 0);
        }
        else
        {
            Debug.Log("ゲームマネージャーの置き忘れ！");
            Destroy(this);
        }
    }

    private void Update()
    {
        /*前の残機数を記録しておいて、現在の残機と違った場合*/
        if (oldZankiNum != GManager.instance.zankiNum)
        {
            /*テキストに現在の残機数を書き込む*/
            zankiText.text = string.Format("{0:0}",GManager.instance.zankiNum);
            oldZankiNum = GManager.instance.zankiNum;
        }
    }
}
