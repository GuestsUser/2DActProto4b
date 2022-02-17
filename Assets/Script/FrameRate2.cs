using System.Collections;
using System.Threading;
using UnityEngine;
public class FrameRate2 : MonoBehaviour
{
    /*秒間フレームと固定フレームを変更する処理参照元： https://blog.unity.com/ja/technology/precise-framerates-in-unity */

    public float Rate = 60.0f;/*秒間フレームレートを変更する変数(Update関数)*/
    public float FixedRate = 60f;/*固定フレームレートを変更する変数（FixedUpdate関数）*/
    float currentFrameTime;/*目標フレームレート*/
    void Start()
    {
        Time.fixedDeltaTime = 1f / FixedRate;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 9999;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine("WaitForNextFrame");
    }
    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            currentFrameTime += 1.0f / Rate;
            var t = Time.realtimeSinceStartup;
            var sleepTime = currentFrameTime - t - 0.01f;
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));
            while (t < currentFrameTime)
                t = Time.realtimeSinceStartup;
        }
    }
}