using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeFloor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(-10, 0, 0, ForceMode.Acceleration);
        }
    }
}
