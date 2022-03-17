using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibleSystem : MonoBehaviour
{
    [Header("被弾後の無敵時間機能")]
    [Tooltip("被弾後の無敵時間")] [SerializeField] float time;

    bool corutineRun;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public bool InvincibleRun()
    {
        bool ans = true;
        if (corutineRun)
        {

        }
        else { StartCoroutine(Invincible()); }
        return ans;
    }

    private IEnumerator Invincible()
    {
        float count = 0;

        yield return StartCoroutine(TimeScaleYield.TimeStop());
    }
}
