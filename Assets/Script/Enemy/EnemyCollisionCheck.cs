using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionCheck : MonoBehaviour
{

    public bool isOn = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "Enemy3")
        {
            isOn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ground" || other.gameObject.tag == "Enemy" || other.gameObject.tag == "Enemy3")
        {
            isOn = false;
        }
    }
}