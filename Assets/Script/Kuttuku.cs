using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum kuttuki_move_state
{
    move_up,
    move_down,
    move_right,
    move_left,
    none
}
public class Kuttuku : Padinput
{
    /*グラビティの方向を変更するのに必要*/
    kuttuki_move_state move_type;
    PlayerMove player;
    Vector3 now_gravity;
    public bool collider_exit;
    /*1フレーム前の位置*/
    private Vector3 _prevPosition;
    int kuttuki_time; /*くっつきオブジェクトから離れたときの時間*/
    /*くっつき時の浮く問題解決に必要*/
    float move_x = 0.5f;
    bool kuttuki_To_kuttuki;

    /*くっついてる状態で靴を切り替えるとオブジェクトを貫通する問題解決に必要*/
    bool kuttuki_down;
    //bool kuttuki_

    /*ジャンプ移動の不具合修正に必要*/
    bool jump;

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
    bool kuttuki_left; /*true:左入力をしている状態でくっつきオブジェクトにくっつく*/
    /*追加部分*/

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.5591f;


    /*レイキャスト用変数*/

    public Rigidbody rb;    /*みんな大好きリジッドボデー*/

    /*くっついた時の浮き状態を改善するためのもの*/
    Vector3 kuttuki_pos;

    /* 滑る問題解決 */
    Animator animator;

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();

        player = GetComponent<PlayerMove>();

        rb = GetComponent<Rigidbody>(); /*リジッドボデー*/

        bool_ray_hit = false;

        collider_exit = false;
        move_type = kuttuki_move_state.none;
        // 初期位置を保持
        _prevPosition = transform.position;

