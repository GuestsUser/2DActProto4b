using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOutKeyDisable : MonoBehaviour
{
    /* CameraInCheckはHarukoBodyに付いている、画面内にいるかどうか判定している */
    [Header("コルーチンを呼ぶとカメラに映るまで動作禁止化")]
    [Tooltip("画面に映った瞬間からこの時間分経過すると動けるようになる")] [SerializeField] float extend;

    private int runCount; /* KeyDisableコルーチン同時実行数、複数呼ばれても1つしか起動しないようにする */

    public IEnumerator KeyDisable()
    {
        if (runCount > 0) { yield break; }
        runCount++; /* 実行数加算 */

        StartCoroutine(PlayerMove.MoveRestriction());
        StartCoroutine(PlayerMove.RotateRestriction());
        StartCoroutine(JumpSystem.Restriction());
        StartCoroutine(DashSystemN.Restriction());

        do { yield return StartCoroutine(TimeScaleYield.TimeStop()); } while (!CameraInCheck.playerVisible); /* 後置処理にする事でプレイヤーがリトライで画面外に戻った時にフラグをfalseにしてくれる */

        float count = 0;
        while (count < extend)
        {
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

        runCount--; /* 実行開放 */
        PlayerMove.MoveRestrictionRelease();
        PlayerMove.RotateRestrictionRelease();
        JumpSystem.RestrictionRelease();
        DashSystemN.RestrictionRelease();
    }

    
}
