using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))] //audiosource必須化
public class SESystem : MonoBehaviour
{
    [Header("使用するseを入れる")]
    [Tooltip("使用するseを入れる")] [SerializeField] AudioClip[] _se;
    [Tooltip("seと同じ添え字のこの文字列をスクリプト上で疑似的にseの添え字として使える")] [SerializeField] string[] _index;

    private AudioSource _as;

    public AudioClip[] se { get { return _se; } }
    public string[] index { get { return _index; } }
    public AudioSource audioSource { get { return _as; } }
    
    private int se_size_memo = 0;
    private int index_size_memo = 0;

    public int IndexToSub(string num)
    {
        for(int i=0; i < index.Length; i++)
        {
            if (num == index[i]) { return i; } /* 入れられた名称が同じ位置の添え字を返す */
        }
        return -1;
    }

    void Start()
    {
        _as = GetComponent<AudioSource>(); //自身に付いてるaudiosourceを格納する
    }

    private void OnValidate()
    {
        if (_se.Length != se_size_memo) { Change(_se.Length); }
        if (_index.Length != index_size_memo) { Change(_index.Length); }

        void Change(int resize)
        {
            Array.Resize(ref _se, resize);
            Array.Resize(ref _index, resize);

            se_size_memo = resize;
            index_size_memo = resize;
        }
    }
}
