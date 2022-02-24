using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeScaleYield : MonoBehaviour
{
    public static IEnumerator TimeStop() /* コルーチン内でこのコルーチンの実行が終了するまで待機を指示するとtime.scale==0の間は処理をこちらで受け止めてくれるヤツ */
    {
        do { yield return null; } while (Time.timeScale == 0); /* そのままyield return null だとtimescaleお構いなしなのでこれを使ってtimescaleによって処理を止める仕組みにできる */
    }
}
