using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInCheck : MonoBehaviour
{
    /* RendererがついてないとOnBecame系が反応しないようなのでRendererが付いてるbodyにつけた */
    static private bool _playerVisible = true;
    static public bool playerVisible { get { return _playerVisible; } }

    private void Start()
    {
        _playerVisible = true;
    }

    /* 画面に映った瞬間実行 */
    void OnBecameVisible()
    {
        _playerVisible = true;
    }

    /* 画面外に消えた瞬間実行 */
    void OnBecameInvisible()
    {
        _playerVisible = false;
    }
}
