using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class StageSelectBack : MonoBehaviour
{
    /* 表示するアイテムを変更するのにクリアステージのフラグを持ってくる */
    StageClear clear;

    static bool[] show = new bool[] {false,false }; /* true:既に表示した */

    /*仮：ステージセレクトに戻るとき表記するテキスト入れるオブジェクト*/
    public GameObject stageselectback_object = null;
    public MenuSE menuse;
    private bool se = false;

    int WaitTime = 1;

    /*ポップアップをフェードさせる処理_Tomokazu*/
    public float opasity;
    public float alfa;
    public float a, b, c;
    public bool opasityFlg = false;
    Image image,image2;
    GameObject popup;


    /*ポップアップをフェードさせる処理_Tomokazu*/

    void Start()
    {
        clear = GetComponent<StageClear>();

        se = false;

        /*ポップアップをフェードさせる処理_Tomokazu*/
        opasity = 0.02f;
        alfa = GameObject.Find("Image2").GetComponent<Image>().color.a;
        a = GameObject.Find("Image2").GetComponent<Image>().color.r;
        b = GameObject.Find("Image2").GetComponent<Image>().color.g;
        c = GameObject.Find("Image2").GetComponent<Image>().color.b;
        image = GameObject.Find("Image2").GetComponent<Image>();
        image2 = GameObject.Find("Image3").GetComponent<Image>();
        popup = GameObject.Find("Popup");

        popup.SetActive(false);
        /*ポップアップをフェードさせる処理_Tomokazu*/
    }


    void Update()
    {
        /* 糸数:5/14 追加　ゲットアイテムの表示/非表示切り替え */
        /* Stage1普通にクリアした時の処理 */
        if (show[0] == false)
        {
            if (clear.stage1Clear && !clear.stage2Clear)
            {
                //Debug.Log("ジャンプシューズ用の画像に変更");
                popup.SetActive(true);
                image2.sprite = Resources.Load<Sprite>("ジャンプシューズ用");

                //show[0] = true;

                BeforeFade();
            }
        }
        else
        {
            /*ゲームパットのAボタンが押されて、フェードイン処理が終わっていたら*/
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (!se)
                {
                    se = true;
                    menuse.audio_source.clip = menuse.decision;
                    menuse.audio_source.PlayOneShot(menuse.decision);

                }
                StartCoroutine("BacktoTitle");  /*ステージセレクトに戻る*/

            }
        }

        /* Stage2普通にクリアした時の処理 */
        if (show[1] == false)
        {
            if (clear.stage1Clear && clear.stage2Clear)
            {
                //Debug.Log("ジャンプシューズ用の画像に変更");
                popup.SetActive(true);
                image2.sprite = Resources.Load<Sprite>("疾走シューズ");

                //show[1] = true;

                BeforeFade();
            }
        }
        else
        {
            /*ゲームパットのAボタンが押されて、フェードイン処理が終わっていたら*/
            if (Gamepad.current.buttonSouth.wasPressedThisFrame)
            {
                if (!se)
                {
                    se = true;
                    menuse.audio_source.clip = menuse.decision;
                    menuse.audio_source.PlayOneShot(menuse.decision);

                }
                StartCoroutine("BacktoTitle");  /*ステージセレクトに戻る*/

            }
        }
        /* 糸数:5/14 追加　ゲットアイテムの表示/非表示切り替え */

    }

    private IEnumerator BacktoTitle() //シーンチェンジ用
    {
        yield return new WaitForSecondsRealtime(WaitTime);  //処理を待機

        /*ステージセレクト画面に切り替える*/
        SceneManager.LoadScene(1);

    }

    private void BeforeFade() /* 糸数:5/14 追加フェード前処理 (中身自体は友一スクリプト)*/
    {
        /*ゲームパッドのAボタンが押されて、フェードインが始まっていなかったら*/
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && alfa > 0)
        {
            if (!se)
            {
                se = true;
                menuse.audio_source.clip = menuse.decision;
                menuse.audio_source.PlayOneShot(menuse.decision);

            }

            opasityFlg = true;  /*フェードイン処理を始める*/

        }
        /*フェードイン処理*/
        if (opasityFlg)
        {

            image2.gameObject.SetActive(false);     /*ポップアップを非表示にする*/
            StartFadeIn();                         /*フェードイン処理開始*/


        }
        /*フェードイン処理*/

        /*ゲームパットのAボタンが押されて、フェードイン処理が終わっていたら*/
        if (Gamepad.current.buttonSouth.wasPressedThisFrame && alfa <= 0)
        {
            if (!se)
            {
                se = true;
                menuse.audio_source.clip = menuse.decision;
                menuse.audio_source.PlayOneShot(menuse.decision);

            }
            if(show[0] == false)
            {
                show[0] = true;
            }
            else if (show[0] && !show[1])
            {
                show[1] = true;
            }
            StartCoroutine("BacktoTitle");  /*ステージセレクトに戻る*/

        }

        /*操作ボタンが分かるように仮表示*/
        Text stageselectback_text = stageselectback_object.GetComponent<Text>();
        stageselectback_text.text = "Aボタンでステージセレクト画面に戻ります";
    }

    /*フェードイン処理*/
    private void StartFadeIn()
    {

        alfa -= opasity;    /*透明度を上げる*/

        SetAlpha();     /*透明度を適用*/

        /*透明度がゼロになったら*/
        if (alfa <= 0)
        {

            opasityFlg = false;     /*フェードイン処理を終了*/
            se = false;             /*SEをリセットして音が鳴るようにする*/
           popup.SetActive(false);  /*ポップアップを非表示にする*/

        }

    }

    private void SetAlpha()
    {

           image.color = new Color(a, b, c, alfa);  /*透明度を適用*/

    }
    /*フェードイン処理*/

}
