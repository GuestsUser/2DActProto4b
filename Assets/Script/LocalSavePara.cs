using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum moveType { LR, UD }

[RequireComponent(typeof(LocalSaveSystem))]
public class LocalSavePara : MonoBehaviour
{
    [Header("セーブデータ保存システムのパラメーター")]
    [Tooltip("need_resetとの表示があればLoaclSaveSystemをResetする必要あり")] [SerializeField] string systemReset = "match";
    [Tooltip("選択肢に使うオブジェクトのコピー元プレハブ")] [SerializeField] GameObject choicesPrefab;
    [Tooltip("選択中の選択肢を指すオブジェクト")] [SerializeField] GameObject selector;
    [Tooltip("ページスクロール可能かどうか表す矢印オブジェクト")] [SerializeField] GameObject[] scrollArrow;
    [Tooltip("下に敷かれてるパネル")] [SerializeField] GameObject basePanel;
    [Tooltip("セーブファイル名(拡張子除く)")] [SerializeField] string fileName = "save";
    [Tooltip("セーブファイル総数")] [SerializeField] int saveVol = 0;
    [Tooltip("セーブファイルの1ページにおける横縦配置数")] [SerializeField] Vector2Int layout;

    [Tooltip("ページスクロール方向、lr=左右  ud=上下")] [SerializeField] moveType cursorMove;

    public GameObject[] choices; /* 選択肢オブジェクト、作成順は左上から右下の順 */
    /* スクロール方向がUDならx方向に作ってゆき、x限界でyを一段下げる */
    /* LRならy方向に作り限界でxを一列進める */

    [HideInInspector] public int[,] nav; /* 選択肢オブジェクトの繋がり、謂わばナビゲーション、choicesの添え字で記録、添え字ならchoices作成ループが1回で済むメリットを優先 */
    /* nav[ 選択肢添え字 = i , 0～4の数値(順番に上、下、左、右に対応) = num ]で出てきた整数=nextとして  choices[i]からnum方向に移動した先のオブジェクトをchoices[next]で取得可能 */

    //[SerializeField] private EventSystem es=GameObject.Find("EventSystem").GetComponent<EventSystem>();

    /* 以下シリアライズ化された変数のプロパティ集 */
    public GameObject _choicesPrefab { get { return choicesPrefab; } }
    public GameObject _selector { get { return selector; } }
    public GameObject[] _scrollArrow { get { return scrollArrow; } }
    public GameObject _basePanel { get { return basePanel; } }
    public string _fileName { get { return fileName; } }
    public int _saveVol { get { return saveVol; } }
    public Vector2Int _layout { get { return layout; } }

    public moveType _cursorMove { get { return cursorMove; } }

    /* 以下Reset通知用コピー変数 */
    private GameObject memo_choicesPrefab;
    private GameObject memo_selector;
    private GameObject[] memo_scrollArrow;
    private GameObject memo_basePanel;
    private int memo_saveVol = 0;
    private Vector2Int memo_layout;
    private moveType memo_cursorMove;

    void OnValidate()
    {
        /* 選択肢作成に使ったパラメーターと差異が発生していればテキストをneed_resetに */
        if (memo_choicesPrefab != choicesPrefab) { MismatchText(); return; }
        if (memo_selector != selector) { MismatchText(); return; }

        if (memo_scrollArrow.Length != scrollArrow.Length) { MismatchText(); return; } /* サイズを更新していればその時点でneed_reset */
        for(int i=0;i< scrollArrow.Length; i++)
        {
            if (memo_scrollArrow[i] != scrollArrow[i]) { MismatchText(); return; } /* 中身をチェック */
        }

        if (memo_basePanel != basePanel) { MismatchText(); return; }
        if (memo_saveVol != saveVol) { MismatchText(); return; }
        if (memo_layout != layout) { MismatchText(); return; }
        if (memo_cursorMove != cursorMove) { MismatchText(); return; }

        systemReset = "match"; /* 一致していればmatchに */

        void MismatchText() { systemReset = "need_reset"; }

    }

    public void MatchCheckDateCopy() /* チェック用データの代入 */
    {
        memo_choicesPrefab = choicesPrefab;
        memo_selector = selector;
        memo_scrollArrow = scrollArrow;
        memo_basePanel = basePanel;
        memo_saveVol = saveVol;
        memo_layout = layout;
        memo_cursorMove = cursorMove;

        systemReset = "match"; /* 新しく生産したのでmatch */
    }

    void OnDrawGizmos()
    {

    }
}
