using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LocalSavePara))]
public class LocalSaveSystem : MonoBehaviour
{
    delegate void NavigatFunction(int sub);

    private float[] layoutPosX; /* 1ページ内の選択肢オブジェクト配置位置、こいつはx座標 */
    private float[] layoutPosY; /* 1ページ内の選択肢オブジェクト配置位置、こっちはy座標 */

    private string path; /* セーブファイルパス */
    private LocalSave[] save; /* セーブファイル実体 */
    LocalSavePara para; /* セーブUI用パラメーター */

    [HideInInspector] [SerializeField] Vector2 panelSize; /* basePanelのサイズ */

    private void Reset()
    {
        para = GetComponent<LocalSavePara>();
        if (para.choices != null) { foreach (GameObject obj in para.choices) { DestroyImmediate(obj); } } /* 何か入ってた場合消しておく */

        panelSize = para._basePanel.GetComponent<RectTransform>().sizeDelta;
        layoutPosX = NewLayout(para._layout.x, panelSize.x);
        layoutPosY = NewLayout(para._layout.y, panelSize.y);

        para.choices = new GameObject[para._saveVol];
        para.nav = new int[para._saveVol, 4];
        NewChoice(para._cursorMove);
        if (para._saveVol <= para._layout.x * para._layout.y) { foreach (GameObject obj in para._scrollArrow) { obj.SetActive(false); } } /* セーブ総数がレイアウトの配置総数を超えてない場合スクロール用矢印非表示化 */
        else { foreach (GameObject obj in para._scrollArrow) { obj.SetActive(true); } } /* そうじゃなければ表示する */

        para.MatchCheckDateCopy();
    }

    float[] NewLayout(int arrayVol, float placeSize) /* 新しいレイアウト座標作成関数 */
    {
        float[]  pos = new float[arrayVol];
        placeSize /= (arrayVol + 1);
        for (int i = 1; i < arrayVol + 1; i++) { pos[i - 1] = placeSize * i; }
        return pos;
    }

