using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeFloor : MonoBehaviour
{
    [SerializeField] float force=0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            
            Vector3 angle = transform.rotation.eulerAngles;
            other.gameObject.GetComponent<Rigidbody>().AddForce(force * Mathf.Cos(angle.z * Mathf.Deg2Rad), force * Mathf.Sin(angle.z * Mathf.Deg2Rad), 0, ForceMode.VelocityChange);
        }
    }
}
