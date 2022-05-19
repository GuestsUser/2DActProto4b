using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBGMFade : MonoBehaviour
{
    [Header("ステージ移動時、BGMにFadeをかけるScript")]
    [Tooltip("Harukoコンポネント")] [SerializeField] StageSelect select;
    [Tooltip("音量")] [SerializeField] float volume;
    [Tooltip("BGMのAudioSource")] [SerializeField] AudioSource _audio;
    [Tooltip("処理動作確認用フラグ")] [SerializeField] private bool volume_down;

    // Start is called before the first frame update
    void Start()
    {
        /* 必要なコンポネントの取得 */
        _audio = GetComponent<AudioSource>();
        select = GameObject.Find("Haruko").GetComponent<StageSelect>();

        /* 変数の初期化 */
        volume = 0.01f;
        _audio.volume = 0.2f;

        /* フラグの初期化 */
        volume_down = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (select.startVF)
        {
            FadeVolume();
        }
    }
    public void FadeVolume()
    {
        /* 処理確認用フラグをオン！インスペクターで確認出来る */
        volume_down = true;

        /* 音量を毎フレーム下げていく */
        if (_audio.volume > 0) _audio.volume -= volume;
    }
}
