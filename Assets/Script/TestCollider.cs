using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCollider : MonoBehaviour
{

    CapsuleCollider collider1;
    Animator animator;
    Transform jb;

    // Start is called before the first frame update
    void Start()
    {
        collider1 = GameObject.Find("Haruko").GetComponentInChildren<CapsuleCollider>();
        animator = GameObject.Find("Haruko").GetComponent<Animator>();
        jb = GameObject.Find("ASI").transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //　コライダの高さの調整
        collider1.height = animator.GetFloat("ColliderHeight");
        //　コライダの中心位置の調整
        //collider1.center = new Vector3(collider1.center.x, animator.GetFloat("ColliderCenter"), collider1.center.z);
        collider1.center = new Vector3(animator.GetFloat("ColliderCenterX") + collider1.center.x, animator.GetFloat("ColliderCenter"), collider1.center.z);
        //　コライダの半径の調整
        collider1.radius = animator.GetFloat("ColliderRadiuse");
        Debug.Log(animator.GetFloat("ColliderHeight"));
        jb.transform.localPosition = new Vector3(0, collider1.center.y + -collider1.height / 2, 0);

    }
}
