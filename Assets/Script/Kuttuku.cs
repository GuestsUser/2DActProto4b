using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kuttuku : Padinput
{
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

        rb = GetComponent<Rigidbody>(); /*リジッドボデー*/

        bool_ray_hit = false;
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

        /*脳筋式レイキャストの当たり判定処理*/


        if (change_shoes.type == ShoesType.Magnet_Shoes)
        {

            if ((Physics.Raycast(ray, out rayHit, rayDistance) || Physics.Raycast(ray2, out rayHit, rayDistance)) && rayHit.collider.tag == "kuttuku")
            {
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

                    /* X軸の90度回転*/
                    rotationAngles.z = -90.0f;

                    /* オイラー角 → クォータニオンへの変換*/
                    rotation = Quaternion.Euler(rotationAngles);

                    /* Transform値を設定する*/
                    this.transform.localPosition = position;
                    this.transform.localRotation = rotation;
                    this.transform.localScale = scale;

                    Debug.Log("くっつく");
                    Physics.gravity = new Vector3(-9.8f, 0, 0);
                }
            }

        }
        else if (bool_ray_hit == true) /*←追加部分*/
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
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "kuttuku" && bool_ray_hit == true)
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
    }
}


