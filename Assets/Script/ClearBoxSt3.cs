using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxSt3 : MonoBehaviour
{
    public StageClear StageClear;

    /*各ステージ宝箱の状態*/
    [SerializeField] private GameObject Stage3TB;
    [SerializeField] private GameObject Stage3TBOpen;

    void Start()
    {

    }

    void Update()
    {
        /*ステージのクリアフラグがたつとポールのみのオブジェクトが非アクティブ化され、旗のフラグがアクティブ化される*/

        if (StageClear._isStage2Clear)
        {
            Stage3TB.gameObject.SetActive(false);
            Stage3TBOpen.gameObject.SetActive(true);
        }
    }



}
