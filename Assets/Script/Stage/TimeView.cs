using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeView : MonoBehaviour
{
    [Header("")]
    [Tooltip("数値のマテリアル")] [SerializeField] Material[] numMat;
    [Tooltip("どのステージのスコアか")] [SerializeField] bool[] stage;
    [Tooltip("スタッフレコード")] [SerializeField] int[] staffTime;
    [Tooltip("速いスタッフレコード")] [SerializeField] int[] superTime;

    [Tooltip("プレイヤー用時間オブジェクト")] [SerializeField] Image[] pTimeObj;
    [Tooltip("スタッフ用時間オブジェクト")] [SerializeField] Image[] sTimeObj;
    [Tooltip("速いスタッフ用時間オブジェクト")] [SerializeField] Image[] ssTimeObj;
    [Tooltip("スタッフ背景")] [SerializeField] Image sBack;
    [Tooltip("速いスタッフ背景")] [SerializeField] Image ssBack;

    int[] pTime;
    bool useSuper;

    // Start is called before the first frame update
    void Start()
    {
        pTime = new int[3];
        if (stage[0])
        {
            pTime[2] = PlayerPrefs.GetInt("bestSt1TimeMin", 99);
            pTime[1] = PlayerPrefs.GetInt("bestSt1TimeSec", 59);
            pTime[0] = PlayerPrefs.GetInt("bestSt1TimeMil", 9);
        }
        if (stage[1])
        {
            pTime[2] = PlayerPrefs.GetInt("bestSt2TimeMin", 99);
            pTime[1] = PlayerPrefs.GetInt("bestSt2TimeSec", 59);
            pTime[0] = PlayerPrefs.GetInt("bestSt2TimeMil", 9);
        }
        if (stage[2])
        {
            pTime[2] = PlayerPrefs.GetInt("bestSt3TimeMin", 99);
            pTime[1] = PlayerPrefs.GetInt("bestSt3TimeSec", 59);
            pTime[0] = PlayerPrefs.GetInt("bestSt3TimeMil", 9);
        }


        int pScore = TimeAttack.ScoreCalculat(pTime[0], pTime[1], pTime[2]);
        int sScore= TimeAttack.ScoreCalculat(staffTime[0], staffTime[1], staffTime[2]);
        //int ssScore = TimeAttack.ScoreCalculat(superTime[0], superTime[1], superTime[2]);

        if (pScore <= sScore) { useSuper = true; }

        foreach (Image obj in pTimeObj) { obj.gameObject.SetActive(false); }
        foreach (Image obj in sTimeObj) { obj.gameObject.SetActive(false); }
        foreach (Image obj in ssTimeObj) { obj.gameObject.SetActive(false); }
        sBack.gameObject.SetActive(false);
        ssBack.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            foreach (Image obj in pTimeObj) { obj.gameObject.SetActive(true); }
            for (int i = 0; i < 3; i++) { MatChange(pTimeObj, pTime, i); }

            if (useSuper)
            {
                foreach (Image obj in sTimeObj) { obj.gameObject.SetActive(false); }
                foreach (Image obj in ssTimeObj) { obj.gameObject.SetActive(true); }
                sBack.gameObject.SetActive(false);
                ssBack.gameObject.SetActive(true);
                for (int i = 0; i < 3; i++) { MatChange(ssTimeObj, superTime, i); }
            }
            else
            {
                foreach (Image obj in sTimeObj) { obj.gameObject.SetActive(true); }
                foreach (Image obj in ssTimeObj) { obj.gameObject.SetActive(false); }
                sBack.gameObject.SetActive(true);
                ssBack.gameObject.SetActive(false);
                for (int i = 0; i < 3; i++) { MatChange(sTimeObj, staffTime, i); }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (Image obj in pTimeObj) { obj.gameObject.SetActive(false); }
            foreach (Image obj in sTimeObj) { obj.gameObject.SetActive(false); }
            foreach (Image obj in ssTimeObj) { obj.gameObject.SetActive(false); }
            sBack.gameObject.SetActive(false);
            ssBack.gameObject.SetActive(false);
        }
    }

    void MatChange(Image[] target,int[] time,int digit) /* digitに桁数を入れるとその桁のマテリアルを交換してくれる */
    {
        int ten = time[digit] / 10;
        int one = ten > 0 ? time[digit] - ten * 10 : time[digit]; /* 10の位がある場合1の位はDigit-10の位で出す */
        if (ten > 9) { return; } /* tenが2桁以上(100の位が存在する)場合終了 */

        if (digit == 0) { target[0].material = numMat[one]; }
        else
        {
            target[(digit - 1) * 2 + 1].material = numMat[one];
            target[digit * 2].material = numMat[ten];
        }
    }
}
