using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TimeAttack : MonoBehaviour
{
    [Header("タイムアタック時間計測")]
    [Tooltip("数値のマテリアル")] [SerializeField] Material[] numMat;
    [Tooltip("数値のオブジェクト、桁の小さい順に格納、位毎に格納")] [SerializeField] Image[] numObj;

    int[] numDigit; /* 各桁数数値、桁の小さい順に並ぶ、秒の1の位、秒の10の位という風の格納ではなく、ミリ秒、秒、分の各桁数毎の格納になっている */
    int digitVol = 5; /* 桁数、現在桁数調整機能はついてない */
    int maxMinutes = 99;

    // Start is called before the first frame update
    void Start()
    {
        numDigit = new int[Mathf.CeilToInt(digitVol / 2f)];
        for(int i = 0; i < numDigit.Length; i++) { numDigit[i] = 0; }
        StartCoroutine(TimeCount());
    }

    private IEnumerator TimeCount()
    {
        float count = 0;
        while (true)
        {
            int progressTime = (int)(count * 10); /* 前回yieldから進んだ第一位ミリ秒単位 */
            if (progressTime > 0) /* 100ミリ秒以上の進行があった場合実行 */
            {
                count -= progressTime / 10f;
                for (int i = 0; i < numDigit.Length; i++)
                {
                    int sub = Convert.ToInt32(i); /* 0以外なら1、0なら0を返す数値 */
                    numDigit[i] += progressTime;
                    progressTime = numDigit[i] / (10 + 50 * sub);

                    if (progressTime <= 0)
                    {
                        MatChange(i);
                        break;
                    }
                    numDigit[i] -= progressTime * (10 + 50 * sub);
                    MatChange(i);

                }

                if (numDigit[numDigit.Length - 1] > maxMinutes) /* 最大値を超えた場合範囲内に納めてコルーチンも終了する */
                {
                    numDigit[0] = 9;
                    numDigit[1] = 59;
                    numDigit[2] = maxMinutes;
                    for(int i=0;i< numDigit.Length; i++) { MatChange(i); }
                    break;
                }
                PlayerPrefs.SetInt("timeMin", numDigit[2]);
                PlayerPrefs.SetInt("timeSec", numDigit[1]);
                PlayerPrefs.SetInt("timeMil", numDigit[0]);
                PlayerPrefs.SetInt("timeScore", numDigit[0] + numDigit[1] * 10 + numDigit[2] * 600);
            }
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
    }
    void MatChange(int digit) /* digitに桁数を入れるとその桁のマテリアルを交換してくれる */
    {
        int ten = numDigit[digit] / 10;
        int one = ten > 0 ? numDigit[digit] - ten * 10 : numDigit[digit]; /* 10の位がある場合1の位はDigit-10の位で出す */
        if (ten > 9) { return; } /* tenが2桁以上(100の位が存在する)場合終了 */

        if (digit == 0) { numObj[0].material = numMat[one]; }
        else
        {
            numObj[(digit - 1) * 2 + 1].material = numMat[one];
            numObj[digit * 2].material = numMat[ten];
        }
    }

    static public int ScoreCalculat(int mil,int sec,int min) /* 時間表示をint型数値に変換 */
    {
        return mil + sec * 10 + min * 600;
    }

    static public void BestTimeUpDate(int stageNum)
    {
        int[] pTime = { PlayerPrefs.GetInt("timeMil", 9), PlayerPrefs.GetInt("timeSec", 59), PlayerPrefs.GetInt("timeMin", 99) };
        int pScore = ScoreCalculat(pTime[0], pTime[1], pTime[2]);

        int[] bTime = new int[3];
        int bScore;

        switch (stageNum)
        {
            case 1:
                bTime[0] = PlayerPrefs.GetInt("bestSt1TimeMil", 9);
                bTime[1] = PlayerPrefs.GetInt("bestSt1TimeSec", 59);
                bTime[2] = PlayerPrefs.GetInt("bestSt1TimeMin", 99);
                bScore = ScoreCalculat(bTime[0], bTime[1], bTime[2]);
                if (pScore < bScore)
                {
                    PlayerPrefs.SetInt("bestSt1TimeMil", pTime[0]);
                    PlayerPrefs.SetInt("bestSt1TimeSec", pTime[1]);
                    PlayerPrefs.SetInt("bestSt1TimeMin", pTime[2]);
                }
                break;
            case 2:
                bTime[0] = PlayerPrefs.GetInt("bestSt2TimeMil", 9);
                bTime[1] = PlayerPrefs.GetInt("bestSt2TimeSec", 59);
                bTime[2] = PlayerPrefs.GetInt("bestSt2TimeMin", 99);
                bScore = ScoreCalculat(bTime[0], bTime[1], bTime[2]);
                if (pScore < bScore)
                {
                    PlayerPrefs.SetInt("bestSt2TimeMil", pTime[0]);
                    PlayerPrefs.SetInt("bestSt2TimeSec", pTime[1]);
                    PlayerPrefs.SetInt("bestSt2TimeMin", pTime[2]);
                }
                break;
            case 3:
                bTime[0] = PlayerPrefs.GetInt("bestSt3TimeMil", 9);
                bTime[1] = PlayerPrefs.GetInt("bestSt3TimeSec", 59);
                bTime[2] = PlayerPrefs.GetInt("bestSt3TimeMin", 99);
                bScore = ScoreCalculat(bTime[0], bTime[1], bTime[2]);
                if (pScore < bScore)
                {
                    PlayerPrefs.SetInt("bestSt3TimeMil", pTime[0]);
                    PlayerPrefs.SetInt("bestSt3TimeSec", pTime[1]);
                    PlayerPrefs.SetInt("bestSt3TimeMin", pTime[2]);
                }
                break;


        }

    }
}
