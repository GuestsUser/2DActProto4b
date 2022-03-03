using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    public Fade fade;

    void Start()
    {
        fade.FadeOut(1.5f);
    }
}
