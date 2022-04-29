using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LocalSaveSystem))]
[RequireComponent(typeof(LocalSavePara))]
public class ChoicesNavigatGizmosDraw : MonoBehaviour
{
    [Header("選択肢の自作ナビゲーション表示")]
    [Tooltip("gizmos表示用eventSystem")] [SerializeField] EventSystem es;
    [Tooltip("添え字の0,1,2,3がそれぞれ上、下、左、右の方向のgizmos色に対応")] [SerializeField] Color32[] colorUDLR;

    LocalSavePara para; /* セーブUI用パラメーター */

    private void Reset()
    {
        para = GetComponent<LocalSavePara>();
    }

    void OnDrawGizmos()
    {
        if (es == null || para.choices == null) { return; } /* eventSystameが空、若しくはchoices未精製の場合gizmos描写しない */

        int drawMax = colorUDLR.Length > 4 ? 4 : colorUDLR.Length; /* 色配列が4個より多くても4つに丸める他、4つ以下でもいいようlengthを利用する */
        GameObject select = es.currentSelectedGameObject; /* 選択中オブジェクト取得、何を選択してもエディター上でnullになってたのでそこから */

        for(int i = 0; i < para.choices.Length; i++) /* navでselectと一致したオブジェクトの添え字が必要なのでforeachではなくfor文で行く */
        {
            if (select == para.choices[i]) /* 現在選択中オブジェクトが選択肢オブジェクトの場合 */
            {
                Vector3 root = select.transform.position; /* 選択中オブジェクトの位置 */
                for (int j = 0; j < drawMax; j++)
                {
                    Gizmos.color = colorUDLR[j]; /* 色設定 */
                    Gizmos.DrawLine(root, para.choices[para.nav[i, j]].transform.position); /* 選択中オブジェクトからナビゲーションの示す先へラインを引く */
                }
                return;
            }
        }

    }
}
