using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JunpScript : MonoBehaviour
{

    [SerializeField]
    public float jumpForce = 5.0f;

    public Rigidbody rb;

    void Start()
    {

        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {

            Jump();

        }

    }

    /*ジャンプするだけ（質量無視の同じジャンプ力）*/
    void Jump()
    {

        rb.AddForce(0, jumpForce, 0, ForceMode.VelocityChange);
        rb.velocity = Vector3.zero;

    }

}
