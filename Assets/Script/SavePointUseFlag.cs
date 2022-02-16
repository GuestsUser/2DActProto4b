using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* セーブポイントに付けるスクリプト、そのセーブポイントが使用済みか否かのフラグを持つだけだが将来的には何か演出を用意するかも */
public class SavePointUseFlag : MonoBehaviour
{
    [System.NonSerialized] public bool useBool = false ; /* このセーブポイントが使用済みか否か、trueで使用済み */
}
