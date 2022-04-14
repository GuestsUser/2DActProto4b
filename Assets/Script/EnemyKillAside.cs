/*敵を踏んで倒すスクリプト*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyKillAside : MonoBehaviour
{
    public Vector3 rayPosition; /*レイキャストの位置*/

    /*レイキャスト用変数*/
    public Ray ray1;
    public Ray ray2;
    public Ray ray3;

    public Transform legtransform1;
    public Transform legtransform2;
    public Transform legtransform3;

    public Vector3 legposition1;
    public Vector3 legposition2;
    public Vector3 legposition3;

    public RaycastHit rayHit;
    [SerializeField] public float rayDistance = 0.1f;   /*raycastの長さ*/

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
        /*レイキャスト*/

        legposition1 = legtransform1.position;
        legposition2 = legtransform2.position;
        legposition3 = legtransform3.position;

        /*位置*/

        ray1 = new Ray(legposition1, -legtransform1.up);
        ray2 = new Ray(legposition2, -legtransform2.up);
        ray3 = new Ray(legposition3, -legtransform3.up);

        Debug.DrawRay(legposition1, ray1.direction * rayDistance,Color.yellow);
        Debug.DrawRay(legposition2, ray2.direction * rayDistance,Color.red);
        Debug.DrawRay(legposition3, ray3.direction * rayDistance,Color.blue) ;
       




        if (Physics.Raycast(ray1, out rayHit, rayDistance) || Physics.Raycast(ray2, out rayHit, rayDistance) || Physics.Raycast(ray3, out rayHit, rayDistance))
        {
            if (rayHit.collider.tag == "Enemy")
            {
                rayHit.collider.gameObject.SetActive(false);/*エネミーを非アクティブ状態にする*/

                rb.velocity = new Vector3(0, 0, 0); /*一瞬プレイヤーの動きを止める*/
                rb.AddForce(new Vector3(0, upForce, 0));    /*敵を踏んだら上にジャンプ*/

            }
        }


    }
}
