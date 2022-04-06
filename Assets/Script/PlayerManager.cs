using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private RetrySystem retry;

    private void Start()
    {
        retry = GetComponent<RetrySystem>();
    }

    private void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DeadArea")
        {
            retry.Retry();
            GManager.instance.SubZankiNum();
        }
    }
}