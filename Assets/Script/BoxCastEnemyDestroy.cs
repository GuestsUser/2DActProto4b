using UnityEngine;
using System.Collections;

public class BoxCastEnemyDestroy : MonoBehaviour
{
	RaycastHit hit;
	[SerializeField] public float rayDistance = 0.1f;	/*rayの長さ*/
	public Rigidbody rb;	
	public float upForce = 300f;	/*上に上がる力*/

	private Vector3 size = new Vector3(1, -1f, 1);  /*boxcastのサイズ*/

	/*敵倒した判定　龍用*/
	public bool isDead = false;


	void Start()
    {
		rb = GetComponent<Rigidbody>(); /*リジッドボデー*/
	}

	void FixedUpdate()
	{
		var scale = transform.lossyScale.x * 0.2f;	
		var isHit = Physics.BoxCast(transform.position, size * scale, transform.up * -1, out hit, transform.rotation, rayDistance);		
					/*boxCastの判定(箱の起点, 箱の大きさ * 調整用のscale, 判定をboxの下の面にする, hit , 箱の角度 , rayの長さ)*/
		if (isHit)
		{
			if (hit.collider.tag == "Enemy")
			{
				Debug.Log("あたった");

				isDead = true;
				hit.collider.gameObject.SetActive(false);/*エネミーを非アクティブ状態にする*/

				rb.velocity = new Vector3(0, 0, 0); /*一瞬プレイヤーの動きを止める*/
				rb.AddForce(new Vector3(0, upForce, 0));    /*敵を踏んだら上にジャンプ*/
			}
		}
	}
    //void OnDrawGizmos() /*Boxcastを疑似的に可視化する(Gizmosを利用)*/
    //{
    //    var scale = transform.lossyScale.x * 0.2f;
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(transform.position + transform.up * -1 * hit.distance, size * scale * 2);
    //}

}