        jump = false;
        kuttuki_left = false;
        move_x = 0.5f;
        kuttuki_To_kuttuki = false;
        kuttuki_down = false;

        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        //if (change_shoes.type == ShoesType.Magnet_Shoes) /*靴のタイプがマグネットの時*/
        //{
        //    animator.applyRootMotion = true;
        //}
        //else
        //{
        //    animator.applyRootMotion = false;
        //}
     }
    void FixedUpdate()
    {
        /*3月9日追加部分*/
        // deltaTimeが0の場合は何もしない
        if (Mathf.Approximately(Time.deltaTime, 0))
            return;
        // 現在位置取得
        var now_position = transform.position;
        // 現在速度計算
        var velocity = (now_position - _prevPosition) / Time.deltaTime;

        /*判定がおかしい部分*/
        /*上下*/
        if(Physics.gravity == new Vector3(9.8f,0,0) || Physics.gravity == new Vector3(-9.8f, 0, 0))
        {
            Debug.Log("上下の移動判定がとられるはず");
            if (velocity.y > 0.0f) /*上に進んでいる*/
            {
                move_type = kuttuki_move_state.move_up;
            }
            else if (velocity.y < 0.0f)/*下に進んでいる*/
            {
                move_type = kuttuki_move_state.move_down;
            }
            /*上下*/
        }

        /*左右*/
        if (Physics.gravity == new Vector3(0, 9.8f, 0) || Physics.gravity == new Vector3(0,- 9.8f, 0))
        {
            if (velocity.x > 0.0f) /*上に進んでいる*/
            {
                move_type = kuttuki_move_state.move_right;
            }
            else if (velocity.x < 0.0f)/*下に進んでいる*/
            {
                move_type = kuttuki_move_state.move_left;
            }
        }
        /*左右*/
        /*判定がおかしい部分*/

        if(velocity == Vector3.zero)
        {
            move_type = kuttuki_move_state.none;
        }

        // 現在速度をログ出力
        //print($"velocity = {velocity}");


        /*3月9日追加部分*/

        Debug.Log(move_type);
        //Debug.Log(kuttuki_time);
        //Debug.Log(collider_exit);
        //Debug.Log(Physics.gravity);

        rayPosition = rb.transform.position;    /*レイキャストの位置*/

        ray = new Ray(rayPosition, transform.up * -1f);     /*足元*/
        ray2 = new Ray(rayPosition, transform.right * 1f);  /*正面*/
        ray3 = new Ray(rayPosition, transform.up * 1.5f);   /*頭*/


        /*デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, ray.direction * rayDistance, Color.red);
        Debug.DrawRay(rayPosition, ray2.direction * rayDistance, Color.blue);
        Debug.DrawRay(rayPosition, ray3.direction * rayDistance, Color.yellow);

        //if(collider_exit == true)
        //{
        //    kuttuki_time++;
        //    if(kuttuki_time > 30)
        //    {
        //        collider_exit = true;
        //        bool_ray_hit = false;
        //        kuttuki_time = 0;
        //    }
        //}
        //else
        //{

        //}

       
        Kuttuki();


        // 前フレーム位置を更新
        _prevPosition = now_position;

    }/*Updateの終了部分*/
    private void Kuttuki() /*くっつき処理*/
    {
        if (change_shoes.type == ShoesType.Magnet_Shoes) /*靴のタイプがマグネットの時*/
        {
            if (Physics.Raycast(ray, out rayHit, rayDistance))/*足元から出ているレイが当たっていたら*/
            {
                if(rayHit.collider.tag == "kuttuku")
                {
                    bool_ray_hit = true; /*くっつき時のフラグ(bool_ray_hit)をtrueにする*/
                }
                /*下の処理はくっつき状態でそのまま移動していて足元がくっつきオブジェクトからノーマルオブジェクトに変わったときの処理*/
                else if (bool_ray_hit == true && rayHit.collider.tag != "kuttuku") /*この処理は上りでは使えない、回転が変わり続ける→戻る→落ちる→くっつくのループ*/
                {
                    if (kuttuki_down == true) /*オブジェクトの下にくっついている状態の時*/
                    {
                        Physics.gravity = new Vector3(0, -9.8f, 0);
                        bool_ray_hit = false;
                        Vector3 player_pos = this.transform.position;
                        Vector3 plus_pos = new Vector3(0, -1, 0);
                        this.transform.position = player_pos + plus_pos;
                        collider_exit = false;

                        kuttuki_down = false;
                    }
                    else
                    {
                        bool_ray_hit = false;
                        this.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        Physics.gravity = new Vector3(0, -9.8f, 0);
                        collider_exit = false;
                    }
                   
                }
            }
            //else if (!Physics.Raycast(ray, out rayHit, rayDistance) && bool_ray_hit == true)
            //{
            //    collider_exit = true;
            //}

            if (Physics.Raycast(ray2, out rayHit, rayDistance)) /*プレイヤーの正面から出ているレイが当たっている時*/
            {

                //Debug.Log("通りました");
                /*コライダーを持つオブジェクトから、タグを読み取る（壁をkuttukuに設定）*/
                if (rayHit.collider.tag == "kuttuku" && bool_ray_hit == false)
                {
                    bool_ray_hit = true;

                    /* Transform値を取得する*/
                    Vector3 position = this.transform.localPosition;
                    Quaternion rotation = this.transform.localRotation;

                    /* クォータニオン → オイラー角への変換*/
                    Vector3 rotationAngles = rotation.eulerAngles;

                    /*くっついた時の浮き状態を改善するためのもの*/
                    if (player.right != 0)
                    {
                        move_x = 0.5f;
                    }
                    else
                    {
                        move_x = -0.5f;
                    }
                    Vector3 kuttuki_pos = new Vector3(move_x, 0, 0);

                    /*x軸がキャラクターの前後の場合*/
                    rotationAngles.z = 90.0f;

                    /*z軸がキャラクターの前後の場合*/
                    /* X軸の90度回転*/
                    //rotationAngles.x = -90.0f;

                    /* オイラー角 → クォータニオンへの変換*/
                    rotation = Quaternion.Euler(rotationAngles);

                    /* Transform値を設定する*/

                    this.transform.localRotation = rotation;

                    /*下の処理の効果：右にも左にもくっつけるようになる*/
                    if (player.right != 0)
                    {
                        Physics.gravity = new Vector3(9.8f, 0, 0);
                    }
                    else if (player.left != 0)
                    {
                        Physics.gravity = new Vector3(-9.8f, 0, 0);
                    }
                    this.transform.localPosition = position + kuttuki_pos;
                }
                /*地面にぶつかったとき*/
                else if (rayHit.collider.tag != "kuttuku" && bool_ray_hit == true && move_type == kuttuki_move_state.move_down)
                {
                    bool_ray_hit = false;
                    collider_exit = false;
                    Physics.gravity = new Vector3(0, - 9.8f, 0);
                }
                /*くっつき状態でもう一つのくっつきオブジェクトに接触した場合*/
                else if (Physics.Raycast(ray2, out rayHit, rayDistance) && rayHit.collider.tag == "kuttuku" && bool_ray_hit == true)
                {
                    kuttuki_To_kuttuki = true;
                    Quaternion kuttuki_rot;
                    /*オブジェクトの内側の回転処理*/
                    switch (move_type)
                    {
                        case kuttuki_move_state.move_up: //上に走って、オブジェクトに接触した場合
                            kuttuki_rot = Quaternion.Euler(0, 0, 180);
                            if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                            {

                                Quaternion rotation = this.transform.localRotation;
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;



                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0, 0.3f, 0);
                                this.transform.localPosition = position + kuttuki_pos;

                            }
                            else if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;
                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0, 0.3f, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }

                            Physics.gravity = new Vector3(0, 9.8f, 0);
                            kuttuki_down = true;
                            //collider_exit = false;

                            //bool_ray_hit = false;
                            break;

                        case kuttuki_move_state.move_down: //下に走って、オブジェクトから離れた場合
                                                           //Vector3 gravity_up = new Vector3(0, 9.8f, 0);
                                                           //Physics.gravity = Vector3.Lerp(now_gravity, gravity_up, Time.deltaTime);
                            kuttuki_down = false;
                            kuttuki_rot = Quaternion.Euler(0, 0, 0);
                            if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;

                                /* クォータニオン → オイラー角への変換*/
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;

                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0, -0.3f, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            else if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                            {

                                Quaternion rotation = this.transform.localRotation;
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;
                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0, -0.3f, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                                Debug.Log("こことおってるかも");
                            }
                            Physics.gravity = new Vector3(0, -9.8f, 0);

                            //collider_exit = false;
                            //bool_ray_hit = false;
                            break;

                        case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
                            kuttuki_down = false;
                            kuttuki_rot = Quaternion.Euler(0, 0, 90);
                            if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;

                                /* クォータニオン → オイラー角への変換*/
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;

                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0.3f, 0, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;

                                /* クォータニオン → オイラー角への変換*/
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;

                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(0.3f, 0, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            Physics.gravity = new Vector3(9.8f, 0, 0);
                            //collider_exit = false;

                            //bool_ray_hit = false;
                            break;

                        case kuttuki_move_state.move_left: //左に走って、オブジェクトから離れた場合
                            kuttuki_down = false;
                            kuttuki_rot = Quaternion.Euler(0, 0, 90);
                            if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;

                                /* クォータニオン → オイラー角への変換*/
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;

                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(-0.3f, 0, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                            {
                                Quaternion rotation = this.transform.localRotation;

                                /* クォータニオン → オイラー角への変換*/
                                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                                Quaternion q = this.transform.localRotation;

                                this.transform.localRotation = q * rot;

                                Vector3 position = this.transform.localPosition;
                                kuttuki_pos = new Vector3(-0.3f, 0, 0);
                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            Physics.gravity = new Vector3(-9.8f, 0, 0);

                            //collider_exit = false;
                            //bool_ray_hit = false;
                            break;
                        //case kuttuki_move_state.none: //元々の地面がくっつきオブジェクトの場合で壁に向かって走っている時に靴を切り替えた時
                        //    if(player.right != 0)
                        //    {
                        //        move_type = kuttuki_move_state.move_right;
                        //    }
                        //    else if (player.left != 0)
                        //    {
                        //        move_type = kuttuki_move_state.move_left;
                        //    }
                        //    break;
                    }
                }
            }
            /*今迷走してる部分*/
            /*3月7日追加部分*/
            else if (bool_ray_hit == true) /**/
            {


                if (!Physics.Raycast(ray, out rayHit, rayDistance) && collider_exit == true) /*離れた場合は重力の方向を変える必要がある*/
                {
                    Quaternion kuttuki_rot;

                    /*離れてから時間経過が長い場合の処理*/
                    kuttuki_time++;
                    if (jump == true)
                    {
                        if (kuttuki_time > 47) //47はジャンプの滞空時間
                        {
                            collider_exit = false;
                            bool_ray_hit = false;
                            kuttuki_time = 0;
                            kuttuki_rot = Quaternion.Euler(0, 0, 0);
                            this.transform.localRotation = kuttuki_rot;
                            Physics.gravity = new Vector3(0, -9.8f, 0);
                        }
                    }
                    else
                    {
                        if (kuttuki_time > 5) //回転時に離れた時
                        {
                            collider_exit = false;
                            bool_ray_hit = false;
                            kuttuki_time = 0;
                            kuttuki_rot = Quaternion.Euler(0, 0, 0);
                            this.transform.localRotation = kuttuki_rot;
                            Physics.gravity = new Vector3(0, -9.8f, 0);
                        }
                    }


                    /*オブジェクトの外側の回転処理*/
                    if (player.idle == false && jump == false && kuttuki_time != 0 && kuttuki_To_kuttuki == false)
                    {
                        switch (move_type)
                        {
                            case kuttuki_move_state.move_up: //上に走って、オブジェクトから離れた場合
                                kuttuki_down = false;
                                kuttuki_rot = Quaternion.Euler(0, 0, 0);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Debug.Log("回転するはずのところを通過しました");
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;



                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0, -0.3f, 0);
                                    this.transform.localPosition = position + kuttuki_pos;

                                }
                                else if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;
                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0, -0.1f, 0); /*ココだけなぜか、-0.2,-0.3だと不具合発生*/
                                    this.transform.localPosition = position + kuttuki_pos;
                                }

                                Physics.gravity = new Vector3(0, -9.8f, 0);

                                collider_exit = false;

                                //bool_ray_hit = false;
                                break;

                            case kuttuki_move_state.move_down: //下に走って、オブジェクトから離れた場合
                                                               //Vector3 gravity_up = new Vector3(0, 9.8f, 0);
                                                               //Physics.gravity = Vector3.Lerp(now_gravity, gravity_up, Time.deltaTime);
                                kuttuki_rot = Quaternion.Euler(0, 0, 180);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0, 0.1f, 0); /*変更箇所*/
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                else if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;
                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0, 0.3f, 0);
                                    //Debug.Log("変な浮きの原因はココだ");
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                Physics.gravity = new Vector3(0, 9.8f, 0);
                                kuttuki_down = true;
                                collider_exit = false;
                                //bool_ray_hit = false;
                                break;

                            case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
                                kuttuki_down = false;
                                kuttuki_rot = Quaternion.Euler(0, 0, 90);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(-0.3f, 0, 0);
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(-0.3f, 0, 0);
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                Physics.gravity = new Vector3(-9.8f, 0, 0);
                                collider_exit = false;

                                //bool_ray_hit = false;
                                break;

                            case kuttuki_move_state.move_left: //左に走って、オブジェクトから離れた場合
                                kuttuki_down = false;
                                kuttuki_rot = Quaternion.Euler(0, 0, 90);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0.3f, 0, 0);
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                if (player.left != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(0.3f, 0, 0);
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                Physics.gravity = new Vector3(9.8f, 0, 0);

                                collider_exit = false;
                                //bool_ray_hit = false;
                                break;
                        }
                    }

                }
                /*上下*/
                /*今迷走してる部分*/


                //}

            }
            /*3月7日追加部分*/

        }
       
        else /*靴がマグネットシューズ意外になったときに発動*/
        {
            Physics.gravity = new Vector3(0, -9.8f, 0);
            //Transform myTransform;

            if (kuttuki_down == true) /*オブジェクトの下にくっついている状態の時*/
            {
                Vector3 player_pos = this.transform.position;
                Vector3 plus_pos = new Vector3(0, -1, 0);
                this.transform.position = player_pos + plus_pos;

                Quaternion rot = Quaternion.AngleAxis(180f, Vector3.right);
                Quaternion q = this.transform.localRotation;
                this.transform.localRotation = q * rot;


                kuttuki_down = false;
            }

            bool_ray_hit = false;
            move_type = kuttuki_move_state.none;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        jump = false;
        if (collision.gameObject.tag == "kuttuku" && bool_ray_hit == true && collider_exit == true)
        {
            kuttuki_time = 0;
            collider_exit = false;
            kuttuki_To_kuttuki = false;
            Debug.Log("離れた後にくっついた判定");
        }
    }

    private void OnCollisionExit(Collision collision) /*これから離れた判定を変えないといけない*/
    {

        if (collision.gameObject.tag == "kuttuku" && bool_ray_hit == true)
        {
            Debug.Log("離れました");
            collider_exit = true;
        }
    }

    public override void Jump()
    {
        jump = true;
    }
}


