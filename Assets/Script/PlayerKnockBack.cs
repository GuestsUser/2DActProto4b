using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    [Header("ノックバック機能")]
    [Tooltip("ノックバックの力、計算にCosを用いてるので最初は代入されたそのままの数値、時間が半分経過すると急激に速度低下という風になる")] [SerializeField] float force = 0.1f;
    [Tooltip("ノックバック持続時間")] [SerializeField] float time = 0.4f;

    private static float angle; /* 吹っ飛ばす方向 */
    private static bool runOrder; /* trueのコルーチンの実行を更新 */
    private Rigidbody rb; /* リジットボディ記憶用 */

    private static bool _runState = false; /* ノックバック実行状況 */
    public static bool runState { get { return _runState; } } /* 上記のプロパティ */

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(KnockBackSystem());
    }

    public static void KnockBack(float ag, DamageSystem.DamageCombo dc) /* この関数は必ずダメージを与える関数の前に呼び出す事 */
    {
        if (dc > DamageSystem.playerConbo) /* コンボ値が現在値を上回った場合のみ上書き */
        {
            angle = ag;
            runOrder = true;
        }
    }
    private IEnumerator KnockBackSystem() /* 常に実行しておくタイプのやつ */
    {
        float count = 0;
        Vector3 editPos = Vector3.zero; /* ノックバック計算結果の座標記録用 */
        DamageSystem.DamageCombo memo = DamageSystem.playerConbo; /* 現在コンボ値記憶用 */

        while (true)
        {
            while (!runOrder) { yield return StartCoroutine(TimeScaleYield.TimeStop()); } /* 平常から変化があるまで待機 */

            Reset(); /* 実行前に初期化 */

            while (count < time) /* time分実行 */
            {
                rb.velocity = Vector3.zero; /* ノックバック用に加速度停止 */
                count += Time.deltaTime;
                if (runOrder) { Reset(); } /* 割り込みがあればリセット */

                /* 自身の位置を直接弄ってる訳ではないので壁を貫通してしまう問題の解決から */
                /* 直接弄ると操作が可能なので普通に移動できてしまう問題が…… */

                float rate = 90 / time * count * Mathf.Deg2Rad; /* countがtimeと同値になった時90*deg2radになる式 */
                //editPos.x += force * Mathf.Cos(angle) * Mathf.Cos(rate); /* force*angleで方向決定、*rateで移動量決定 */
                //editPos.y += force * Mathf.Sin(angle) * Mathf.Cos(rate); /* yは方向決定にSinを用いる */
                editPos.x = transform.position.x + force * Mathf.Cos(angle) * Mathf.Cos(rate); /* force*angleで方向決定、*rateで移動量決定 */
                editPos.y = transform.position.y + force * Mathf.Sin(angle) * Mathf.Cos(rate); /* yは方向決定にSinを用いる */
                transform.position = editPos;

                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }
            PlayerMove.MoveRestrictionRelease();
            PlayerMove.RotateRestrictionRelease();
            _runState = false;

            while (DamageSystem.playerConbo == memo && !runOrder) { yield return StartCoroutine(TimeScaleYield.TimeStop()); } /* 無敵時間終了か上書きまで待機 */
        }

        void Reset()
        {
            StartCoroutine(PlayerMove.MoveRestriction());
            StartCoroutine(PlayerMove.RotateRestriction());
            editPos = transform.position; /* 変化があった瞬間の座標記録 */
            memo = DamageSystem.playerConbo;
            count = 0;
            _runState = true;
            runOrder = false; /* 更新したので更新命令を更新 */
        }
    }
}
