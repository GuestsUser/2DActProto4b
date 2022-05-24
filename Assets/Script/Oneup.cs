using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oneup : MonoBehaviour
{
    /*エフェクト発生用変数*/
    [SerializeField] ParticleSystem jumpEfect;
    ParticleSystem newParticle;
    [SerializeField] Transform ts;

    [SerializeField] public GManager gManager;

    void Update()
    {
        if (gManager.OneupHP)
        {
            newParticle = Instantiate(jumpEfect);
            newParticle.transform.position = ts.transform.position;
            newParticle.Play(); /*エフェクトを発生*/
            gManager.OneupHP = false;

        }
    }
}
