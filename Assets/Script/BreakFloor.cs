using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BreakFloor : MonoBehaviour
{
    [Header("プレイヤーが乗ると崩れる")]
    [Tooltip("乗ってから崩れるまでの時間、秒指定")] [SerializeField] float notice = 1;
    [Tooltip("この時間分経過すると復活、秒指定")] [SerializeField] float respawn = 5;
    [Tooltip("この変数にマテリアルが入っていると乗った時、消えるまでの時間オブジェクトをこのマテリアルに変更")] [SerializeField] Material noticeMaterial;

    List<GameObject> allObj; /* このオブジェクトと子のオブジェクト全て */
    List<Color32> childColor; /* 子のオブジェクトの元の色 */
    Color32 clear;/* 透明色 */

    List<bool> isCollider; /* コンポーネントの有無チェック */
    List<bool> isRender; /* 同上 */

    bool useFlg = false; /* 破壊と再生のコルーチンが使用中か否かフラグ */

    // Start is called before the first frame update
    void Start()
    {
        allObj = new List<GameObject>();
        childColor = new List<Color32>();
        clear = new Color32(255, 255, 255, 0);
        isCollider = new List<bool>();
        isRender = new List<bool>();

        /* 関数の方では自身を格納できないので先に入れておく */
        allObj.Add(gameObject);

        isCollider.Add(GetComponent<Collider>() != null); /* コライダーの有無チェックはキャッシュ以上の意味を持たない…… */
        isRender.Add(GetComponent<MeshRenderer>() != null); /* MeshRendererがついていればtrueを格納 */
        if (isRender[0]) { childColor.Add(gameObject.GetComponent<MeshRenderer>().material.color); }

        GetChildren(gameObject);
    }
    private void GetChildren(GameObject obj) /* listオブジェクトに格納する為の関数 */
    {
        foreach(Transform child in obj.transform)
        {
            allObj.Add(child.gameObject);

            isCollider.Add(child.GetComponent<Collider>() != null);
            isRender.Add(child.GetComponent<MeshRenderer>() != null); /* MeshRendererがついていればtrueを格納 */
            if (isRender[isRender.Count-1]) { childColor.Add(child.GetComponent<MeshRenderer>().material.color); }
            
            GetChildren(child.gameObject); /* 再帰式にする事で子から孫まで全てを取得できる */
        }
    }
    IEnumerator Del() /* 壊したり復活したりするコルーチン */
    {
        useFlg = true; /* 使用中 */
        for (int j = 0; j < 2; j++)
        {
            bool flg = Convert.ToBoolean(j); /* 現在の繰り返し回数を元にactiveに用いるフラグの決定 */
            float limit = flg ? respawn : notice; /* フラグの状態によって用いるべき時間の決定 */
            for (float i = 0; i < limit;) /* 待機 */
            {
                i += Time.deltaTime;
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }
            int sub = 0; /* childColorはmeshrendererがついてないオブジェクトは飛ばすのでallObjと添え字が合わない事態を回避する為独自に添え字を用意してた */
            for (int i= 0; i < allObj.Count; i++) /* 所属オブジェクトの透明度と当たり判定の削除 */
            {
                if (isRender[i])
                {
                    allObj[i].GetComponent<MeshRenderer>().material.color = flg ? childColor[sub] : clear;
                    sub++;
                }
                if (isCollider[i]) { allObj[i].GetComponent<Collider>().enabled = flg; }
            }
        }
        useFlg = false; /* 解放 */
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && !useFlg)
        {
            StartCoroutine(Del());
        }
    }
}
