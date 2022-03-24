using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 自機接触でダメージを与えるだけ */
public class RamAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player") { DamageSystem.Damage(0.5f, DamageSystem.DamageCombo.damage); }
    }
}