    void NewChoice(moveType type)
    {
        NavigatFunction NavigatCreate = (type == moveType.LR) ? (NavigatFunction)NavigatCreateLR : NavigatCreateUD; /* 毎回条件式を通したくなかったのでデリゲート使用 */

        float[] unScroll;
        float[] scroll;

        if (type == moveType.UD)
        {
            unScroll = layoutPosX;
            scroll = layoutPosY;
        }
        else
        {
            unScroll = layoutPosY;
            scroll = layoutPosX;
        }

        string nameBase = para._choicesPrefab.GetComponent<Text>().text; /* テキストベース、これの後ろに数値を付加する */
        string objName = para._choicesPrefab.name;/* オブジェクト名ベース */
        for (int i = 0; i < para.choices.Length; i++)
        {
            para.choices[i] = Instantiate(para._choicesPrefab); /* 新プレハブ精製 */
            para.choices[i].transform.parent = para._basePanel.transform; /* ベースパネルの子オブジェクト化 */
            para.choices[i].GetComponent<Text>().text = nameBase + (i + 1).ToString(); /* 表示テキスト後ろに数字を付加 */
            para.choices[i].name = objName + (i + 1).ToString(); /* オブジェクト名更新 */
            NavigatCreate(i); /* ナビゲーション作成 */

            //NavigatCreateUD(i); /* ナビゲーション作成 */
            //Debug.Log((i + 1).ToString() + "回目");
            //for (int j = 0; j < 4; j++) { Debug.Log(para.nav[i, j]); }


            int unScrollSub = i % unScroll.Length; /* スクロールしない方向の添え字 */
            if (i / unScroll.Length < scroll.Length)
            {
                para.choices[i].transform.localPosition = NewPos(unScroll[unScrollSub], scroll[i / unScroll.Length]); /* スクロール対象側の座標使用添え字はiが非対象側の最大数に達する度増える */
                para.choices[i].SetActive(true); /* 元プレハブが非表示状態でも表示されるよう明示的にtrue設定 */
            }
            else
            {
                para.choices[i].transform.localPosition = NewPos(unScroll[unScrollSub], scroll[scroll.Length - 1]); /* 対象側が最大値に達した場合最大値-1を使い続ける */
                para.choices[i].SetActive(false); /* 不可視化もしておく */
            }
        }

        void NavigatCreateUD(int sub) /* udタイプのナビゲーション作成 */
        {
            /* 抜け補完を実施する事で */
            /*  〇 〇 〇 〇  */
            /*  〇 〇 〇 〇  */
            /*  〇 〇        */
            /* このような形の位置[0,3]で上を押した時[1,3]に移動するようにできる */
            /* 現在は上下移動で左右は動かない、上の形なら[0,4]で上を押した場合[1,3]に動くような挙動ではない */

            int endLine = para._saveVol / para._layout.x + 1; /* 最下段の数値 */
            int currentLine = sub / para._layout.x + 1; /* subの所属段数 */
            int root = (currentLine - 1) * para._layout.x; /* 今回段の最小値 */
            int max = para._layout.x * endLine; /* 最下段にあるかもしれない抜けを補完した値 */

            /* 上 */
            if (currentLine <= 1) /* 添え字が1段目の場合 */
            {
                para.nav[sub, 0] = max + sub - para._layout.x; /* 抜け補完環境における移動先添え字 */
                if (para.nav[sub, 0] >= para._saveVol) { para.nav[sub, 0] -= para._layout.x; } /* 抜けの位置を指定された場合一段上げる */

                if (para._layout.y <= 1) { para.nav[sub, 0] = sub; } /* 一段しかない場合自身を指す */
            }
            else { para.nav[sub, 0] = sub - para._layout.x; }

            /* 下 */
            if (sub >= para._saveVol - para._layout.x)  /* 下が抜けになっていたりでこれ以上下がない場合 */
            {
                para.nav[sub, 1] = (sub + para._layout.x) % max; /* 補完で余りを出す事でy軸位置報を損失しない */
                if (para.nav[sub, 1] >= para._saveVol) { para.nav[sub, 1] = (para.nav[sub, 1] + para._layout.x) % max; } /* 抜けの位置を指定された場合一段下げて余りを出す */

                if (para._layout.y <= 1) { para.nav[sub, 1] = sub; } /* 一段しかない場合自身を指す */
            }
            else { para.nav[sub, 1] = sub + para._layout.x; }


            if (currentLine == endLine) /* 最下段の場合 */
            {
                int mod = para._saveVol - root; /* 抜けを排除した今回段の配置個数 */
                int use = sub - root; /* 添え字を今回の所属段元からの値に変換 */
                para.nav[sub, 2] = root + ((mod + use - 1) % mod); /* 左 加算値をmodにする事で抜けを考慮した配置になる */
                para.nav[sub, 3] = root + ((use + 1) % mod); /* 右 */
            }
            else
            {
                para.nav[sub, 2] = root + ((currentLine * para._layout.x + sub - 1) % para._layout.x); /* 単純な sub - 1 % layout.x ではなく currentLine * para._layout.x を挟む事で0の位置でも正常に動くようになっている */
                para.nav[sub, 3] = root + ((sub + 1) % para._layout.x);
            }

            /* 以下端に着いた場合段移動するタイプのコード */
            /* 左 */
            //if (sub - 1 < 0) { para.nav[sub, 2] = para._saveVol - 1; } /* マイナスに入る場合一番後ろに移動する */
            //else { para.nav[sub, 2] = sub - 1; }
            /* 右 */
            //para.nav[sub, 3] = (sub + 1) % para._saveVol;
        }

        void NavigatCreateLR(int sub) /* lrタイプのナビゲーション作成 */
        {
            int endLine = para._saveVol / para._layout.y + 1; /* 最下段の数値 */
            int currentLine = sub / para._layout.y + 1; /* subの所属段数 */
            int root = (currentLine - 1) * para._layout.y; /* 今回段の最小値 */
            int max = para._layout.y * endLine; /* 最下段にあるかもしれない抜けを補完した値 */

            if (currentLine == endLine) /* 最後列の場合 */
            {
                int mod = para._saveVol - root; /* 抜けを排除した今回段の配置個数 */
                int use = sub - root; /* 添え字を今回の所属段元からの値に変換 */
                para.nav[sub, 0] = root + ((mod + use - 1) % mod); /* 上 加算値をmodにする事で抜けを考慮した配置になる */
                para.nav[sub, 1] = root + ((use + 1) % mod); /* 下 */
            }
            else
            {
                para.nav[sub, 0] = root + ((currentLine * para._layout.y + sub - 1) % para._layout.y);
                para.nav[sub, 1] = root + ((sub + 1) % para._layout.y);
            }

            /* 左 */
            if (currentLine <= 1) /* 添え字が1列目の場合 */
            {
                para.nav[sub, 2] = max + sub - para._layout.y; /* 抜け補完環境における移動先添え字 */
                if (para.nav[sub, 2] >= para._saveVol) { para.nav[sub, 2] -= para._layout.y; } /* 抜けの位置を指定された場合一列上げる */

                if (para._layout.x <= 1) { para.nav[sub, 2] = sub; } /* 一列しかない場合自身を指す */
            }
            else { para.nav[sub, 2] = sub - para._layout.y; }

            /* 右 */
            if (sub >= para._saveVol - para._layout.y)  /* 下が抜けになっていたりでこれ以上下がない場合 */
            {
                para.nav[sub, 3] = (sub + para._layout.y) % max; /* 補完で余りを出す事でx軸位置報を損失しない */
                if (para.nav[sub, 3] >= para._saveVol) { para.nav[sub, 3] = (para.nav[sub, 3] + para._layout.y) % max; } /* 抜けの位置を指定された場合一列下げて余りを出す */

                if (para._layout.x <= 1) { para.nav[sub, 3] = sub; } /* 一列しかない場合自身を指す */
            }
            else { para.nav[sub, 3] = sub + para._layout.y; }
        }

        Vector3 NewPos(float un, float s) /* スクロール方向に応じたxy座標返し、unに非スクロール座標を入れる */
        {
            /* パネルの中心から左右対称に配置したいのでpanelSizeの半分を引く */
            /* yは加算で上に上がるので半分を加算してlayoutの進行で値を引く */
            if (type == moveType.UD) { return new Vector3(un - panelSize.x / 2, panelSize.y / 2 - s); }
            else { return new Vector3(s - panelSize.x / 2, panelSize.y / 2 - un); }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/";
        save = new LocalSave[para._saveVol];
        for (int i = 0; i < para._saveVol; i++)
        {
            FileRead(i + 1);
            //foreach(GameObject obj in text.)
        }
        //save[i].version = Application.version;
        FileWrite();


    }
    public void FileWrite()
    {
        string json = JsonUtility.ToJson(save);
        StreamWriter st = new StreamWriter(path);
        st.Write(json);
        st.Flush();
        st.Close();

        Navigation nv = new Navigation();
        nv.selectOnDown= para.choices[0].GetComponent<Button>();
        para.choices[0].GetComponent<Button>().navigation = nv;
    }
    public void FileRead(int num)
    {
        string directory = path + para._fileName + num.ToString() + ".json";
        if (File.Exists(directory))
        {
            StreamReader st = new StreamReader(directory);
            string json = st.ReadToEnd();
            st.Close();
            save[num] = JsonUtility.FromJson<LocalSave>(directory);
        }
    }
}
