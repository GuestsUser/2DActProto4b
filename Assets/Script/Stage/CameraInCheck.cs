using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInCheck : MonoBehaviour
{
    static private bool _playerVisible = true;
    static public bool playerVisible { get { return _playerVisible; } }

    private void Start()
    {
        _playerVisible = true;
    }

    void OnBecameVisible()
    {
        _playerVisible = true;
    }

    void OnBecameInvisible()
    {
        _playerVisible = false;
    }
}
