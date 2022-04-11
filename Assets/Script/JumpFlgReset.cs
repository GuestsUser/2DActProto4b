using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プレイヤーに付けて使う */
[RequireComponent(typeof(JumpSystem))] /* ジャンプ必須化 */
public class JumpFlgReset : MonoBehaviour
{
    private JumpSystem jump; /* ジャンプシステム */
    // Start is called before the first frame update
    void Start()
    {
        jump = GetComponent<JumpSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        jump.completion = false; /* 一番最後に実行する事で押された瞬間だけtrueとして処理を書ける */
    }
}
