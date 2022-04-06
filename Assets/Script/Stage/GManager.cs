using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GManager : MonoBehaviour
{
    /*最初からメモリの領域を確保する*/
    /*GManager_test.instanceと書くだけでどこからでもアクセスすることができる*/
    public static GManager instance = null;

    [Header("現在の残機")] public int zankiNum;
    [Header("コイン要素の取得数")] public int itemNum = 0;
    [Header("デフォルトの残機")] public int defaultZankiNum;
    [HideInInspector] public bool isGameOver;

    private void Awake()
    {
        if (instance == null)
        {
            /*確保されたメモリ領域に自分自身を入れる*/
            instance = this;
            /*シーン切り替え時に破棄されない状態にする命令*/
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            /*中身がすでに入っていた場合インスタンスがくっついているゲームオブジェクトの破棄*/
            Destroy(this.gameObject);
        }
    }

    /*残機を1増やす*/
    public void AddZankiNum()
    {
        /*残機が99より少なかったら*/
        if (zankiNum < 99)
        {
            ++zankiNum;
        }
    }

    /*残機を1減らす*/
    public void SubZankiNum()
    {
        /*残機が0より多かったら*/
        if (zankiNum > 0)
        {
            --zankiNum;
        }
        else
        {
            isGameOver = true;
            SceneManager.LoadScene("GameOver");
        }
    }
}
