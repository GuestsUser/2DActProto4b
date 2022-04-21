using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* プレイヤーに付けて使う */
[RequireComponent(typeof(GroundFooter))] /* 接地判定必須化 */
public class FooterReset : MonoBehaviour
{
    private GroundFooter footer;
    // Start is called before the first frame update
    void Start()
    {
        footer = GetComponent<GroundFooter>();
    }

    private void LateUpdate()
    {
        footer.isGround = false; /* 最後にfalse化する */
        footer.floor = null; /* 空にする */
    }
}
