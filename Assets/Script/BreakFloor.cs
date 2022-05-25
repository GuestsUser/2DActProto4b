using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SESystem))]
public class BreakFloor : MonoBehaviour
{
    [Header("プレイヤーが乗ると崩れる")]
    [Tooltip("乗ってから崩れるまでの時間、秒指定")] [SerializeField] float notice = 1;
    [Tooltip("この時間分経過すると復活、秒指定")] [SerializeField] float respawn = 5;
    [Tooltip("この変数にマテリアルが入っていると乗った時、消えるまでの時間オブジェクトをこのマテリアルに変更")] [SerializeField] Material noticeMaterial;
    [Tooltip("乗った瞬間の演出用マテリアル")] [SerializeField] Material whiteEffectMat;
    [Tooltip("乗った瞬間の演出時間")] [SerializeField] float whiteEffectTime = 0.04f;

    List<GameObject> allObj; /* このオブジェクトと子のオブジェクト全て */
    List<Color32> childColor; /* 子のオブジェクトの元の色 */
    Color32 clear;/* 透明色 */
    Color32 white;

    List<bool> isCollider; /* コンポーネントの有無チェック */
    List<bool> isRender; /* 同上 */

    bool useFlg = false; /* 破壊と再生のコルーチンが使用中か否かフラグ */
    Material originMat; /* 元のマテリアル */

    SESystem sound;

    // Start is called before the first frame update
    void Start()
    {
        allObj = new List<GameObject>();
        childColor = new List<Color32>();
        clear = new Color32(255, 255, 255, 0);
        white = new Color32(186, 35, 128, 255);
        isCollider = new List<bool>();
        isRender = new List<bool>();
        sound = GetComponent<SESystem>();

        /* 関数の方では自身を格納できないので先に入れておく */
        allObj.Add(gameObject);

        isCollider.Add(GetComponent<Collider>() != null); /* コライダーの有無チェックはキャッシュ以上の意味を持たない…… */
        isRender.Add(GetComponent<MeshRenderer>() != null); /* MeshRendererがついていればtrueを格納 */
        if (isRender[0])
        {
            originMat = gameObject.GetComponent<MeshRenderer>().material;
            childColor.Add(originMat.color);
        }

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

            if (!flg) { StartCoroutine(WhiteEffect()); }
            for (float i = 0; i < limit;) /* 待機 */
            {
                i += Time.deltaTime;
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }

            if (flg)
            {
                ColorReset();
                if (noticeMaterial != null) { MaterialChange(originMat); }
            }
            else
            {
                sound.audioSource.PlayOneShot(sound.se[sound.IndexToSub("break")]);
                ColorChange(clear);
            }
            for (int i = 0; i < allObj.Count; i++) { if (isCollider[i]) { allObj[i].GetComponent<Collider>().enabled = flg; } } /* 所属オブジェクトの当たり判定の削除と復活 */
        }
        useFlg = false; /* 解放 */
    }

    IEnumerator WhiteEffect()
    {
        float count = 0;

        if (noticeMaterial != null) { MaterialChange(whiteEffectMat); }
        while (count < whiteEffectTime)
        {
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        MaterialChange(noticeMaterial);
    }

    void MaterialChange(Material mat)
    {
        for (int i = 0; i < allObj.Count; i++)
        {
            if (isRender[i]) { allObj[i].GetComponent<MeshRenderer>().material = mat; }
        }
    }

    void ColorChange(Color32 c)
    {
        for (int i = 0; i < allObj.Count; i++)
        {
            if (isRender[i]) { allObj[i].GetComponent<MeshRenderer>().material.color = c; }
        }
    }

    void ColorReset()
    {
        int sub = 0; /* childColorはmeshrendererがついてないオブジェクトは飛ばすのでallObjと添え字が合わない事態を回避する為独自に添え字を用意してた */
        for (int i = 0; i < allObj.Count; i++) /* 所属オブジェクトの透明度を元に戻す */
        {
            if (isRender[i])
            {
                allObj[i].GetComponent<MeshRenderer>().material.color = childColor[sub];
                sub++;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player" && !useFlg)
        {
            sound.audioSource.PlayOneShot(sound.se[sound.IndexToSub("ride")]);
            StartCoroutine(Del());
        }
    }
}
