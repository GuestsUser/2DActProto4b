using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LocalSaveSystem : MonoBehaviour
{
    [Header("セーブデータ保存システム")]
    [Tooltip("選択肢に使うオブジェクトのコピー元プレハブ")] [SerializeField] GameObject choicesPrefab;
    [Tooltip("選択中の選択肢を指すオブジェクト")] [SerializeField] GameObject selector;
    [Tooltip("ページスクロール可能かどうか表す矢印オブジェクト")] [SerializeField] GameObject[] scrollArrow;
    [Tooltip("下に敷かれてるパネル")] [SerializeField] GameObject basePanel;
    [Tooltip("セーブファイル名(拡張子除く)")] [SerializeField] string fileName= "save";
    [Tooltip("セーブファイル総数")] [SerializeField] int saveVol = 0;
    [Tooltip("セーブファイルの1ページにおける横縦配置数")] [SerializeField] Vector2Int layout;
    private Vector2Int layoutOld; /* レイアウト変更検知用記憶変数 */

    private float[] layoutPosX; /* 1ページ内の選択肢オブジェクト配置位置、こいつはx座標 */
    private float[] layoutPosY; /* 1ページ内の選択肢オブジェクト配置位置、こっちはy座標 */

    [SerializeField] private GameObject[] choices; /* 選択肢オブジェクト */
    private string path; /* セーブファイルパス */
    private LocalSave[] save; /* セーブファイル実体 */

    private Vector2Int select =Vector2Int.zero; /* 選択肢の現在位置記録用 */
    

    
    private enum moveType { LR,UD }
    [Tooltip("ページスクロール方向、lr=左右  ud=上下")] [SerializeField] moveType cursorMove;

    private void OnValidate() /* inspectorの値を変更した時呼び出される */
    {
        bool layoutChange = false;
        if (layout.x != layoutOld.x) /* レイアウトを変更された場合位置集を新しくする(x) */
        {
            NewLayout(ref layoutPosX, layout.x, basePanel.GetComponent<RectTransform>().sizeDelta.x);
            layoutOld.x = layout.x;
            layoutChange = true;
        }
        if (layout.y != layoutOld.y) /* レイアウトを変更された場合位置集を新しくする(y) */
        {
            NewLayout(ref layoutPosY, layout.y, basePanel.GetComponent<RectTransform>().sizeDelta.y);
            layoutOld.y = layout.y;
            layoutChange = true;
        }
        if (layoutChange)/* セーブファイル数を書き換えた時連動してtextのサイズも合わせられる */
        {
            if (choices.Length > 0) /* 一度作ったことがあるなら以前の物をすべて破棄 */
            {
                foreach(GameObject obj in choices) { Destroy(obj); } //上手く動いてないのでここから
            }
            choices = new GameObject[saveVol];
            NewChoice(cursorMove);
        }
        if (saveVol <= layout.x * layout.y) { foreach (GameObject obj in scrollArrow) { obj.SetActive(false); } }/* セーブ総数がレイアウトの配置総数を超えてない場合スクロール用矢印非表示化 */


        void NewLayout(ref float[] pos,int arrayVol,float placeSize) /* 新しいレイアウト座標作成関数 */
        {
            pos = new float[arrayVol];
            placeSize /= (arrayVol + 1);
            for (int i = 1; i < arrayVol + 1; i++) { pos[i - 1] = placeSize * i; }
        }

        void NewChoice(moveType type)
        {
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

            for (int i = 0; i < choices.Length; i++)
            {
                choices[i] = Instantiate(choicesPrefab); /* 新プレハブ精製 */
                choices[i].transform.parent = basePanel.transform; /* ベースパネルの子オブジェクト化 */
                choices[i].GetComponent<Text>().text = choices[i].GetComponent<Text>().text + (i + 1).ToString(); /* 表示テキスト後ろに数字を付加 */

                int unScrollSub = i % unScroll.Length; /* スクロールしない方向の添え字 */
                if (i / unScroll.Length < scroll.Length)
                {
                    choices[i].transform.position = NewPos(unScroll[unScrollSub], scroll[i / unScroll.Length]); /* スクロール対象側の座標使用添え字はiが非対象側の最大数に達する度増える */
                }
                else
                {
                    choices[i].transform.position = NewPos(unScroll[unScrollSub], scroll[scroll.Length - 1]); /* 対象側が最大値に達した場合最大値-1を使い続ける */
                    choices[i].SetActive(false); /* 不可視化もしておく */
                }
            }

            Vector3 NewPos(float un,float s) /* スクロール方向に応じたxy座標返し、unに非スクロール座標を入れる */
            {
                if (type == moveType.UD) { return new Vector3(un, s); }
                else { return new Vector3(s, un); }
            }
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/";
        save = new LocalSave[saveVol];
        for (int i = 0; i < saveVol; i++)
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
    }
    public void FileRead(int num)
    {
        string directory = path + fileName + num.ToString() + ".json";
        if (File.Exists(directory))
        {
            StreamReader st = new StreamReader(directory);
            string json = st.ReadToEnd();
            st.Close();
            save[num] = JsonUtility.FromJson<LocalSave>(directory);
        }
    }
}
