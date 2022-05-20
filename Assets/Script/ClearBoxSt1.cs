using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBoxSt1 : MonoBehaviour
{
    public StageClear StageClear;

    /*各ステージ宝箱の状態*/
    [SerializeField] private GameObject Stage1TB;
    [SerializeField] private GameObject Stage1TBOpen;

    void Start()
    {

    }

    void Update()
    {
        /*ステージのクリアフラグがたつとポールのみのオブジェクトが非アクティブ化され、旗のフラグがアクティブ化される*/

        if (StageClear._isStage1Clear)
        {
            Stage1TB.gameObject.SetActive(false);
            Stage1TBOpen.gameObject.SetActive(true);
        }
    }
}
