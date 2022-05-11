using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginalTools : MonoBehaviour /* 便利な関数集 */
{
    /* 配列削除に関するtips */
    /* DestroyImmediateによる即消しによりforeach内の添え字が狂った可能性大 */
    /* 0 1 2 3 4 → 1 2 3 4 → 1 3 4  → 結果 1 3*/
    /* 実             実           実*/
    /* for文に変えて条件を カウンタ<childCount にしても同様のことが起きた */
    /* 配列先頭が消えると詰められるようだ */

    static public void DelChildren(GameObject obj) /* 引数オブジェクトの孫も含めた全子要素削除、引数自身は消さない */
    {
        int childMax = obj.transform.childCount; /* 条件にchildcountを含むとDestroyImmediateにより要素数が変わってしまうので記録しておく */
        for (int i = 0; i < childMax; i++) { DelLoop(obj.transform.GetChild(0).gameObject); }/* 要素が実行毎に前倒しされるので添え字は0でよし */

        void DelLoop(GameObject del)
        {
            int delMax = del.transform.childCount;
            for (int i = 0; i < delMax; i++) { DelLoop(del.transform.GetChild(0).gameObject); }/* 再帰式にする事で子から孫まで全てを取得できる */
            DestroyImmediate(del); /* 末端の子から消してゆく */
        }
    }
}
