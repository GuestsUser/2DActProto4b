using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParentCache : MonoBehaviour
{
    private static Transform _player;
    public static Transform player { get { return _player; } }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
