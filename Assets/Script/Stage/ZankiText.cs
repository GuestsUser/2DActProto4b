using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZankiText : MonoBehaviour
{
    private Text zankiText = null;
    private int oldZankiNum = 0;

    private void Start()
    {
        zankiText = GetComponent<Text>();

        if (GManager.instance != null)
        {
            zankiText.text = "残機数：" + GManager.instance.zankiNum;
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
            zankiText.text = "残機数：" + GManager.instance.zankiNum;
            oldZankiNum = GManager.instance.zankiNum;
        }
    }
}
