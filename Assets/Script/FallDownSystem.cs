using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RetrySystem))] /* リトライスクリプト必須化 */
public class FallDownSystem : MonoBehaviour
{
    [Header("落下死ステージのプレイヤーに付ける")]
    [Tooltip("落下死地点集、x座標の整数部が重ならないようお願いしたい")] [SerializeField] private List<GameObject> fallPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        fallPoint.Sort((a, b) => (int)((a.transform.position.x - b.transform.position.x) * 1000)); /* 1000掛ける事で第3位までの小数点を整数部に持ってくる */
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int useSub = fallPoint.Count - 1;
        for (int i=0;i< fallPoint.Count; i++)
        {
            if (transform.position.x <= fallPoint[i].transform.position.x)
            {
                if (i == 0) { return; } /* 初めの位置より小さい位置にプレイヤーがいる場合終了 */
                else
                {
                    useSub = i - 1; /* プレイヤーより小さく、最も近い位置の添え字を記録 */
                    break;
                }
            }
        }
        if(transform.position.y <= fallPoint[useSub].transform.position.y) /* プレイヤー位置が落下死ポイントのy座標以下の場合即死判定 */
        {
            DamageSystem.Damage(9999, DamageSystem.DamageCombo.falldown);
        }
    }
}
