using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RetrySystem))] /* リトライ必須化 */
/* ダメージを無効化したり食らう予定のダメージ値を取得したり等の機能が必要な場合仲里に相談を */
public class DamageSystem : MonoBehaviour
{
    /* 連続ダメージ阻止用連鎖値システム */
    /* ダメージ関数の後ろにDamageComboの値を入れ、直近に受けたCombo値を記憶し、直近Combo値以下のダメージを無効化する事で連続ヒットを防ぐ */
    /* DamageComboの値は必要に応じて増やしてよし */
    public enum DamageCombo
    {
        neutral = 0, /* 平常状態用(ダメージを受けてない状態)、追加する値はこれ以上である必要あり */
        damage = 5, /* 通常ダメージ用 */
        falldown = 9999 /* 落下用、追加する値はこれ以下である必要あり */
    }

    private static float damage = 0; /* 外部からダメージを受け取るための変数 */
    private static float recover = 0; /* 外部から回復値を受け取るための変数 */
    private static DamageCombo combo = DamageCombo.neutral; /* static化したくなかった…… */

    public static DamageCombo playerConbo { get { return combo; } } //comboの取得用

    [Header("ライフと残機のシステム")]
    [Tooltip("ライフ画像、ヒエラルキーに配置済みの画像を入れる事でその位置を基準にライフを生成")] [SerializeField] RawImage imageLife;
    [Tooltip("ライフの縁、背景画像")] [SerializeField] RawImage imageBack; /* 【糸数】5月2日追加 */
    [Tooltip("ライフ横配置間隔")] [SerializeField] int placeSpace;
    [Tooltip("ライフ最大値")] [SerializeField] float maxLife = 3;
    [Tooltip("ライフ現在値")] [SerializeField] float life = 3;
    [Tooltip("残機最大値")] [SerializeField] int maxRemain = 99;
    [Tooltip("残機現在値")] [SerializeField] int remain = 4;

    [Header("以下無敵時間とその演出用")]
    [Tooltip("無敵時間")] [SerializeField] float invincible = 2.5f;
    [Tooltip("ダメージを受けた瞬間、このマテリアルのalbedoとemissionをこのオブジェクトに設定されてるマテリアルに設定する")] [SerializeField] Material hitColor;
    [Tooltip("ダメージを受けた後無敵時間が切れるまで、このマテリアルのalbedoとemissionをこのオブジェクトに設定されてるマテリアルに設定する")] [SerializeField] Material invincibleColor;

    private List<RawImage> drawLife; /* ライフ画像描写用 */
    private List<RawImage> drawBack; /* ライフ画像描写用 */

    private Vector3 imageScale; /* 画像の初期拡大率 */
    private Rect imageRect; /* 画像初期uvRect */
    private float imageWidth; /* 画像横幅 */

    private RetrySystem rty; /* リトライシステム */

    /*ダメージSEようの変数*/
    AudioSource damageSouce;
    [SerializeField] AudioClip damageSe;
    /*ダメージSEようの変数*/

    // Start is called before the first frame update
    void Start()
    {
        maxLife = PlayerPrefs.GetFloat("maxLife", maxLife);
        life = PlayerPrefs.GetFloat("life", life);
        maxRemain = PlayerPrefs.GetInt("maxRemain", maxRemain);
        remain = PlayerPrefs.GetInt("remain", remain);

        placeSpace = -20; /* 【糸数】5月3日追加　ビルドで見たときの間隔がこれくらいが良かった */

        /* static変数の初期化 */
        damage = 0;
        recover = 0;
        combo = DamageCombo.neutral;

        rty = gameObject.GetComponent<RetrySystem>(); /* リトライスクリプトを挿入 */

        /* 元画像処理 */
        imageScale = imageLife.rectTransform.localScale; /* 元画像初期拡大率格納 */
        imageRect = imageLife.uvRect; /* 元画像uvRect格納 */
        imageWidth = imageLife.rectTransform.rect.width; /* 元画像横幅格納 */
        imageLife.gameObject.SetActive(false); /* 元画像は使わない */
        imageBack.gameObject.SetActive(false); /* 【糸数】5月2日追加 */ 

        drawLife = new List<RawImage>();
        drawLife.Capacity = 0;

        /* 【糸数】5月2日追加 */
        drawBack = new List<RawImage>();
        drawBack.Capacity = 0;
        /* 【糸数】5月2日追加終了 */

        AddLife((int)Mathf.Ceil(maxLife)); /* ライフ描写用画像用意用 */

        /* 表示更新*/
        LifeVisibleChange();
        DrawSizeChange();

        StartCoroutine(InvincibleSystem()); /* コンボ値を無敵時間に合わせてリセットしてくれる */

        /*ダメージSEようの変数初期化*/
        damageSouce = GetComponent<AudioSource>();
        /*ダメージSEようの変数初期化*/

    }

    void LateUpdate()
    {

        if (damage != 0  || recover != 0) /* 更新があった場合だけ更新 */
        {

            float upDataLife = recover - damage;
            life += upDataLife;

            /*ダメージSEを鳴らす*/
            damageSouce.PlayOneShot(damageSe);
            /*ダメージSEを鳴らす*/

            /* 加算したのでリセット */
            damage = 0;
            recover = 0; 

            if (life < 0) { life = 0; } /* 最低値を下回らないように */
            if (life > maxLife) { life = maxLife; } /* 最大値に丸める */

            if (life <= 0) /* 体力が最低値に達した場合残機消費と体力を最大値に更新する */
            {
                rty.Retry();
                life = maxLife;
            }

            /* 表示更新*/
            LifeVisibleChange();
            DrawSizeChange();

            
        }
        DrawBack(); /*【糸数】5月2日追加*/
    }

