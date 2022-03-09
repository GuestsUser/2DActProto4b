using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeFloor : MonoBehaviour
{
    [Header("乗ると滑る床")]
    [Tooltip("乗っている間加え続けられる力")] [SerializeField] float force = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<SlopeDash>().standSlopeObj = gameObject;
        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            Vector3 angle = transform.rotation.eulerAngles;
            other.gameObject.GetComponent<Rigidbody>().AddForce(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), 0, ForceMode.Acceleration);
            /* 下の処理はHaruko置き換えの際に起きたvelocity消滅の原因調査用 */
            //Vector3 power = other.gameObject.GetComponent<Rigidbody>().velocity;
            //power.x += force * Mathf.Cos(angle.z * Mathf.Deg2Rad);
            //power.y += force * Mathf.Sin(angle.z * Mathf.Deg2Rad);
            //other.gameObject.GetComponent<Rigidbody>().velocity = power;
            //Debug.Log(other.gameObject.GetComponent<Rigidbody>().velocity);
        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject.GetComponent<SlopeDash>().standSlopeObj==gameObject) /* 乗ってる床に自身と関係ないオブジェクトが入っている可能性を取り除く */
        {
            other.gameObject.GetComponent<SlopeDash>().standSlopeObj = null; /* 乗っている床が自身だった場合に限り変数を空にする */
        }
    }
}
