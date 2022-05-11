using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GroundColliderAutoResize : MonoBehaviour
{
    
    [Header("ここを始点に")]
    [Header("〇〇〇〇〇")]
    [Header("↓")]
    [Header("注意:左上始点にすること")]
    [Header("子オブジェクト位置とColliderをResetで調整してくれる")]
    [Tooltip("need_resetとの表示があればこのコンポーネントをResetする必要あり")] [SerializeField] string systemReset = "match";
    [Tooltip("床個別のcolliderサイズ")] [SerializeField] Vector3 size = Vector3.zero;

    private BoxCollider box;
    private Vector3 centerMemo = Vector3.zero;
    private Vector3 sizeMemo = Vector3.zero;
    int childVolMemo = 0;

    [ContextMenu("自動補完開始")]
    private void Run()
    {
        /* コンポーネントゲット */
        box = GetComponent<BoxCollider>();
        childVolMemo = transform.childCount;
        centerMemo = box.center;
        sizeMemo = box.size;

        /* コンポーネント構成要素の変更 */
        sizeMemo.x = size.x * childVolMemo; /* colliderのx幅は子になってる床の数 */
        sizeMemo.y = size.y;
        centerMemo.x = size.x * childVolMemo / 2; /* xを子の中心に持ってくる */
        centerMemo.y = -size.y / 2;

        /* 更新 */
        box.center = centerMemo;
        box.size = sizeMemo;
        
        /* 子要素の位置更新 */
        for(int i=0;i< childVolMemo; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 pos = child.localPosition;
            pos.x = size.x * i + size.x / 2;
            child.localPosition = pos;
        }
    }

    void OnValidate()
    {
        /* 最後にResetした時とパラメーターに差異が発生していればテキストをneed_resetに */
        if (box == null) { MismatchText(); return; }
        if (transform.childCount != childVolMemo) { MismatchText(); return; }
        if (box.center.x != centerMemo.x) { MismatchText(); return; }
        if (box.size.x != sizeMemo.x) { MismatchText(); return; }

        systemReset = "match"; /* 一致していればmatchに */

        void MismatchText() { systemReset = "need_reset"; }

    }
}
