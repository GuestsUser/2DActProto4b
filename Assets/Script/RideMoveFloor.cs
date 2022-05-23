using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerFollowFloor))] /* プレイヤーの床追従スクリプト必須化 */
public class RideMoveFloor : MonoBehaviour
{
    [Header("プレイヤーが触ると動き出す")]
    [Tooltip("この値分移動する")] [SerializeField] Vector3 move = Vector3.zero;
    [Tooltip("move分移動するまでの時間、秒指定")] [SerializeField] float limit = 0;
    [Tooltip("乗ってから実際に移動を開始するまでの待ち時間")] [SerializeField] float delay = 2;
    [Tooltip("これがtrueなら目標地点に着いた後プレイヤーが再び乗るまで現在位置で待機する")] [SerializeField] bool goalWait = false;

    [Tooltip("これがtrueならプレイヤーがやられると床が元の位置に戻る")] [SerializeField] bool respawnReset = false;
    //[Tooltip("プレイヤーやられ時床が元の位置に戻るまでの時間")] [SerializeField] float respawnBackTime = 0.7f;

    private enum PointFlag { ini = 0, end, move } /* 現在の移動状態を示すフラグ値、iniは初期位置、moveは移動中、endは目標位置にいる */
    Vector3 iniPos; /* 初期位置記録 */
    PointFlag flg = PointFlag.ini; /* 現在の移動状態保持用フラグ */
    Vector3 newPos; /* fixedUpdateで更新する新しい位置 */

    private RetrySystem retrySys; /* 元の位置に戻す為のタイミング把握用プレイヤー保持変数、格納はCollisionEnterで行う(そもそもプレイヤーの乗ってない床は動いてないから戻す必要なし) */

    // Start is called before the first frame update
    void Start()
    {
        iniPos = transform.position;
        newPos = iniPos;
        move /= 2; /* 移動量を半分に */
        iniPos += move; /* 記録上の初期位置に移動量の半分を加算 */
    }

    private void FixedUpdate() { transform.position = newPos; } /* 位置変更タイミングをFixedUpdateにする事でプレイヤーが床にめり込まず、疾走を通常速で扱える */

    private void LateUpdate()
    {
        if(retrySys != null && retrySys.isRetry)
        {
            retrySys = null;
            flg = PointFlag.ini;
            //transform.position = iniPos - move;
            newPos = iniPos - move;
        }
    }

    IEnumerator Move()
    {
        while (true) /* ループにする事で一度乗ったらここ以外でのflgは常にmoveを返す様になり起動タスクを1つにできる */
        {
            float iniAngle = 90 * (-1 + 2 * ((int)flg)); /* flgがiniなら0で-90、endなら1で90を角度とする仕組み */
            flg = PointFlag.move;
            for (float i = 0; i < delay;) /* 乗っても実際に動き出すまでにラグを出す */
            {
                if (retrySys != null && retrySys.isRetry) { yield break; }
                i += Time.deltaTime;
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }
            
            for (float count = 0; count < limit;) /* 移動部分 */
            {
                if (retrySys != null && retrySys.isRetry) { yield break; }
                newPos = iniPos + move * Mathf.Sin((float)((180 / limit * count + iniAngle) * Mathf.Deg2Rad));
                //transform.position = iniPos + move * Mathf.Sin((float)((180 / limit * count + iniAngle) * Mathf.Deg2Rad));
                count += Time.deltaTime;
                yield return StartCoroutine(TimeScaleYield.TimeStop());
            }
            float code = Mathf.Sin((float)((iniAngle + 180) * Mathf.Deg2Rad)); /* 初期角に180を足す事で対岸へ綺麗に移動できる */
            newPos = iniPos + move * code;
            //transform.position = iniPos + move * code;

            if (code > 0) { flg = PointFlag.end; } /* sinで返ってきた値が1なら目標地点、-1なら初期地点に到達したことになる */
            else { flg = PointFlag.ini; }

            if (goalWait) { break; } /* goalWaitがtrueなら一度終了し再度乗ったら再び動き出す */
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (flg != PointFlag.move)
            {
                retrySys = other.gameObject.GetComponent<RetrySystem>();
                StartCoroutine(Move());
            }
        }
    }
}
