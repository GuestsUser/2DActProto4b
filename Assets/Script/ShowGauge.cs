using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGauge : MonoBehaviour
{
    public DashSystemN dashsystem;
    // Start is called before the first frame update
    void Start()
    {
        dashsystem = GameObject.Find("Haruko").GetComponent<DashSystemN>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dashsystem._dash)
        {
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
