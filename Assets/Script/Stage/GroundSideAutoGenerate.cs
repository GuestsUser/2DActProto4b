using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundDownAutoGenerate))]
[RequireComponent(typeof(GroundColliderAutoResize))]
[RequireComponent(typeof(BoxCollider))]
public class GroundSideAutoGenerate : MonoBehaviour
{
    [Header("床の左右をメニューから自動補完")]
    [Header("注意:DownAutoGenerate実行後に実行する事")]
    [Tooltip("特に意味はない変数、変数ないとheader使えないから……")] [SerializeField] int hoge;

    [ContextMenu("自動補完開始")]
    private void Run()
    {
        for(int i = 0; i < transform.childCount; i++) /* 床の数だけ実行 */
        {
            Transform floor = transform.GetChild(i);
            for (int j = 0; j < floor.childCount; j++) { OriginalTools.DelChildren(floor.GetChild(j).gameObject); } /* 同じ階層(サイド床階層)を全削除しておく、この段階で消すことにより左と右のサイドが同じだった場合にも対応する */
        }

        Create(0, 90); /* 左側サイド、y軸90度回転 */
        Create(transform.childCount-1, 270); /* 右側サイド */

        void Create(int sub,float angle)
        {
            Transform root = transform.GetChild(sub);
            Vector3 rotate = Vector3.zero;
            rotate.y = angle; /* y軸回転の反映 */

            Vector3 scale = root.localScale;
            for (int j = 0; j < root.childCount; j++) /* 床下の数だけ実行 */
            {
                Transform down = root.GetChild(j); /* 床下オブジェクト */
                GameObject panel = Instantiate(down.gameObject);
                Vector3 setRotate = rotate;
                setRotate += down.localRotation.eulerAngles; //親になる床下オブジェクト回転状況を基本角度に合算

                panel.transform.parent = down;
                panel.transform.localPosition = Vector3.zero;
                panel.transform.localRotation = Quaternion.Euler(setRotate);
                panel.transform.localScale = scale;
            }
        }
    }
}
