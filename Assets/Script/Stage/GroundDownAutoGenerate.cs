using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundColliderAutoResize))]
[RequireComponent(typeof(BoxCollider))]
public class GroundDownAutoGenerate : MonoBehaviour
{
    [Header("床の下の部分をメニューから自動補完してくれる")]
    [Header("注意:AutoResize実行後に実行する事")]
    [Tooltip("このオブジェクトで補完する")] [SerializeField] GameObject obj;
    [Tooltip("補完オブジェクトの配置を床からこの値分ずらした位置から始める")] [SerializeField] Vector3 firstAdjust = Vector3.zero;
    [Tooltip("補完オブジェクトが増える度座標にこの値を加算する")] [SerializeField] Vector3 adjust = Vector3.zero;
    [Tooltip("補完オブジェクトのcolliderサイズ、xサイズは床と違うって事はないと思う……")] [SerializeField] float size = 0;
    [Tooltip("床下の数")] [SerializeField] int vol;

    [ContextMenu("自動補完開始")]
    private void Run()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 hit = box.size;
        Vector3 center = box.center;

        hit.y += size * vol;
        center.y -= size / 2 * vol;

        box.size = hit;
        box.center = center;

        Vector3 scale = obj.transform.localScale;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform root = transform.GetChild(i);
            Vector3 rootPos = root.transform.localPosition;
            OriginalTools.DelChildren(root.gameObject); /* 個別の床が持つ孫も含めた全子オブジェクト削除 */

            for(int j = 0; j < vol; j++)
            {
                GameObject panel = Instantiate(obj);
                panel.transform.parent = root;
                panel.transform.localPosition = firstAdjust + adjust * j; /* 床ローカル座標+初期ずらし+増分ずらし(初回は初期ずらしのみなのでjに+1する必要なし) */
                panel.transform.localScale = scale;
            }
        }
        
    }
}
