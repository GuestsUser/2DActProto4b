using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    /* クリア時のみtrueになるフラグシーン遷移で初期化 */
    public bool isStage1Clear;   /*ステージ1をクリアしたかどうか*/
    public bool isStage2Clear;   /*ステージ2をクリアしたかどうか*/
    public bool isStage3Clear;   /*ステージ3をクリアしたかどうか*/

    /* 一度クリアしたらずっとtrue */
    static public bool _isStage1Clear;
    static public bool _isStage2Clear;
    static public bool _isStage3Clear;

    /*フラグ持ち込み用_Tomokazu*/
    public bool stage1Clear {get{ return _isStage1Clear; } } /* 糸数:変数からプロパティに変更 */
    public bool stage2Clear { get { return _isStage2Clear; } }

    void Start()
    {

        /*ステージ1が始まったらクリアしてない状態になるから、どうにかしないといけない*/
        isStage1Clear = false;
        /*ステージ2が始まったらクリアしてない状態になるから、どうにかしないといけない*/
        isStage2Clear = false;
        /*ステージ3が始まったらクリアしてない状態になるから、どうにかしないといけない*/
        isStage3Clear = false;
    }


    void Update()
    {
        if (isStage1Clear)
        {
            _isStage1Clear = true;
        }

        if (isStage2Clear)
        {
            _isStage2Clear = true;
        }

        if (isStage3Clear)
        {
            _isStage3Clear = true;
        }

        switch (SceneManager.GetActiveScene().name)
        {
            case "Stage1":
                /*ステージ1またはステージ2がクリアしていたら*/
                if (isStage1Clear) /* 確認用で3を入れた */
                {
                    /*シーンをステージクリア画面に切り替える*/
                    SceneManager.LoadScene("Clear");
                }
                break;

            case "Stage2":
                /*ステージ1またはステージ2がクリアしていたら*/
                if (_isStage1Clear && isStage2Clear) /* 確認用で3を入れた */
                {
                    /*シーンをステージクリア画面に切り替える*/
                    SceneManager.LoadScene("Clear");
                }
                break;

            case "Stage3":
                /*すべてのステージをクリアしていたら*/
                if (_isStage1Clear && _isStage2Clear && isStage3Clear) /* ステージ3のみ毎回初期化されるフラグにしておく */
                {
                    /*シーンをオールステージクリア画面に切り替える*/
                    SceneManager.LoadScene("AllClear");
                }
                break;
        }

        /*ステージ1またはステージ2がクリアしていたら*/
        //if (isStage1Clear || isStage2Clear ) /* 確認用で3を入れた */
        //{
        //    /*シーンをステージクリア画面に切り替える*/
        //    SceneManager.LoadScene("Clear");
        //}
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aa");
        ///*現在のシーンがStage1なら*/
        //if (SceneManager.GetActiveScene().name == "PreStage1")
        //{
        //    /*触れたオブジェクトのタグがPlayerなら*/
        //    if (other.gameObject.tag == ("Player"))
        //    {
        //        /*isStage1Clearがfalseなら*/
        //        if (!isStage1Clear)
        //        {
        //            /*ステージ１をクリアにする*/
        //            isStage1Clear = true;
        //            Debug.Log("ステージ1クリア" + isStage1Clear);
        //        }
        //    }
        //}
        ///*現在のシーンがStage2なら*/
        //else if (SceneManager.GetActiveScene().name == "Game2")
        //{
        //    /*触れたオブジェクトのタグがPlayerなら*/
        //    if (other.gameObject.tag == ("Player"))
        //    {
        //        /*isStage2Clearがfalseなら*/
        //        if (!isStage2Clear)
        //        {
        //            /*ステージ2をクリアにする*/
        //            isStage2Clear = true;
        //            Debug.Log("ステージ2クリア" + isStage2Clear);
        //        }
        //    }
        //}
        ///*現在のシーンがStage3なら*/
        //else if (SceneManager.GetActiveScene().name == "KutuTestStage")
        //{
        //    /*触れたオブジェクトのタグがPlayerなら*/
        //    if (other.gameObject.tag == ("Player"))
        //    {
        //        /*isStage2Clearがfalseなら*/
        //        if (!isStage3Clear)
        //        {
        //            /*ステージ2をクリアにする*/
        //            isStage3Clear = true;
        //            Debug.Log("ステージ2クリア" + isStage3Clear);
        //        }
        //    }
        //}



        /*4月14日ビルド用*/
        /*現在のシーンがStage1なら*/
        if (SceneManager.GetActiveScene().name == "Stage1")
        {
            /*触れたオブジェクトのタグがPlayerなら*/
            if (other.gameObject.tag == ("Player"))
            {
                /*isStage1Clearがfalseなら*/
                if (!isStage1Clear)
                {
                    /*ステージ１をクリアにする*/
                    isStage1Clear = true;
                    Debug.Log("ステージ1クリア" + isStage1Clear);
                }
            }
        }
        /*現在のシーンがStage2なら*/
        else if (SceneManager.GetActiveScene().name == "Stage2")
        {
            /*触れたオブジェクトのタグがPlayerなら*/
            if (other.gameObject.tag == ("Player"))
            {
                /*isStage2Clearがfalseなら*/
                if (_isStage1Clear && !isStage2Clear)
                {
                    /*ステージ2をクリアにする*/
                    isStage2Clear = true;
                    Debug.Log("ステージ2クリア" + isStage2Clear);
                }
            }
        }
        /*現在のシーンがStage3なら*/
        else if (SceneManager.GetActiveScene().name == "Stage3")
        {
            /*触れたオブジェクトのタグがPlayerなら*/
            if (other.gameObject.tag == ("Player"))
            {
                /*isStage2Clearがfalseなら*/
                if (_isStage1Clear && _isStage2Clear && !isStage3Clear)
                {
                    /*ステージ3をクリアにする*/
                    isStage3Clear = true;
                    Debug.Log("ステージ3クリア" + isStage3Clear);
                }
            }
        }
    }
}
