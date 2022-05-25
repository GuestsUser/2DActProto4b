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

    /*残機MAX*/
    [SerializeField] ParticleSystem maxEffect;
    ParticleSystem newMaxParticle;

    private void Start()
    {
        gManager = GameObject.Find("GameManager").GetComponent<GManager>();
    }
    void Update()
    {
        if (gManager.OneupHP)
        {
            newParticle = Instantiate(jumpEfect);
            newParticle.transform.position = ts.transform.position;
            newParticle.Play(); /*エフェクトを発生*/
            gManager.OneupHP = false;

        }

        if (gManager.ZankiMax)
        {
            newParticle = Instantiate(maxEffect);
            newParticle.transform.position = ts.transform.position;
            newParticle.Play();
            gManager.ZankiMax = false;
        }
    }
    
}
