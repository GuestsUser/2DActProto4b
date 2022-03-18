using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* ダメージを無効化したり食らう予定のダメージ値を取得したり等の機能が必要な場合仲里に相談を */
public class DamageSystem : MonoBehaviour
{
    /* 連続ダメージ阻止用連鎖値システム */
    /* ダメージ関数の後ろにDamageComboの値を入れ、直近に受けたCombo値を記憶し、直近Combo値以下のダメージを無効化する事で連続ヒットを防ぐ */
    /* DamageComboの値は必要に応じて増やしてよし */
    public enum DamageCombo
    {
        neutral=0, /* 平常状態用(ダメージを受けてない状態)、追加する値はこれ以上である必要あり */
        damage=5, /* 通常ダメージ用 */
        falldown=9999 /* 落下用、追加する値はこれ以下である必要あり */
    }

    private static float damage = 0; /* 外部からダメージを受け取るための変数 */
    private static float recover = 0; /* 外部から回復値を受け取るための変数 */
    private static DamageCombo combo = DamageCombo.neutral; /* static化したくなかった…… */

    [Header("ライフと残機のシステム")]
    [Tooltip("ライフ画像、ヒエラルキーに配置済みの画像を入れる事でその位置を基準にライフを生成")] [SerializeField] RawImage imageLife;
    [Tooltip("ライフ横配置間隔")] [SerializeField] int placeSpace;
    [Tooltip("ライフ最大値")] [SerializeField] float maxLife = 3;
    [Tooltip("ライフ現在値")] [SerializeField] float life = 3;
    [Tooltip("残機最大値")] [SerializeField] int maxRemain = 99;
    [Tooltip("残機現在値")] [SerializeField] int remain = 4;
    
    private List<RawImage> drawLife; /* ライフ画像描写用 */

    private Vector3 imageScale; /* 画像の初期拡大率 */
    private Rect imageRect; /* 画像初期uvRect */
    private float imageWidth; /* 画像横幅 */
    
    // Start is called before the first frame update
    void Start()
    {
        maxLife = PlayerPrefs.GetFloat("maxLife", maxLife);
        life = PlayerPrefs.GetFloat("life", life);
        maxRemain = PlayerPrefs.GetInt("maxRemain", maxRemain);
        remain = PlayerPrefs.GetInt("remain", remain);

        /* 元画像処理 */
        imageScale = imageLife.rectTransform.localScale; /* 元画像初期拡大率格納 */
        imageRect = imageLife.uvRect; /* 元画像uvRect格納 */
        imageWidth = imageLife.rectTransform.rect.width; /* 元画像横幅格納 */
        imageLife.gameObject.SetActive(false); /* 元画像は使わない */

        drawLife = new List<RawImage>();
        drawLife.Capacity = 0;
        AddLife((int)Mathf.Ceil(maxLife)); /* ライフ描写用画像用意用 */

        /* 表示更新*/
        LifeVisibleChange();
        DrawSizeChange();
    }

    void LateUpdate()
    {
        if (damage != 0  || recover != 0) /* 更新があった場合だけ更新 */
        {
            float upDataLife = damage + recover;
            life += upDataLife;

            /* 加算したのでリセット */
            damage = 0;
            recover = 0; 

            if (life < 0) { life = 0; } /* 最低値を下回らないように */
            if (life > maxLife) { life = maxLife; } /* 最大値に丸める */

            /* 表示更新*/
            LifeVisibleChange();
            DrawSizeChange();
        }
    }

    public static void Damage(float dmg, DamageCombo dc) /* dmgに入れた値分ダメージを与える */
    {
        if (combo > dc) /* 現在コンボ超過のダメージのみ加算する */
        {
            damage += dmg;
            combo = dc;
            if (damage < 0) { damage = 0; } /* マイナス以下を許可しない */
        }
    }

    public static void Recover(float rc) /* rcの値分回復 */
    {
        recover += rc;
        if (recover < 0) { recover = 0; }
    }

    void AddLife(int addLife) /* ライフ最大数増加に使う、addLifeに増加値をいれる */
    {
        if (addLife <= 0) { return; } /* 0以下の値を指定してきた場合何もしない */

        drawLife.Capacity += addLife; /* キャパシティの新調 */
        maxLife = drawLife.Count + addLife; /* 最大数更新 */

        for (int i = drawLife.Count; i < maxLife; i++) /* 加算分だけ追加 */
        {
            drawLife.Add(Instantiate(imageLife, imageLife.transform.parent)); /* 追加 */
            Vector3 pos = drawLife[i].rectTransform.position;
            pos.x += (imageWidth + imageWidth / 2 + placeSpace) * i; /* 個数に応じた配置ずらし */
            drawLife[i].rectTransform.position = pos; 
            drawLife[i].gameObject.SetActive(false); /* 一旦不可視化する */

            /* 増加の際にアニメを追加するならここにすると増加した分のライフを引数に渡せていい感じ */
        }
    }

    void LifeVisibleChange() /* 現在体力に応じて画像の描写可不可の切り替えを行う */
    {
        int border = ((int)Mathf.Ceil(life)); /* この値未満の添え字のライフを表示する */
        for (int i = 0; i < drawLife.Count; i++)
        {
            drawLife[i].rectTransform.localScale = imageScale;
            drawLife[i].uvRect = imageRect;
            drawLife[i].gameObject.SetActive(i < border); /* 表示切替 */
        }
    }

    void DrawSizeChange() /* 現在体力に小数点が含まれる場合その値に応じて画像を隠す(描写域と拡大率操作で隠したように見せる)関数 */
    {
        int current = ((int)Mathf.Ceil(life)) - 1; /* ceilと-1の組合せならlifeに小数点が含まれなくても拡大率に1を返す事ができる */
        if (current < 0) { return; } /* lifeが0の状態で入るとcurrentが-1を返すのでその時は何もしない */

        Vector3 scale = imageScale;
        Rect rect = imageRect;

        /* xを小数点状態に合わせたサイズに更新 */
        scale.x = imageScale.x * (life - current);
        rect.width= imageRect.width * (life - current);

        /* 変更の代入 */
        drawLife[current].rectTransform.localScale = scale;
        drawLife[current].uvRect = rect;
    }
}
