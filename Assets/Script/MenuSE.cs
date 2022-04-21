using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))] //audiosource必須化

public class MenuSE : MonoBehaviour //seの音を保持しておく為のコンポーネント
{
    public AudioClip decision; //決定音
    public AudioClip cancel; //キャンセル音
    public AudioClip move; //カーソル移動音
    public AudioClip open_menu; //メニュー表示音
    public AudioClip close_menu; //メニュー閉じる音

    private AudioSource _audio_source; //音を鳴らすためのaudiosource

    public AudioClip d_se { get { return decision; } }
    public AudioClip c_se { get { return cancel; } }
    public AudioClip move_se { get { return move; } }
    public AudioSource audio_source { get { return _audio_source; } }

    // Start is called before the first frame update
    void Start()
    {
        _audio_source = GetComponent<AudioSource>(); //自身に付いてるaudiosourceを格納する
    }

}