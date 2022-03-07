using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum kuttuki_move_state
{
    move_up,
    move_down,
    move_right,
    move_left
}
public class Kuttuku : Padinput
{
    /*グラビティの方向を変更するのに必要*/
    kuttuki_move_state move_type;
    PlayerMove player;
    Vector3 now_gravity;
    float old_pos_y;
    float now_pos_y {get { return transform.position.y; } }
    float old_pos_z;
    float now_pos_z { get { return transform.position.z; } }


    /*レイキャスト用変数*/
    public Vector3 rayPosition; /*レイキャストの位置*/

    [SerializeField] private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    /*脳筋式レイキャスト変数*/
    public Ray ray;     /*足元*/
    public Ray ray2;    /*正面*/
    public Ray ray3;    /*頭*/

    public RaycastHit rayHit;

    /*追加部分*/
    public bool bool_ray_hit;
    /*追加部分*/

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.5591f;


    /*レイキャスト用変数*/

    public Rigidbody rb;    /*みんな大好きリジッドボデー*/

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();

        player = GetComponent<PlayerMove>();

        rb = GetComponent<Rigidbody>(); /*リジッドボデー*/

        bool_ray_hit = false;

        old_pos_y = now_pos_y;

        now_gravity = Physics.gravity;
    }

    void FixedUpdate()
    {
        rayPosition = rb.transform.position;    /*レイキャストの位置*/

        ray = new Ray(rayPosition, transform.up * -4f);
        ray2 = new Ray(rayPosition, transform.right * 1f);
        ray3 = new Ray(rayPosition, transform.up * 1.5f);


        /*デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray2.direction * rayDistance, Color.blue);
        Debug.DrawRay(rayPosition, ray3.direction * rayDistance, Color.yellow);

        /*レイキャストの当たり判定処理*/

        if (change_shoes.type == ShoesType.Magnet_Shoes)
        {
            /*下のif文から外したもの(Physics.Raycast(ray, out rayHit, rayDistance) || */
            if (Physics.Raycast(ray2, out rayHit, rayDistance) && rayHit.collider.tag == "kuttuku")
            {
                Debug.Log("通りました");
                /*コライダーを持つオブジェクトから、タグを読み取る（壁をkuttukuに設定）*/
                if (rayHit.collider.tag == "kuttuku" && bool_ray_hit == false)
                {
                    /*追加部分*/
                    bool_ray_hit = true;
                    /*追加部分*/

                    /* Transform値を取得する*/
                    Vector3 position = this.transform.localPosition;
                    Quaternion rotation = this.transform.localRotation;
                    Vector3 scale = this.transform.localScale;

                    /* クォータニオン → オイラー角への変換*/
                    Vector3 rotationAngles = rotation.eulerAngles;

                    /*x軸がキャラクターの前後の場合*/
                    rotationAngles.z = 90.0f;

                    /*z軸がキャラクターの前後の場合*/
                    /* X軸の90度回転*/
                    //rotationAngles.x = -90.0f;

                    /* オイラー角 → クォータニオンへの変換*/
                    rotation = Quaternion.Euler(rotationAngles);

                    /* Transform値を設定する*/
                    this.transform.localPosition = position;
                    this.transform.localRotation = rotation;
                    this.transform.localScale = scale;

                    Debug.Log("くっつく");

                    /*下の処理の効果：右にも左にもくっつけるようになる*/
                    /*3月7日追加部分*/
                    if (player.right != 0)
                    {
                        Physics.gravity = new Vector3(9.8f, 0, 0);
                    }
                    else if(player.left != 0)
                    {
                        Physics.gravity = new Vector3(-9.8f, 0, 0);
                    }
                    
                }
            }

        }
        else if (bool_ray_hit == true) /*靴がマグネットシューズ意外になったときに発動*/
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
            Transform myTransform = this.transform;

            /* ワールド座標を基準に、回転を取得*/
            Vector3 worldAngle = myTransform.eulerAngles;
            worldAngle.x = 0.0f; /* ワールド座標を基準に、x軸を軸にした回転を10度に変更*/
            worldAngle.y = 0.0f; /* ワールド座標を基準に、y軸を軸にした回転を10度に変更*/
            worldAngle.z = 0.0f; /* ワールド座標を基準に、z軸を軸にした回転を10度に変更*/
            myTransform.eulerAngles = worldAngle; /* 回転角度を設定*/

            /*追加部分*/
            bool_ray_hit = false;
            /*追加部分*/
        }

        /*3月7日追加部分*/
        //if (bool_ray_hit == true) /**/
        //{
        //    /*上下*/
        //    if (old_pos_y != now_pos_y) //前フレームのyの位置が変わっていたら
        //    {
        //        if (old_pos_y > now_pos_y)
        //        {
        //            move_type = kuttuki_move_state.move_down;
        //        }
        //        else if (old_pos_y < now_pos_y)
        //        {
        //            move_type = kuttuki_move_state.move_up;
        //        }
        //    }
            
        //    /*上下*/

        //    /*左右*/
        //    if (old_pos_z > now_pos_z)
        //    {
        //        move_type = kuttuki_move_state.move_left;
        //    }
        //    else if (old_pos_z < now_pos_z)
        //    {
        //        move_type = kuttuki_move_state.move_right;
        //    }
        //    /*左右*/
            
        //}

        //old_pos_y = now_pos_y;
        //old_pos_z = now_pos_z;
        /*3月7日追加部分*/

    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "kuttuku" && bool_ray_hit == true)
        {
            //switch (move_type)
            //{
            //    case kuttuki_move_state.move_up: //上に走って、オブジェクトから離れた場合
            //        Vector3 gravity_up = new Vector3(0, -9.8f, 0);
            //        //Vector3.Lerp();
            //        Physics.gravity = new Vector3(0, -9.8f, 0);
            //        break;

            //    case kuttuki_move_state.move_down: //下に走って、オブジェクトから離れた場合
            //        Physics.gravity = new Vector3(0, 9.8f, 0);
            //        break;

            //    case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
            //        Physics.gravity = new Vector3(-9.8f, 0, 0);
            //        break;

            //    case kuttuki_move_state.move_left: //左に走って、オブジェクトから離れた場合
            //        Physics.gravity = new Vector3(9.8f, 0, 0);
            //        break;
            //}

            Physics.gravity = new Vector3(0, -9.8f, 0);
            Transform myTransform = this.transform;

            /* ワールド座標を基準に、回転を取得*/
            Vector3 worldAngle = myTransform.eulerAngles;
            worldAngle.x = 0.0f; /* ワールド座標を基準に、x軸を軸にした回転を10度に変更*/
            worldAngle.y = 0.0f; /* ワールド座標を基準に、y軸を軸にした回転を10度に変更*/
            worldAngle.z = 0.0f; /* ワールド座標を基準に、z軸を軸にした回転を10度に変更*/
            myTransform.eulerAngles = worldAngle; /* 回転角度を設定*/

            /*追加部分*/
            bool_ray_hit = false;
            /*追加部分*/
        }
    }
}


