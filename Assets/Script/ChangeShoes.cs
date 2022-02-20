using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChangeShoes : Padinput
{
    //[SerializeField] private SEResource _se_resource;
    [SerializeField] private GameObject parent; //itemの親
    [SerializeField] private GameObject[] _item_obj;
    [SerializeField] private GameObject _selector_obj;
    //ChangeUI input;

    /*靴の並び配列(ゲーム中に並び替え出来るようにしたい)*/ 
    public ShoesType[] Shoes = { ShoesType.Jump_Shoes, ShoesType.Speed_Shoes, ShoesType.Magnet_Shoes };

    /*現在選択している靴のタイプ*/
    public ShoesType type {get { return Shoes[menu_number]; } }

    public GameObject selector_obj { get { return _selector_obj; } }
    public GameObject[] item_obj { get { return _item_obj; } }

    static int menu_number;
    float leftShoulder { get { return Gamepad.current.leftShoulder.ReadValue(); } }
    float rightShoulder { get { return Gamepad.current.rightShoulder.ReadValue(); } }

    public override void Change()
    {
        if (leftShoulder == 1)
        {
            if (++menu_number > 2) menu_number = 0;

        }
        if (rightShoulder == 1)
        {
            if (--menu_number < 0) menu_number = 2;
        }

        /*これをプレイヤーの方で管理*/
        //switch (type)
        //{
        //    case ShoesType.Jump_Shoes:

        //        break;

        //    case ShoesType.Speed_Shoes:

        //        break;

        //    case ShoesType.Magnet_Shoes:

        //        break;
        //}
        /**/

        //Debug.Log(Gamepad.current.leftShoulder.ReadValue());
        //Debug.Log("靴切り替え");
        //Debug.Log(menu_number);
    }

    /**/
    public virtual void Double_Jump() { 
        
    }
    public virtual void Speed_Dush()
    {
        
    }
    public virtual void Wall_Run()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        menu_number = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(type);
        var CursorY = _item_obj[menu_number].transform.position.y;
        _selector_obj.transform.parent = _item_obj[menu_number].transform; //親要素変更
        _selector_obj.transform.position = new Vector3(_item_obj[menu_number].transform.position.x, CursorY - 20, 0);
    }

}
