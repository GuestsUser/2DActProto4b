using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxSt2 : MonoBehaviour
{
    public StageClear StageClear;

    /*各ステージ宝箱の状態*/
    [SerializeField] private GameObject Stage2TB;
    [SerializeField] private GameObject Stage2TBOpen;

    void Start()
    {

    }

    void Update()
    {
        /*ステージのクリアフラグがたつとポールのみのオブジェクトが非アクティブ化され、旗のフラグがアクティブ化される*/

        if (StageClear._isStage2Clear)
        {
            Stage2TB.gameObject.SetActive(false);
            Stage2TBOpen.gameObject.SetActive(true);
        }
    }



}
