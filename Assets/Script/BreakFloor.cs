using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BreakFloor : MonoBehaviour
{
    [Header("プレイヤーが乗ると崩れる")]
    [Tooltip("乗ってから崩れるまでの時間、秒指定")] [SerializeField] float notice = 1;
    [Tooltip("この時間分経過すると復活、秒指定")] [SerializeField] float respawn = 5;

    List<GameObject> allObj; /* このオブジェクトと子のオブジェクト全て */
    List<Color32> childColor; /* 子のオブジェクトの元の色 */
    Color32 clear;/* 透明色 */

    bool useFlg = false; /* 破壊と再生のコルーチンが使用中か否かフラグ */

    // Start is called before the first frame update
    void Start()
    {
        allObj = new List<GameObject>();
        childColor = new List<Color32>();
        clear = new Color32(255, 255, 255, 0);

        /* 関数の方では自身を格納できないので先に入れておく */
        allObj.Add(gameObject);
        childColor.Add(gameObject.GetComponent<MeshRenderer>().material.color);
        GetChildren(gameObject);
        
    }
    private void GetChildren(GameObject obj) /* listオブジェクトに格納する為の関数 */
    {
        foreach(Transform child in transform)
        {
            allObj.Add(child.gameObject);
            childColor.Add(child.gameObject.GetComponent<MeshRenderer>().material.color);
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
            for (int i= 0; i < allObj.Count; i++) /* 所属オブジェクトの透明度と当たり判定の削除 */
            {
                allObj[i].GetComponent<MeshRenderer>().material.color = flg ? childColor[i] : clear;
                allObj[i].GetComponent<Collider>().enabled = flg;
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
