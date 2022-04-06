using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{

    private GameOverMenu Gom;

    //[SerializeField] private GameObject gameOvertext;
    //[SerializeField] private GameObject gameOverMenu;

    void Start()
    {
        Gom = GetComponent<GameOverMenu>();
        //gameOvertext = GameObject.Find("GameOver");
        //gameOverMenu = GameObject.Find("GameOverMenu");
       // gameOverMenu.SetActive(false);
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

        Gom.show_menu = true;
    }
}
