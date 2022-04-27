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
        if (es != null && para.choices != null) { return; } /* eventSystameが空、若しくはchoices未精製の場合gizmos描写しない */

        GameObject select = es.currentSelectedGameObject; /* 選択中オブジェクト取得 */
        foreach (GameObject obj in para.choices) /* 生成 */
        {
            if (select == obj)
            {

                return;
            }
        }

    }
}
