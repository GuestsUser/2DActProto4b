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

    /*下にくっついてる状態で靴を切り替えると床を貫通する問題解決に必要*/
    bool kuttuki_down;

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
    }
    //private void Update()
    //{
    //    now_pos_y = this.transform.position.y;
    //    now_pos_x = this.transform.position.x;
    //    if (!Physics.Raycast(ray2, out rayHit, rayDistance) && bool_ray_hit == true)
    //    {
    //        if ((transform.localRotation.z != 1 && transform.localRotation.z != 0) && old_pos_y != now_pos_y) //前フレームのyの位置が変わっていたら
    //        {
    //            Debug.Log("上下の動きを読み取っています");
    //            if (old_pos_y > now_pos_y)
    //            {
    //                move_type = kuttuki_move_state.move_down;
    //            }
    //            else if (old_pos_y < now_pos_y)
    //            {
    //                move_type = kuttuki_move_state.move_up;
    //            }
    //        }
    //        /*左右*/
    //        else if (transform.localRotation.z == 1 || transform.localRotation.z == 0 && old_pos_x != now_pos_x) //前フレームのyの位置が変わっていたら
    //        {
    //            Debug.Log("左右の動きを読み取っています");
    //            if (old_pos_x > now_pos_x)
    //            {
    //                move_type = kuttuki_move_state.move_left;
    //            }
    //            else if (old_pos_x < now_pos_x)
    //            {
    //                Debug.Log("右に移動中");
    //                move_type = kuttuki_move_state.move_right;
    //            }
    //        }
    //        /*左右*/

    //        if (Physics.Raycast(ray, out rayHit, rayDistance) && rayHit.collider.tag == "kuttuku")
    //        {
    //            ; /*足元から出ているレイが当たっているときは何もしなくていい*/
    //        }
    //        else if (collider_exit == true) /*離れた場合は重力の方向を変える必要がある*/
    //        {

    //            Quaternion kuttuki_rot;

    //            switch (move_type)
    //            {
    //                case kuttuki_move_state.move_up: //上に走って、オブジェクトから離れた場合
    //                    kuttuki_rot = Quaternion.Euler(0, 0, 0);
    //                    if (player.right != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;
    //                        Quaternion rot;
    //                        rot = Quaternion.AngleAxis(90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;
    //                        this.transform.localRotation = q * rot;
    //                    }
    //                    else if (player.left != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;
    //                        Quaternion rot;
    //                        rot = Quaternion.AngleAxis(-90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;
    //                        this.transform.localRotation = q * rot;
    //                    }

    //                    Physics.gravity = new Vector3(0, -9.8f, 0);

    //                    //collider_exit = false;

    //                    //bool_ray_hit = false;
    //                    break;

    //                case kuttuki_move_state.move_down: //下に走って、オブジェクトから離れた場合
    //                                                   //Vector3 gravity_up = new Vector3(0, 9.8f, 0);
    //                                                   //Physics.gravity = Vector3.Lerp(now_gravity, gravity_up, Time.deltaTime);
    //                    kuttuki_rot = Quaternion.Euler(0, 0, 180);
    //                    if (player.right != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;

    //                        /* クォータニオン → オイラー角への変換*/
    //                        Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;

    //                        this.transform.localRotation = q * rot;
    //                    }
    //                    Physics.gravity = new Vector3(0, 9.8f, 0);

    //                    //collider_exit = false;
    //                    //bool_ray_hit = false;
    //                    break;

    //                case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
    //                                                    //Debug.Log("move_rightに変わってます");
    //                    kuttuki_rot = Quaternion.Euler(0, 0, 90);
    //                    if (player.right != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;

    //                        /* クォータニオン → オイラー角への変換*/
    //                        Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;

    //                        this.transform.localRotation = q * rot;
    //                    }
    //                    if (player.left != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;

    //                        /* クォータニオン → オイラー角への変換*/
    //                        Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;

    //                        this.transform.localRotation = q * rot;
    //                    }
    //                    Physics.gravity = new Vector3(-9.8f, 0, 0);

    //                    //collider_exit = false;

    //                    //bool_ray_hit = false;
    //                    break;

    //                case kuttuki_move_state.move_left: //左に走って、オブジェクトから離れた場合
    //                    kuttuki_rot = Quaternion.Euler(0, 0, 90);
    //                    if (player.right != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;

    //                        /* クォータニオン → オイラー角への変換*/
    //                        Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;

    //                        this.transform.localRotation = q * rot;
    //                        Physics.gravity = new Vector3(9.8f, 0, 0);
    //                    }
    //                    if (player.left != 0 && this.transform.rotation != kuttuki_rot)
    //                    {
    //                        Quaternion rotation = this.transform.localRotation;

    //                        /* クォータニオン → オイラー角への変換*/
    //                        Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
    //                        Quaternion q = this.transform.localRotation;

    //                        this.transform.localRotation = q * rot;
    //                        Physics.gravity = new Vector3(9.8f, 0, 0);
    //                    }



    //                    //collider_exit = false;
    //                    //bool_ray_hit = false;
    //                    break;
    //            }
    //        }
    //    }
    //    old_pos_y = now_pos_y;
    //    old_pos_x = now_pos_x;
    //}
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
        Debug.Log(kuttuki_time);
        Debug.Log(collider_exit);
        Debug.Log(Physics.gravity);

        rayPosition = rb.transform.position;    /*レイキャストの位置*/

        ray = new Ray(rayPosition, transform.up * -4f);
        ray2 = new Ray(rayPosition, transform.right * 1f);
        ray3 = new Ray(rayPosition, transform.up * 1.5f);


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

        /*レイキャストの当たり判定処理*/
        /*くっつく処理ここから↓*/
        if (change_shoes.type == ShoesType.Magnet_Shoes)
        {
            if (Physics.Raycast(ray, out rayHit, rayDistance))
            {
                bool_ray_hit = true;
            }
            /*下のif文から外したもの(Physics.Raycast(ray, out rayHit, rayDistance) || */
            if (Physics.Raycast(ray2, out rayHit, rayDistance))
            {
                if(bool_ray_hit == true && rayHit.collider.tag != "kuttuku")
                {
                    bool_ray_hit = false;
                    this.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    Physics.gravity = new Vector3(0, -9.8f, 0);
                    collider_exit = false;
                }
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

                    /*くっついた時の浮き状態を改善するためのもの*/
                    if(player.right != 0)
                    {
                        move_x = 0.5f;
                    }
                    else
                    {
                        move_x = -0.5f;
                    }
                    Vector3 kuttuki_pos = new Vector3(move_x, 0 , 0);
                    //if (player.left != 0)
                    //{
                    //    Vector3 kuttuki_pos = new Vector3(-0.5f, 0, 0);
                    //}
                    //else if (player.right != 0)
                    //{
                    //    Debug.Log("めっちゃ右に行くはず");
                    //    Vector3 kuttuki_pos = new Vector3(10f, 0, 0);
                    //}
                    

                    /*x軸がキャラクターの前後の場合*/
                    rotationAngles.z = 90.0f;

                    /*z軸がキャラクターの前後の場合*/
                    /* X軸の90度回転*/
                    //rotationAngles.x = -90.0f;

                    /* オイラー角 → クォータニオンへの変換*/
                    rotation = Quaternion.Euler(rotationAngles);

                    /* Transform値を設定する*/
                    
                    this.transform.localRotation = rotation;
                    
                    this.transform.localScale = scale;


                    /*下の処理の効果：右にも左にもくっつけるようになる*/
                    /*3月7日追加部分*/
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
                /*くっつき状態でもう一つのくっつきオブジェクトに接触した場合*/
                else if (Physics.Raycast(ray2, out rayHit, rayDistance) && rayHit.collider.tag == "kuttuku" && bool_ray_hit == true) 
                {
                    kuttuki_To_kuttuki = true;
                    Quaternion kuttuki_rot;
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
                    }
                }
            }
            /*今迷走してる部分*/
            /*3月7日追加部分*/
            else if (bool_ray_hit == true) /**/
            {
                

                if (Physics.Raycast(ray, out rayHit, rayDistance) && rayHit.collider.tag == "kuttuku")
                {
                    ; /*足元から出ているレイが当たっているときは何もしなくていい*/

                }
                else if (collider_exit == true) /*離れた場合は重力の方向を変える必要がある*/
                {
                    Quaternion kuttuki_rot;

                    /*離れてから時間経過が長い場合の処理*/
                    kuttuki_time++;
                    if(jump == true)
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

                    /*離れてから時間経過が長い場合の処理*/

                    if (player.idle == false && jump == false && kuttuki_time != 0 && kuttuki_To_kuttuki == false)
                    {
                        switch (move_type)
                        {
                            case kuttuki_move_state.move_up: //上に走って、オブジェクトから離れた場合
                                kuttuki_rot = Quaternion.Euler(0, 0, 0);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Debug.Log("回転するはずのところを通過しました");
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot  = Quaternion.AngleAxis(-90, Vector3.forward);
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
                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                Physics.gravity = new Vector3(0, 9.8f, 0);
                                kuttuki_down = true;
                                collider_exit = false;
                                //bool_ray_hit = false;
                                break;

                            case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
                                kuttuki_rot = Quaternion.Euler(0, 0, 90);
                                if (player.right != 0 && this.transform.rotation != kuttuki_rot)
                                {
                                    Quaternion rotation = this.transform.localRotation;

                                    /* クォータニオン → オイラー角への変換*/
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Vector3 position = this.transform.localPosition;
                                    kuttuki_pos = new Vector3(-0.3f,0 , 0);
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
                                    Debug.Log("ここ通ってる説ある");
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
            Transform myTransform;
            
            if (kuttuki_down == true)
            {
                Vector3 player_pos = this.transform.position;
                Vector3 plus_pos = new Vector3(0,-1,0);
                this.transform.position = player_pos + plus_pos;

                Quaternion rot = Quaternion.AngleAxis(180f, Vector3.right);
                Quaternion q = this.transform.localRotation;
                this.transform.localRotation = q * rot;

                
                kuttuki_down = false;
            }
            else
            {
                myTransform = this.transform;
                /* ワールド座標を基準に、回転を取得*/
                Vector3 worldAngle = myTransform.eulerAngles;
                worldAngle.x = 0.0f; /* ワールド座標を基準に、x軸を軸にした回転を10度に変更*/
                worldAngle.y = 0.0f; /* ワールド座標を基準に、y軸を軸にした回転を10度に変更*/
                worldAngle.z = 0.0f; /* ワールド座標を基準に、z軸を軸にした回転を10度に変更*/
                myTransform.eulerAngles = worldAngle; /* 回転角度を設定*/
            }
            

            /*追加部分*/
            bool_ray_hit = false;
            /*追加部分*/
            move_type = kuttuki_move_state.none;
        }

        
        // 前フレーム位置を更新
        _prevPosition = now_position;

    }/*Updateの終了部分*/

    private void OnCollisionEnter(Collision collision)
    {
        jump = false;
        if (collision.gameObject.tag == "kuttuku"/* && bool_ray_hit == true && collider_exit == true*/)
        {
            kuttuki_time = 0;
            collider_exit = false;
            kuttuki_To_kuttuki = false;
            Debug.Log("離れた後にくっついた判定");
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        
        if(collision.gameObject.tag == "kuttuku"/* && bool_ray_hit == true*/)
        {
            Debug.Log("離れました");
            collider_exit = true;
            //Debug.Log("オブジェクトから離れました");
            //switch (move_type)
            //{
            //    case kuttuki_move_state.move_up: //上に走って、オブジェクトから離れた場合
                    
            //        Physics.gravity = new Vector3(0, -9.8f, 0);

            //        bool_ray_hit = false;
            //        break;

            //    case kuttuki_move_state.move_down: //下に走って、オブジェクトから離れた場合
            //        Vector3 gravity_up = new Vector3(0, 9.8f, 0);
            //        Physics.gravity = Vector3.Lerp(now_gravity, gravity_up, Time.deltaTime);
            //        //Physics.gravity = new Vector3(0, 9.8f, 0);
                    

            //        bool_ray_hit = false;
            //        break;

            //    case kuttuki_move_state.move_right: //右に走って、オブジェクトから離れた場合
            //        Physics.gravity = new Vector3(-9.8f, 0, 0);

            //        bool_ray_hit = false;
            //        break;

            //    case kuttuki_move_state.move_left: //左に走って、オブジェクトから離れた場合
            //        Physics.gravity = new Vector3(9.8f, 0, 0);

            //        bool_ray_hit = false;
            //        break;
            //}

            //Physics.gravity = new Vector3(0, -9.8f, 0);
            //Transform myTransform = this.transform;

            ///* ワールド座標を基準に、回転を取得*/
            //Vector3 worldAngle = myTransform.eulerAngles;
            //worldAngle.x = 0.0f; /* ワールド座標を基準に、x軸を軸にした回転を10度に変更*/
            //worldAngle.y = 0.0f; /* ワールド座標を基準に、y軸を軸にした回転を10度に変更*/
            //worldAngle.z = 0.0f; /* ワールド座標を基準に、z軸を軸にした回転を10度に変更*/
            //myTransform.eulerAngles = worldAngle; /* 回転角度を設定*/

            /*追加部分*/
            //bool_ray_hit = false;
            /*追加部分*/
        }
    }
    //bool x_pos_different(float now_pos, float old_pos)
    //{
    //    if(now_pos != old_pos)
    //    {
    //        //値が違う場合trueを返す
    //        return true;
    //    }
    //    else
    //    {
    //        //値が一緒の場合falseを返す
    //        return false;
    //    }

    //}
    public override void Jump()
    {
        jump = true;
    }
}


