﻿using UnityEngine;
using System.Collections;

public class BoxCastEnemyDestroy : MonoBehaviour
{
	RaycastHit hit;
	[SerializeField] public float rayDistance = 0.1f;   /*rayの長さ*/

	private Rigidbody rb;

	public float upForce = 300f;    /*上に上がる力*/

	private Vector3 size = new Vector3(1, -1f, 1);  /*boxcastのサイズ*/

	/*敵倒した判定　龍用*/
	public bool isStepOnDead = false;

	/*敵を踏んだ時のSE用*/
	private AudioSource stepOnSource;
	[SerializeField] private AudioClip stepOnSe;


	void Start()
	{
		rb = GetComponent<Rigidbody>();
		stepOnSource = GetComponent<AudioSource>();
	}

	void FixedUpdate()
	{
		var scale = transform.lossyScale.x * 0.2f;
		var isHit = Physics.BoxCast(transform.position, size * scale, transform.up * -1, out hit, transform.rotation, rayDistance);
		/*boxCastの判定(箱の起点, 箱の大きさ * 調整用のscale, 判定をboxの下の面にする, hit , 箱の角度 , rayの長さ)*/
		if (isHit)
		{
			if (hit.collider.tag == "Enemy" && !isStepOnDead)
			{
				//Debug.Log("あたった");

				

				isStepOnDead = true;
				EnemyObjectCollision eCollision = hit.collider.GetComponent<EnemyObjectCollision>();
				if(eCollision != null)
                {
					eCollision.playerSteoOn = true;
					/*敵を踏んだら音を鳴らす*/
					stepOnSource.PlayOneShot(stepOnSe);
				}
				rb.velocity = new Vector3(0, 0, 0); /*一瞬プレイヤーの動きを止める*/
				rb.AddForce(new Vector3(0, upForce, 0));    /*敵を踏んだら上にジャンプ*/
			}
			else
			{
				isStepOnDead = false;
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