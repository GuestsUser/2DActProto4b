using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashSystemN : Padinput
{
    [SerializeField] float coolTimeMove = 3f; /* 動作可能になるまでの時間 */
    [SerializeField] float coolTime = 5f; /*クールタイム(初期値は5秒)*/
    [SerializeField] float dashForce = 5f; /*ダッシュの強さ(初期値は５)*/
    [SerializeField] public float rayDistance = 0.5f; /*（レイキャスト）可視光線の長さ*/
    private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    private Animator animator;
    private bool rePlayFlg = true; /* trueでダッシュ使用可能 */

    private Vector3 adjust; /* 基準位置を中心に持ってくる為の変数 */

    private Rigidbody rb;
    private PlayerMove control;

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        control = GetComponent<PlayerMove>();

        adjust = new Vector3(0, transform.localScale.y / 2, 0);
    }

    public override void Skill()
    {
        /*アビリティ発動ボタンが押されたら*/
        if (rePlayFlg && Gamepad.current.buttonWest.wasPressedThisFrame && change_shoes.type== ShoesType.Speed_Shoes)
        {
            StartCoroutine(Dush());
            StartCoroutine(ReCharge());
        }
    }

    IEnumerator Dush()
    {
        rb.AddRelativeForce(dashForce + rb.velocity.x, rb.velocity.y, 0, ForceMode.VelocityChange);
        animator.SetTrigger("Attack");

        control.movePermit = false;
        control.rotatePermit = false;

        float count = 0f;
        RaycastHit rayHit;
        while (true)
        {
            if (Physics.BoxCast(transform.position + adjust, transform.localScale / 2, transform.right, out rayHit, new Quaternion(), rayDistance))
            {
                if (rayHit.collider.tag == "Enemy") { rayHit.collider.gameObject.SetActive(false); } /* レイキャストに触れたenemyタグを持つオブジェクトは消えることになる */
            }
            //rb.velocity = Vector3.zero;

            if (count> coolTimeMove) { break; }
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }

        control.movePermit = true;
        control.rotatePermit = true;
    }

    IEnumerator ReCharge()
    {
        rePlayFlg = false;
        float count = 0f;
        while (count < coolTime)
        {
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
        }
        rePlayFlg = true;
    }

    void OnDrawGizmos()
    {
        //　Cubeのレイを疑似的に視覚化
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + adjust + transform.right * rayDistance, transform.localScale/2);
    }
}
