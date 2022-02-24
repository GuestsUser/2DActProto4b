using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LocalSave
{
    public string version ;
    public bool dataBool = false; /* セーブを読めなかった場合こちらをfalseとする事で空セーブとして扱う */
}