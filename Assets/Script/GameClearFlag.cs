using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameClearFlag : MonoBehaviour
{
    public StageClear StageClear;
    
    /*各ステージクリア確認用旗*/
    [SerializeField] private GameObject Stage1pl;
    [SerializeField] private GameObject Stage1;
    [SerializeField] private GameObject Stage2pl;
    [SerializeField] private GameObject Stage2;
    [SerializeField] private GameObject Stage3pl;
    [SerializeField] private GameObject Stage3;

    /*各ステージ宝箱の状態*/
    [SerializeField] private GameObject Stage1TB;
    [SerializeField] private GameObject Stage1TBOpne;
    [SerializeField] private GameObject Stage2TB;
    [SerializeField] private GameObject Stage2TBOpen;
    [SerializeField] private GameObject Stage3TB;
    [SerializeField] private GameObject Stage3TBOpen;

    void Start()
    {

    }

    void Update()
    {
        /*ステージのクリアフラグがたつとポールのみのオブジェクトが非アクティブ化され、旗のフラグがアクティブ化される*/
        if (StageClear._isStage1Clear)
        {
            Stage1pl.gameObject.SetActive(false);
            Stage1.gameObject.SetActive(true);
            Stage1TB.gameObject.SetActive(false);
            Stage1TBOpne.gameObject.SetActive(true);
        }
        if (StageClear._isStage2Clear)
        {
            Stage2pl.gameObject.SetActive(false);
            Stage2.gameObject.SetActive(true);
            Stage1TB.gameObject.SetActive(false);
            Stage1TBOpne.gameObject.SetActive(true);
        }
        if (StageClear._isStage3Clear)
        {
            Stage3pl.gameObject.SetActive(false);
            Stage3.gameObject.SetActive(true);
            Stage1TB.gameObject.SetActive(false);
            Stage1TBOpne.gameObject.SetActive(true);
        }
    }



}
