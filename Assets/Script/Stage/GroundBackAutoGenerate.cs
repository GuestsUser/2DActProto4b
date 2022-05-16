using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundColliderAutoResize))]
[RequireComponent(typeof(BoxCollider))]
public class GroundBackAutoGenerate : MonoBehaviour
{
    [Header("床の真下をメニューから自動補完")]
    [Tooltip("このオブジェクトを複製する、配置位置はこのオブジェクトの座標をそのままローカル座標とする")] [SerializeField] GameObject prefab;

    [ContextMenu("自動補完開始")]
    private void Run()
    {
        Vector3 pos = prefab.transform.position; /* 複製元座標記録 */
        Vector3 scale = prefab.transform.localScale; /* 複製元サイズ */
        Quaternion rotate = prefab.transform.localRotation; /* 複製元回転 */
        int floorFinal = transform.GetChild(0).transform.childCount; /* 床下の最終添え字 */
        if (floorFinal > 0)
        {
            for (int i = 0; i < transform.childCount; i++) { CreateBack(transform.GetChild(i).transform.GetChild(floorFinal - 1)); }/* 各床の最下段に作成 */
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++) { CreateBack(transform.GetChild(i)); }
        }

        void CreateBack(Transform parent)
        {
            GameObject panel = Instantiate(prefab);
            panel.transform.parent = parent;
            panel.transform.localPosition = pos;
            panel.transform.localScale = scale;
            panel.transform.localRotation = rotate;
        }
    }
    [ContextMenu("床下以下削除")]
    private void Clean()
    {
        for (int i = 0; i < transform.childCount; i++) { OriginalTools.DelChildren(transform.GetChild(i).gameObject); }
    }
}
