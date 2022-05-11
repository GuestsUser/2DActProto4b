using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    // フレームレートを60に固定する場合つける
    void Start()
    {
        Application.targetFrameRate = 60;//FPS60化
        Time.fixedDeltaTime = 1f / 60;
    }

}
