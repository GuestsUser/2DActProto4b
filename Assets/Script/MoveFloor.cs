using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerFollowFloor))] /* プレイヤーの床追従スクリプト必須化 */
public class MoveFloor : MonoBehaviour
{
    [Header("反復移動する")]
    [Tooltip("この値分移動する")] [SerializeField] Vector3 move = Vector3.zero;
    [Tooltip("move分移動して戻ってくるまでの時間、秒指定")] [SerializeField] float limit = 0;
    [Tooltip("trueにすると初期地点を0とし0、move、0、-move、0(以下ループ)と移動する")] [SerializeField] bool inverse = false;
    private Vector3 iniPos; /* 初期位置記録 */
    private Vector3 newPos; /* fixedUpdateで更新する新しい位置 */
    // Start is called before the first frame update
    void Start()
    {
        iniPos = transform.position;
        newPos = iniPos;
        float iniAngle = 0;
        if (!inverse)
        {
            move /= 2; /* 移動量を半分に */
            iniPos += move ; /* 記録上の初期位置に移動量の半分を加算 */
            iniAngle = -90; /* 初期移動状態を-1とすることで現在位置から始めることができる */
        }
        StartCoroutine(Repeat(iniAngle));
    }

    private void FixedUpdate() { transform.position = newPos; } /* 位置変更タイミングをFixedUpdateにする事でプレイヤーが床にめり込まず、疾走を通常速で扱える */

    IEnumerator Repeat(float iniAngle)
    {
        float count = 0;
        while (true)
        {
            //transform.position = iniPos + move * Mathf.Sin((float)((360 / limit * count + iniAngle) * Mathf.Deg2Rad));
            newPos = iniPos + move * Mathf.Sin((float)((360 / limit * count + iniAngle) * Mathf.Deg2Rad)); ;
            count = (count + Time.deltaTime) % limit;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        
    }
    
}
