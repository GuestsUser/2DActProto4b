using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_JumpSystem : MonoBehaviour
{

    [SerializeField] public float jumpForce = 5.0f;
    [SerializeField] public int dubleJump = 0;
    [SerializeField] public bool jump = false;

    /*レイキャスト用変数*/
    public Vector3 rayPosition;

    public Ray ray;
    public RaycastHit rayHit;

    [SerializeField] public float rayDistance = 5f;

    public Rigidbody rb;
    /*レイキャスト用変数*/

    void Start()
    {

        rb = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {

        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */
        rayPosition = rb.transform.position;
        ray = new Ray(rayPosition, transform.up * -1);
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.red);
        print(jump);
        if (Physics.Raycast(ray, out rayHit, rayDistance))
        {

            if (rayHit.collider.tag == "ground")
            {

                dubleJump = 2;
                jump = true;

            }

        }
        else if (jump)
        {
            dubleJump = 1;
            jump = false;

        }
        /*レイキャストの使い方がわからないのでお願いします（プレイやの当たり判定をレイキャストにする）参照元： https://getabakoclub.com/2020/05/11/unity%e3%81%a7%e5%9c%b0%e9%9d%a2%e3%81%ae%e5%bd%93%e3%81%9f%e3%82%8a%e5%88%a4%e5%ae%9a%e3%82%92%e8%b6%b3%e5%85%83%e3%81%a0%e3%81%91%e5%8f%96%e5%be%97%e3%81%99%e3%82%8b%e3%80%903d%e3%80%91/ */

        /*Colliderを使った当たり判定処理（二弾ジャンプ）*/
        //print(jump);
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (dubleJump == 2 && jump)
            {

                dubleJump--;
                Jump();

            }
            else if (dubleJump >= 1 && !jump)
            {

                dubleJump--;
                Jump();

            }
            //else
            //{

            //    dubleJump = 2;

            //}


        }
        /*Colliderを使った当たり判定処理（二弾ジャンプ）*/

    }

    /*Colliderを使った当たり判定処理（二弾ジャンプ）*/
    //    private void OnCollisionEnter(Collision collision)
    //{

    //    if (collision.gameObject.tag == "ground")
    //    {

    //        jump = true;
    //        dubleJump = 2;

    //    }
    //    else
    //    {

    //        jump = false;
    //        dubleJump = 1;

    //    }

    //}

    //private void OnCollisionExit(Collision collision)
    //{

    //    if (collision.gameObject.tag == "ground")
    //    {

    //        jump = false;
    //        dubleJump = 1;

    //    }
    //}
    /*Colliderを使った当たり判定処理（二弾ジャンプ）*/

    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    void Jump()
    {

        rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        rb.velocity = Vector3.zero;

    }

}
