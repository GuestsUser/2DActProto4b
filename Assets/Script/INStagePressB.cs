using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class INStagePressB : MonoBehaviour
{
    [Header("StageSelectのBでステージにはいるテキスト表示切替")]
    [Tooltip("表示オブジェクト")] [SerializeField] private GameObject press_b;
    [Tooltip("当たり判定")] [SerializeField] private bool hit;
    [Tooltip("StageObjと表示Objの相対距離")] [SerializeField] private float offset_y;

    // Start is called before the first frame update
    void Start()
    {
        press_b = GameObject.Find("PressBbutton");
        press_b.SetActive(false);
        hit = false;

    }
    private void OnTriggerEnter(Collider other)
    {
        /* もしステージオブジェクトに当たったら */
        if (other.tag == "StageObj")
        {
            var pos_x = other.transform.position.x;
            var pos_y = other.transform.position.y;


            hit = true; /* 当たっている判定 */
            press_b.SetActive(true); /* オブジェクトを表示 */

            if (other.name == "Stage1") /* ステージ１なら */
            {
                press_b.transform.position = new Vector3(pos_x, pos_y - offset_y, press_b.transform.position.z);
            }
            else if (other.name == "Stage2") /* ステージ2なら */
            {
                press_b.transform.position = new Vector3(pos_x, pos_y - offset_y, press_b.transform.position.z);
            }
            else if (other.name == "Stage3") /* ステージ3なら */
            {
                press_b.transform.position = new Vector3(pos_x, pos_y - offset_y, press_b.transform.position.z);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        /* ステージオブジェクトから離れたら */
        if (other.tag == "StageObj")
        {
            hit = false; /* 当たっていない判定に切り替え */
            press_b.SetActive(false); /* オブジェクトを非表示 */
        }
    }

    private float Ydistance(GameObject pos1, GameObject pos2)
    {
        /* y座標の取得 */
        var y_pos1 = pos1.transform.position.y;
        var y_pos2 = pos2.transform.position.y;

        var distance = 0f; /* 初期化 */

        if (y_pos1 > y_pos2)
        {
            distance = y_pos1 - y_pos2;
        }
        else
        {
            distance = y_pos2 - y_pos1;
        }
        return distance;
    }

}
