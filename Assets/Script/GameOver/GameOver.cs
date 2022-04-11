using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameOverMenu Gom;
    private GameObject gameOver;

    void Start()
    {
        Gom = GetComponent<GameOverMenu>();
        gameOver = GameObject.Find("GameOver");
        /*アイテム数の初期化*/
        GManager.instance.itemNum = 0;
        /*残機数の初期化*/
        GManager.instance.zankiNum = 3;
    }

    void Update()
    {
        taiki();
    }

    private void taiki()
    {
        StartCoroutine("GameOverMenu");
    }

    IEnumerator GameOverMenu()
    {
        yield return new WaitForSecondsRealtime(3);  //処理を待機
        gameOver.SetActive(false);
        Gom.show_menu = true;
    }
}
