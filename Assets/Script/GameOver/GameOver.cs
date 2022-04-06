using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    private GameOverMenu Gom;
    public GameObject gameOver;

    void Start()
    {
        Gom = GetComponent<GameOverMenu>();
        gameOver = GameObject.Find("GameOver");
        GManager.instance.itemNum = 0;
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
