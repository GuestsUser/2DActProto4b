/*敵を踏んで倒すスクリプト*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillAside : MonoBehaviour
{
    public Vector3 rayPosition; /*レイキャストの位置*/

    /*レイキャスト用変数*/
    public Ray ray;
    public RaycastHit rayHit;
    [SerializeField] public float rayDistance = 0.1f;

    /*リジッドボディー*/
    public Rigidbody rb;

    /*AddForce*/
    public float upForce = 300f;    /*上方向に向ける力*/

    void Start()
    {
        rb = GetComponent<Rigidbody>(); /*リジッドボデー*/
    }


    void Update()
    {

        rayPosition = rb.transform.position; /*レイキャストの位置*/
        /*レイキャストの位置*/
        ray = new Ray(rayPosition, transform.up * -1f);


        /*デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.yellow);
        /*レイキャスト当たり判定処理*/

        if (Physics.Raycast(ray,out rayHit, rayDistance)){
            if(rayHit.collider.tag == "Enemy")
            {
                rayHit.collider.gameObject.SetActive(false);/*エネミーを非アクティブ状態にする*/

                rb.velocity = new Vector3(0,0,0);
                rb.AddForce(new Vector3(0, upForce, 0));    /*敵を踏んだら上にジャンプ*/

            }
        }


    }
}
