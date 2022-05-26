using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RamAttack : MonoBehaviour
{
    [Header("自機接触でダメージを与える(仮)")]
    [Tooltip("接触時ダメージ")] [SerializeField] float damage = 1.0f;
    [Tooltip("ダメージの種類")] [SerializeField] DamageSystem.DamageCombo type = DamageSystem.DamageCombo.damage;

    [Tooltip("中心座標を足元に持ってくる為にyスケールの半分を持つ変数、元々足元にあったり中心座標がオブジェクトの中心以外にある時は自力で計算するか0にする")] [SerializeField] private float radius;

    private void Reset() /* Resetが押された時実行 */
    {
        radius = transform.localScale.y / 2; /* yサイズの半分を自動取得 */
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(Damage(other.gameObject));
            //Debug.Log("プレイヤーに当たりました");
        }
    }

    private IEnumerator Damage(GameObject other) /* 接触判定と敵削除の順番を削除優先にする */
    {
        yield return StartCoroutine(TimeScaleYield.TimeStop());
        float angle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad; /* z回転(ラジアンで取得) */
        Vector3 adjust = new Vector3(radius * Mathf.Sin(angle), radius * Mathf.Cos(angle)); /* 現在の角度に合わせた中心から足元までの距離を取得 */

        Vector3 distance = other.transform.position - (transform.position - adjust);
        PlayerKnockBack.KnockBack(Mathf.Atan2(distance.y, distance.x), type); /* 向かってきた方向と逆方向に吹っ飛ばす */
        DamageSystem.Damage(damage, type); /* ダメージを与える */
    }
}
