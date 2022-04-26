using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameOverMenu Gom;
    private GameObject gameOver;

    private Text gO_text;
    [Tooltip("不透明度(デバッグ用に表示してます)")][SerializeField] private float opacity; 
    void Start()
    {
        Gom = GetComponent<GameOverMenu>();
        gameOver = GameObject.Find("GameOver");
        gO_text = gameOver.GetComponent<Text>();
        /*アイテム数の初期化*/
        GManager.instance.itemNum = 0;
        /*残機数の初期化*/
        GManager.instance.zankiNum = 3;

        opacity = 0;
    }

    void Update()
    {
        taiki();
        
        if(++opacity < 100)
        {
            ;
        }
        else if(opacity > 100)
        {
            opacity = 100;
        }
        gO_text.color = new Color(1, 0, 0, opacity / 100);
    }

    private void taiki()
    {
        StartCoroutine("GameOverMenu");
    }

    IEnumerator GameOverMenu()
    {
        yield return new WaitForSecondsRealtime(3);  //処理を待機
        //gameOver.SetActive(false);
        
        Gom.show_menu = true;
    }
}