    public static void Damage(float dmg, DamageCombo dc) /* dmgに入れた値分ダメージを与える */
    {
        if (combo < dc || dc==DamageCombo.falldown) /* 現在コンボ超過のダメージのみ加算する、落下死はコンボ値に関わらず受け取る */
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
            /* 【糸数】5月2日追加 */
            drawBack.Add(Instantiate(imageBack, imageBack.transform.parent)); /* 追加 */
            /* 【糸数】5月2日追加 */

            drawLife.Add(Instantiate(imageLife, imageLife.transform.parent)); /* 追加 */
            Vector3 pos = drawLife[i].rectTransform.position;
            pos.x += (imageWidth + imageWidth / 2 + placeSpace) * i; /* 個数に応じた配置ずらし */
            drawLife[i].rectTransform.position = pos;
            drawLife[i].gameObject.SetActive(false); /* 一旦不可視化する */

            /* 【糸数】5月2日追加 */
            drawBack[i].rectTransform.position = pos;
            drawBack[i].gameObject.SetActive(false); /* 一旦不可視化する */
            /* 【糸数】5月2日追加 */
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

        /* 【糸数】5月2日追加 */
        drawBack[current].rectTransform.localScale = scale;
        drawBack[current].uvRect = rect;
        /* 【糸数】5月2日追加 */
    }

    /* 【糸数】5月2日追加 */
    void DrawBack()
    {
        Debug.Log("ライフ背景描画");
        for (int i = 0; i < maxLife; i++){
            drawBack[i].rectTransform.localScale = imageScale;
            drawBack[i].uvRect = imageRect;
            drawBack[i].gameObject.SetActive(true); /* 表示切替 */
        }
    }

    private IEnumerator InvincibleSystem() /* 常に実行しておくタイプのやつ */
    {
        DamageCombo currentCombo;
        float count = 0;
        StartCoroutine(InvincibleEffect());
        while (true)
        {
            while(combo == DamageCombo.neutral) { yield return StartCoroutine(TimeScaleYield.TimeStop()); } /* 平常から変化があるまで待機 */

            currentCombo = combo; /* 変化した値にmeshセット */
            while (count < invincible) /* invincible分だけコンボ値維持 */
            {
                count += Time.deltaTime;
                if (currentCombo > combo) /* 割り込みがあれば時間リセット */
                {
                    currentCombo = combo;
                    count = 0;
                }
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }
            count = 0;
            combo = DamageCombo.neutral; /* コンボリセット */
        }

        IEnumerator InvincibleEffect() /* 無敵時間中演出 */
        {
            float hitBlinking = 0.07f; /* 点滅間隔 */
            float hitTime = invincible / 10; /* hitの演出時間 */

            List<Renderer> render = new List<Renderer>(); /* 編集アクセス用 */
            List<Material> originMat = new List<Material>(); /* リセット用元色 */
            List<Material> hitMat = new List<Material>(); /* ダメージを受けた瞬間用 */
            List<Material> invincibleMat = new List<Material>(); /* 無敵時間中透過用 */

            if (GetComponent<Renderer>() != null) /* GetChildは自身を格納しないので先に入れておく */
            {
                render.Add(GetComponent<Renderer>());
                originMat.Add(GetComponent<Renderer>().material);
                hitMat.Add(GetComponent<Renderer>().material);
                invincibleMat.Add(GetComponent<Renderer>().material);
            }
            GetChildren(gameObject);

            while (true)
            {
                while (combo == DamageCombo.neutral) { yield return StartCoroutine(TimeScaleYield.TimeStop()); } /* 平常から変化があるまで待機 */

                while (count < invincible) /* invincible分だけコンボ値維持 */
                {
                    if (count < hitTime)
                    {
                        for (int i = 0; i < render.Count; i++)
                        {
                            render[i].material = Mathf.FloorToInt(count / hitBlinking) % 2 == 0 ? hitMat[i] : originMat[i];
                        }
                    }
                    else
                    {
                        for (int i = 0; i < render.Count; i++) { render[i].material = invincibleMat[i]; }
                    }
                    yield return StartCoroutine(TimeScaleYield.TimeStop());
                }
                for (int i = 0; i < render.Count; i++) { render[i].material = originMat[i]; }
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }

            void GetChildren(GameObject obj) /* listオブジェクトに格納する為の関数 */
            {
                foreach (Transform child in obj.transform)
                {
                    if (child.GetComponent<Renderer>() != null)
                    {
                        render.Add(child.GetComponent<Renderer>());
                        originMat.Add(Instantiate(child.GetComponent<Renderer>().material));
                        hitMat.Add(Instantiate(child.GetComponent<Renderer>().material));
                        invincibleMat.Add(Instantiate(child.GetComponent<Renderer>().material));

                        int sub = originMat.Count - 1; /* 今回追加された要素の添え字 */

                        hitMat[sub].EnableKeyword("_EMISSION");
                        hitMat[sub].color = hitColor.color;
                        hitMat[sub].SetColor("_EmissionColor", hitColor.GetColor("_EmissionColor"));

                        invincibleMat[sub].DisableKeyword("_EMISSION");
                        invincibleMat[sub].color = invincibleColor.color;
                    }

                    GetChildren(child.gameObject); /* 再帰式にする事で子から孫まで全てを取得できる */
                }
            }
        }

    }
}
