using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightMove : MonoBehaviour
{
    private enum direction { minus = 0, plus = 1 } /* 進行方向表現用 */

    [Header("反復移動(常に速度が一定タイプ)")]
    [Tooltip("この値分移動する")] [SerializeField] Vector3 move = Vector3.zero;
    [Tooltip("move分移動して戻ってくるまでの時間、秒指定")] [SerializeField] float limit = 0;
    [Tooltip("trueにすると初期地点を0とし0、move、0、-move、0(以下ループ)と移動する")] [SerializeField] bool inverse = false;

    private Vector3 iniPos; /* 初期位置記録 */
    private Vector3 addMove; /* フレーム毎の移動増加量 */
    private delegate bool Conditions(float a);
    private static Conditions[] con;
    // Start is called before the first frame update
    void Start()
    {
        if (con == null) { con = new Conditions[] { Minus, Plus }; } /* staticなので1回だけ代入 */       
        iniPos = transform.position;
        addMove = move / limit; /* inverseがfalseだろうがtrueだろうがフレーム間の移動量は変わらない */
        float velocity = 0;
        if (!inverse)
        {
            move /= 2; /* 移動量を半分に */
            iniPos += move; /* 記録上の初期位置に移動量の半分を加算 */
            velocity = -1; /* 初期移動状態を-1とすることで現在位置から始めることができる */
        }
        StartCoroutine(Repeat(velocity, direction.plus));
    }
    IEnumerator Repeat(float velocity, direction dic)
    {
        int sub = -1 + 2 * (int)dic;

        while (con[(int)dic](velocity))
        {
            transform.position = iniPos + move * velocity;
            velocity += Time.deltaTime / limit * sub;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        StartCoroutine(Repeat(dic == direction.plus ? 1 : -1, (direction)(((int)dic + 1) % 2)));
    }
    private static bool Plus(float a) { return a < 1 ; } /* +方向に進む継続条件 */
    private static bool Minus(float a) { return a > -1; } /* -方向に進む継続条件 */
}
