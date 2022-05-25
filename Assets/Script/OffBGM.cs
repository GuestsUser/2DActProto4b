using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffBGM : MonoBehaviour
{
    [Header("BGMを簡単に止めれるスクリプト")]
    [Tooltip("true:bgmを止める false:bgmを流す")] public bool stop_bgm;
    [Tooltip("BGMがアタッチされているAudioSourceが入る")] [SerializeField] private AudioSource _audio;
    

    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (stop_bgm)
        {
            _audio.clip = null;
        }
        else
        {
            /* 何もしない */
        }
    }
}
