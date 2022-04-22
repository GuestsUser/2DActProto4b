using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{

    static public bool isStage1Clear;   /*ステージ1をクリアしたかどうか*/
    static public bool isStage2Clear;   /*ステージ2をクリアしたかどうか*/
    static public bool isStage3Clear;   /*ステージ3をクリアしたかどうか*/

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
        /*ステージ1またはステージ2がクリアしていたら*/
<<<<<<< HEAD
        if (isStage1Clear || isStage2Clear || isStage3Clear)
=======
        if (isStage1Clear || isStage2Clear || isStage3Clear) /* 確認用で3を入れた */
>>>>>>> 6683019d42d56f55a7a235e192f231f72e94940d
        {
            /*シーンをステージクリア画面に切り替える*/
            SceneManager.LoadScene("Clear");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
<<<<<<< HEAD
=======
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
>>>>>>> 6683019d42d56f55a7a235e192f231f72e94940d
        /*現在のシーンがStage1なら*/
        if (SceneManager.GetActiveScene().name == "PreStage1")
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
        else if (SceneManager.GetActiveScene().name == "Game2")
        {
            /*触れたオブジェクトのタグがPlayerなら*/
            if (other.gameObject.tag == ("Player"))
            {
                /*isStage2Clearがfalseなら*/
                if (!isStage2Clear)
                {
                    /*ステージ2をクリアにする*/
                    isStage2Clear = true;
                    Debug.Log("ステージ2クリア" + isStage2Clear);
                }
            }
        }
        /*現在のシーンがStage3なら*/
        else if (SceneManager.GetActiveScene().name == "KutuTestStage")
        {
            /*触れたオブジェクトのタグがPlayerなら*/
            if (other.gameObject.tag == ("Player"))
            {
                /*isStage2Clearがfalseなら*/
                if (!isStage3Clear)
                {
                    /*ステージ2をクリアにする*/
                    isStage3Clear = true;
                    Debug.Log("ステージ2クリア" + isStage3Clear);
                }
            }
        }
    }
}
