using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeObjNum : MonoBehaviour
{
    int sibling_index;
    // Start is called before the first frame update
    void Start()
    {
        sibling_index = transform.GetSiblingIndex();
    }

    // Update is called once per frame
    void Update()
    {
        transform.SetAsLastSibling();
    }
}
