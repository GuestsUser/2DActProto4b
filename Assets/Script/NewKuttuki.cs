using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Newkuttuki_move_state
{
    move_up,
    move_down,
    move_right,
    move_left,
    none
}
public class NewKuttuki : Padinput
{
    /* 【ポジション変数】 */
    private Vector3 kuttuki_pos;       　　　　　　　　 /* くっつき時、瞬時にくっつくためのポジション移動に使用 */

    [SerializeField] private Vector3 p_pos;             /* プレイヤー位置 */
    [SerializeField] private Vector3 obj_pos;           /* くっつきオブジェクト位置 */
    [SerializeField] private Vector3 obj_scale;         /* くっつきオブジェクトスケール */

    /* 【グラビティ4方向】 */
    Vector3 gravity_up = new Vector3(0, 9.8f, 0);       /* 上方向 */
    Vector3 gravity_down = new Vector3(0, -9.8f, 0);    /* 下方向 */
    Vector3 gravity_right = new Vector3(9.8f, 0, 0);    /* 右方向 */
    Vector3 gravity_left = new Vector3(-9.8f, 0, 0);    /* 左方向 */

    /* 【フラグ】 */
    public bool bool_ray_hit;                           /* true:くっつき状態 false:未くっつき状態 */
    public bool collider_exit;                          /* true:オブジェクトから離れました false:くっついている、または、ある程度時間が経過した*/

    [SerializeField] private bool jump;                 /* true:ジャンプ中です false:ジャンプしていません */
    [SerializeField] private bool bool_turn;            /* true:これ以上回転出来ません false:回転できます */
    [SerializeField] private bool bool_init;            /* true:初期化出来ます false:初期化できません */
    [SerializeField] private bool bool_exit;            /* true:くっつきオブジェクトから離れて一定時間たって強制的に初期化されました  false:何も起きません */
    [SerializeField] private bool bool_float;           /* true:プレイヤーが浮いてしまう可能性があります(対策済み) false:可能性ゼロ */
    [SerializeField] private bool kuttuki_down;         /* true:オブジェクトの下にくっついている false:オブジェクトの下にくっついていない 靴切り替え時の床貫通対策時に追加 */
    [SerializeField] private bool bool_kinematic;       /* true:Rididbodyのキネマティックをオンにします false:オフにします */
    [SerializeField] private bool bool_on_collision;    /* true:今オブジェクトにくっついています false:くっついていません */
    [SerializeField] private bool kuttuki_To_kuttuki;   /* true:くっついている状態から別のくっつきに移った false:移っていない */
    [SerializeField] private bool bool_oncollision_down;/* true:kuttuku_downオブジェクトにくっついています false:くっついていません */

    /* 【enum(列挙変数)】 */
    [SerializeField] Newkuttuki_move_state move_type; 　/* くっつきobjからみたプレイヤーの移動状態 */ 

    /* 【他スクリプトの取得】 */
    [SerializeField] private PlayerMove player;         /* 左スティックの入力状態の取得に使用 */
    [SerializeField] private ChangeShoes change_shoes;  /* 現在装着している靴の能力の取得に使用 */

    /* 【int型変数】 */
    [SerializeField] private int kuttuki_time;                                   /* くっつきオブジェクトから離れたときの時間 */
    [SerializeField] private int collision_time;                         

    /* 【float型変数】 */
    float move_x = 0f; 　　　　　　　　　　　　　　　 /* 最初のくっつき時の浮く問題を強制的にポジションに代入させて解決 */
    float move_y = 0.2f;

    /* 【レイキャスト変数まとめ】 */
    Ray foot_ray;                                       /* 足元に向けるレイ */
    Ray front_ray;                                      /* 正面に向けるレイ */

    RaycastHit rayHit;                                  /* レイ当たり判定を保持する変数 */
    
    Vector3 rayPosition;                                /* レイキャストの位置 */
    [SerializeField] private float rayDistance = 0.4f; /*（レイキャスト）可視光線の長さ */

    /* プレイヤーにアタッチされているコンポネント変数 */
    Animator animator;　　　　　　　　　　　　　　　　　/* 滑らない処理に使用 */
    public Rigidbody rb;    　　　　　　　　　　　　　　/* 滑らない処理に使用 */

    /* オブジェクト名を記録する */
    [SerializeField] private string obj_tag;
    void Start()
    {
        /* 【コンポネントの取得】 */
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerMove>();
        animator = GetComponent<Animator>();
        change_shoes = GetComponent<ChangeShoes>();

        /* 【フラグの初期化】 */
        jump = false;
        bool_turn = false;
        bool_init = false;
        bool_float = false;
        bool_ray_hit = false;
        kuttuki_down = false;
        collider_exit = false;
        bool_kinematic = false;
        bool_on_collision = false;
        kuttuki_To_kuttuki = false;
        bool_oncollision_down = false;

        /* 【移動状態の初期化】 */
        move_type = Newkuttuki_move_state.none;

        /* 【くっつき時間の初期化】 */
        collision_time = 0;
    }
    private void Update()
    {
        /* 【滑ってしまう問題対策１没になった処理】 */
        //if (change_shoes.type == ShoesType.Magnet_Shoes && bool_ray_hit == true) /*靴のタイプがマグネットの時*/
        //{
        //    animator.applyRootMotion = true;
        //    if(jump == true)
        //    {
        //        animator.applyRootMotion = false;
        //    }
        //}
        //else
        //{
        //    animator.applyRootMotion = false;
        //}

    }
    void FixedUpdate()
    {
        /* 【滑ってしまう問題対策3(完成版)】 */
        if(bool_ray_hit == true && jump == false)
        {

            collision_time++;

            if (collision_time < 20)
            {
                rb.velocity = Vector3.zero;
            }
        }
        else
        {
            collision_time = 0;
        }

        /* 【回転の制御処理】 */
        TurnControll();

        /* 【滑ってしまう問題対策２(未完成)】 */
        /* 変なタイミングで回転したり浮いてしまう問題を確認 */
        //if (bool_ray_hit == true)
        //{
        //    if (collider_exit == false)
        //    {
               
        //    }

        //    if (jump == false && bool_kinematic == false && bool_on_collision == true)
        //    {
        //        collision_time++;
        //        if(collision_time < 10)
        //        {
        //            rb.useGravity = false;
        //            rb.isKinematic = true;
        //        }
        //        else
        //        {
        //            rb.useGravity = true;
        //            rb.isKinematic = false;
        //        }
        //    }
        //    else if (jump == true)
        //    {
        //        collision_time = 0;

        //        rb.useGravity = true;
        //        rb.isKinematic = false;
        //    }
        //}
        //else /* 未くっつき状態の時 */
        //{
        //    collision_time = 0;

        //    rb.useGravity = true;
        //    rb.isKinematic = false;
        //}

        /* 【移動状態の取得】 */
        MoveState();

        /* 【レイの設定】 */
        RaySet();

        if(bool_init == false) /* 初期化フラグがfalseの時 */
        {
            /* 【靴がマグネットの時の処理】 */
            if (change_shoes.type == ShoesType.Magnet_Shoes)
            {
                /* 【くっつきの処理】 */
                Kuttuki();
            }
            /* 【それ以外の靴に切り替わったとき】 */
            else
            {
                /* 【靴を切り替えたときの処理】 */
                Change_shoes();
            }
        }
        else if(bool_init == true) /* 初期化フラグがtrueの時 */
        {
            /* 【初期化処理】 */
            Initialized();
        }
    }/* Updateの終了部分 */

    void TurnControll () /* 【回転の制御処理】 */
    {
        /* 【外側回転で複数回連続で回転してしまう問題対策】 */
        if (bool_ray_hit == true)
        {
            if (!Physics.Raycast(foot_ray, out rayHit, rayDistance))
            {
                Debug.Log("回転を可能にする");
                bool_turn = false;
            }
        }
        /* 【内側回転で複数回連続で回転してしまう問題対策】 */
        if(bool_ray_hit == true)
        {
            if (kuttuki_To_kuttuki == true)
            {
                bool_turn = false;
                bool_float = false;
                /* 【比較に使う値】 */
                float Comparison = 1f;

                /* 【引き算に使う値】 */
                float large = 0f;
                float small = 0f;

                if (Mathf.Abs(Physics.gravity.y) == 9.8f)
                {
                    if (Mathf.Floor(transform.position.x) > Mathf.Floor(p_pos.x))
                    {
                        large = Mathf.Floor(transform.position.x);
                        small = Mathf.Floor(p_pos.x);
                    }
                    else if (Mathf.Floor(transform.position.x) < Mathf.Floor(p_pos.x))
                    {
                        large = Mathf.Floor(p_pos.x);
                        small = Mathf.Floor(transform.position.x);
                    }

                    if (large - small > Comparison)
                    {
                        kuttuki_To_kuttuki = false;
                        Debug.Log("kuttuki_To_kuttukiを無効化にしてます");
                    }
                }
                else if (Mathf.Abs(Physics.gravity.x) == 9.8f)
                {
                    if (Mathf.Floor(transform.position.y) > Mathf.Floor(p_pos.y))
                    {
                        large = Mathf.Floor(transform.position.y);
                        small = Mathf.Floor(p_pos.y);
                    }
                    else if (Mathf.Floor(transform.position.y) < Mathf.Floor(p_pos.y))
                    {
                        large = Mathf.Floor(p_pos.y);
                        small = Mathf.Floor(transform.position.y);
                    }
                    if (large - small > Comparison)
                    {
                        kuttuki_To_kuttuki = false;
                        Debug.Log("kuttuki_To_kuttukiを無効化にしてます");
                    }
                }

            }
            else
            {
                /* 壁のくっつきオブジェから下にあるくっつきオブジェクトに
                ぶつかったときに浮いてしまう対策用フラグ */
                bool_float = true;
            }
        }
    }

    void MoveState() /* 移動状態の判定処理 */
    {
        if (bool_ray_hit == true)
        {
            /* 【上下の移動状態の取得】 */
            if (Mathf.Abs(Physics.gravity.x) == 9.8f) /* 重力は左右のどちらかの時 */
            {
                if(kuttuki_To_kuttuki == true)
                {
                    if (this.transform.position.y > obj_pos.y /*+ (obj_scale.y / 2)*/)
                    {
                        move_type = Newkuttuki_move_state.move_up;
                    }
                    else if (this.transform.position.y < obj_pos.y/* - (obj_scale.y / 2)*/)
                    {
                        move_type = Newkuttuki_move_state.move_down;
                    }
                }
                else
                {
                    if (this.transform.position.y > obj_pos.y + (obj_scale.y / 2))
                    {
                        move_type = Newkuttuki_move_state.move_up;
                    }
                    else if (this.transform.position.y < obj_pos.y - (obj_scale.y / 2))
                    {
                        move_type = Newkuttuki_move_state.move_down;
                    }
                    /* 【お試し処理】 */
                    else
                    {
                        move_type = Newkuttuki_move_state.none;
                        //bool_turn = true;
                    }
                }

            }

            /* 【左右の移動状態の取得】 */
            if (Mathf.Abs(Physics.gravity.y) == 9.8f)/* 重力は上下のどちらかの時 */
            {
                if (kuttuki_To_kuttuki == true)
                {
                    if (this.transform.position.x > obj_pos.x /*+ (obj_scale.x / 2)*/)
                    {
                        move_type = Newkuttuki_move_state.move_right;
                    }
                    else if (this.transform.position.x < obj_pos.x /*- (obj_scale.x / 2)*/)
                    {
                        move_type = Newkuttuki_move_state.move_left;
                    }
                }
                else if(kuttuki_To_kuttuki == false)
                {
                    if (this.transform.position.x > obj_pos.x + (obj_scale.x / 2))
                    {
                        move_type = Newkuttuki_move_state.move_right;
                    }
                    else if (this.transform.position.x < obj_pos.x - (obj_scale.x / 2))
                    {
                        move_type = Newkuttuki_move_state.move_left;
                    }
                    /* 【お試し処理】 */
                    else
                    {
                        move_type = Newkuttuki_move_state.none;
                        //bool_turn = true;
                    }
                }

            }
            /*左右*/
            Debug.Log(move_type);
        }
    }
    void RaySet() /* 【レイキャストの設定】 */
    {
        rayPosition = transform.position;    /*レイキャストの位置*/

        foot_ray = new Ray(rayPosition, transform.up * -1f);     /*足元*/
        front_ray = new Ray(rayPosition, transform.right * 1f);  /*正面*/

        /* デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, foot_ray.direction * rayDistance, Color.yellow);
        Debug.DrawRay(rayPosition, front_ray.direction * rayDistance, Color.yellow);
    }
    void Kuttuki() /* 【くっつきの処理】 */
    {
        //Debug.Log("くっつき処理に入っています");

        /* 【離れたときのフラグ変更処理】 */
        FootRayExit();

        /* 【正面のレイの処理】 */
        Front_Ray();

        /* 【足元のレイの処理】 */
        if (bool_oncollision_down == false)
        {
            Foot_Ray();
        }
    }
    void Front_Ray() /* 【プレイヤーの正面に出ているレイの処理】 */
    {
        /* 【正面のレイが当たっている時】*/
        if(Physics.Raycast(front_ray,out rayHit, rayDistance))
        {
            //Debug.Log(rayHit.collider.tag);
            /* 【まだくっつき判定前で、レイが当たったオブジェクトのタグがくっつくの時】 */
            if (bool_ray_hit == false && rayHit.collider.tag == "kuttuku")
            {

                Debug.Log("はじめてくっつく処理に入っています");

                /* 【プレイヤーの浮き対策】 */
                Vector3 position = this.transform.position;

                /* 【重力を変更する】 */
                if (player.right != 0) /* 壁が右の時 */
                {
                    move_x = 0.4f;
                }
                else /* 壁が左の時 */
                {
                    move_x = -0.4f;
                }

                /* 【プレイヤーの浮き対策】 */
                kuttuki_pos = new Vector3(move_x, 0, 0); /* これでくっつきと同時に壁の方向に近づき直ぐくっつける */
                this.transform.position = position + kuttuki_pos;

                /* 【重力を変更する】 */
                if (player.right != 0) /* 壁が右の時 */
                {
                    Physics.gravity = gravity_right;
                }
                else /* 壁が左の時 */
                {
                    Physics.gravity = gravity_left;
                }

                /* 【回転させる準備】 */
                Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                Quaternion q = this.transform.localRotation;

                /* 【実際に回転させる】 */
                this.transform.localRotation = q * rot;

                /* 【くっつき判定を変更】 */
                bool_ray_hit = true; /* これでくっつき判定になる */

                /* 【オブジェクト情報取得】 */
                Debug.Log("くっつき状態に変更＋オブジェクト情報更新");
                obj_pos = rayHit.transform.position;
                obj_scale = rayHit.transform.lossyScale;
                obj_tag = rayHit.collider.tag;
            }
            /* 【地面にぶつかったとき】 */
            else if (rayHit.collider.tag == "ground" && bool_ray_hit == true && move_type == Newkuttuki_move_state.move_down)
            {
                /* 【フラグ判定切り替え】 */
                Debug.Log("地面にぶつかって初期化");
                bool_init = true;
            }
            /* 【くっつき状態でもう一つのくっつきオブジェクトに接触した場合】 */
            else if (bool_ray_hit == true && rayHit.collider.tag == "kuttuku"/* && bool_turn == false*/)
            {
                Debug.Log("内側回転処理入りました");
                /* 【フラグ判定切り替え】 */
                kuttuki_To_kuttuki = true;
                bool_turn = false;

                /* 【プレイヤーポジション取得】 */
                p_pos = transform.position;

                /* 【オブジェクト情報取得】 */
                //Debug.Log("オブジェクト情報更新");
                //obj_pos = rayHit.transform.position;
                //obj_scale = rayHit.transform.lossyScale;
                //obj_tag = rayHit.collider.tag;

                /* 【内側回転処理】 */
                switch (move_type)
                {
                    case Newkuttuki_move_state.move_up:
                        if (this.transform.rotation.z != 180f || this.transform.rotation.z != 0f)
                        {
                            /* 【回転前準備】 */
                            Quaternion rotation = this.transform.localRotation;
                            Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                            Quaternion q = this.transform.localRotation;

                            /* 【実際に回転させる】 */
                            this.transform.localRotation = q * rot;

                            /* 【念のため用にした処理】 */
                            //if (player.right != 0)
                            //{

                            //}
                            //else if (player.right != 0)
                            //{

                            //}

                            /* 【外側回転処理に入ってしまう問題対策】 */
                            Vector3 position = this.transform.localPosition;
                            kuttuki_pos = new Vector3(0, 0.3f, 0);

                            /* 【実際に移動させる】 */
                            this.transform.localPosition = position + kuttuki_pos;

                            /* 【重力の向きを変更】 */
                            Physics.gravity = gravity_up;

                            /* 【フラグ判定切り替え】 */
                            bool_turn = true;
                            kuttuki_down = true;
                            //collider_exit = false;
                        }
                        break;

                    case Newkuttuki_move_state.move_down:
                        if (this.transform.rotation.z != 0f)
                        {
                            /* 【回転前準備】 */
                            Quaternion rotation = this.transform.localRotation;
                            Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                            Quaternion q = this.transform.localRotation;

                            /* 【実際に回転させる】 */
                            this.transform.localRotation = q * rot;

                            /* 【念のため用にした処理】 */
                            //if (player.right != 0)
                            //{

                            //}
                            //else if (player.right != 0)
                            //{

                            //}

                            /* *****【重要】↓の処理を入れると上に浮いてしまう（問題解決済み）***** */

                            if (bool_float == false)
                            {
                                /* 【外側回転処理に入ってしまう問題対策】 */
                                Vector3 position = this.transform.position;
                                kuttuki_pos = new Vector3(0, -0.3f, 0);

                                /* 【実際に移動させる】 */
                                this.transform.position = position + kuttuki_pos;
                            }
                            else
                            {

                                /* 【外側回転処理に入ってしまう問題対策】 */
                                Vector3 position = this.transform.position;
                                kuttuki_pos = this.transform.position;
                                kuttuki_pos.y = kuttuki_pos.y - 0.3f;

                                /* 【実際に移動させる】 */
                                this.transform.position = /*position + */kuttuki_pos;
                            }


                            /* *****【重要】↑の処理を入れると上に浮いてしまう（問題解決済み）***** */

                            /* 【プレイヤーポジション取得】 */
                            //p_pos = transform.position;

                            /* 【重力の向きを変更】 */
                            Physics.gravity = gravity_down;

                            /* 【フラグ判定切り替え】 */
                            bool_turn = true;
                            collider_exit = false;
                        }
                        break;

                    case Newkuttuki_move_state.move_right:
                        if (Mathf.Abs(transform.rotation.z) != 90 && bool_turn == false)
                        {
                            /* 【回転前準備】 */
                            Quaternion rotation = this.transform.localRotation;
                            Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                            Quaternion q = this.transform.localRotation;

                            /* 【実際に回転させる】 */
                            this.transform.localRotation = q * rot;

                            /* 【念のため用にした処理】 */
                            //if (player.right != 0)
                            //{

                            //}
                            //else if (player.right != 0)
                            //{

                            //}

                            /* 【外側回転処理に入ってしまう問題対策】 */
                            Vector3 position = this.transform.localPosition;
                            kuttuki_pos = new Vector3(0.3f, 0, 0);

                            /* 【実際に移動させる】 */
                            this.transform.localPosition = position + kuttuki_pos;

                            /* 【プレイヤーポジション取得】 */
                            //p_pos = transform.position;

                            /* 【重力の向きを変更】 */
                            Physics.gravity = gravity_right;

                            /* 【フラグ判定切り替え】 */
                            bool_turn = true;
                            kuttuki_down = false;
                            collider_exit = false;
                        }
                        break;

                    case Newkuttuki_move_state.move_left:
                        if (Mathf.Abs(transform.rotation.z) != 90)
                        {
                            /* 【回転前準備】 */
                            Quaternion rotation = this.transform.localRotation;
                            Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                            Quaternion q = this.transform.localRotation;

                            /* 【実際に回転させる】 */
                            this.transform.localRotation = q * rot;

                            /* 【念のため用にした処理】 */
                            //if (player.right != 0)
                            //{

                            //}
                            //else if (player.right != 0)
                            //{

                            //}

                            /* 【外側回転処理に入ってしまう問題対策】 */
                            Vector3 position = this.transform.localPosition;
                            kuttuki_pos = new Vector3(-0.3f, 0, 0);

                            /* 【実際に移動させる】 */
                            this.transform.localPosition = position + kuttuki_pos;

                            /* 【プレイヤーポジション取得】 */
                            //p_pos = transform.position;

                            /* 【重力の向きを変更】 */
                            Physics.gravity = gravity_left;

                            /* 【フラグ判定切り替え】 */
                            bool_turn = true;
                            kuttuki_down = false;
                            collider_exit = false;
                        }
                        break;
                }

            }
            /* 【移動するくっつきオブジェクトに接触した場合】 */
            else if (bool_ray_hit == true && rayHit.collider.tag == "kuttuku_down"/* && bool_turn == false*/)
            {
                Debug.Log("移動用くっつきオブジェクトにくっつきます");
                /* 【フラグ判定切り替え】 */
                bool_turn = false;
                kuttuki_To_kuttuki = true;
                bool_oncollision_down = true;

                /* 【プレイヤーポジション取得】 */
                p_pos = transform.position;

                if(move_type == Newkuttuki_move_state.move_up)
                {
                    if (this.transform.rotation.z != 180f)
                    {
                        /* 【回転前準備】 */
                        Quaternion rotation = this.transform.localRotation;
                        Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                        Quaternion q = this.transform.localRotation;

                        /* 【実際に回転させる】 */
                        this.transform.localRotation = q * rot;

                        /* 【念のため用にした処理】 */
                        //if (player.right != 0)
                        //{

                        //}
                        //else if (player.right != 0)
                        //{

                        //}

                        /* 【外側回転処理に入ってしまう問題対策】 */
                        Vector3 position = this.transform.localPosition;
                        kuttuki_pos = new Vector3(0, 0.3f, 0);

                        /* 【実際に移動させる】 */
                        this.transform.localPosition = position + kuttuki_pos;

                        /* 【重力の向きを変更】 */
                        Physics.gravity = gravity_up;

                        /* 【フラグ判定切り替え】 */
                        bool_turn = true;
                        kuttuki_down = true;
                    }
                }
            }
        }
    }
    void Foot_Ray() /* 【プレイヤーの足元に出ているレイの処理】 */
    {
        /* 【足元のレイが当たっている時】 */
        if (Physics.Raycast(foot_ray, out rayHit, rayDistance))
        {
            if (bool_ray_hit == true)
            {
                /* 【オブジェクト情報取得】 */
                obj_pos = rayHit.transform.position;
                obj_scale = rayHit.transform.lossyScale;
                obj_tag = rayHit.collider.tag;
            }
            /* 【重要】下の処理がないと、くっつきオブジェクトの上で一度未くっつきになった後オブジェクトの角で回転出来ない */
            /* 【そのレイが当たっているオブジェクトがkuttukuタグだった時】 */
            if (bool_ray_hit == false && rayHit.collider.tag == "kuttuku" || rayHit.collider.tag == "kuttuku_down") /* 左側の条件追加(4月2日) */
            {
                Debug.Log("くっつき状態に変更＋オブジェクト情報更新");
                bool_ray_hit = true; /* くっつき状態にする */
            }

            /* 下の処理はくっつき状態でそのまま移動していて足元がくっつきオブジェクトから別のオブジェクトに変わったときの処理 */
            if (bool_ray_hit == true && rayHit.collider.tag != "kuttuku" && rayHit.collider.tag != "kuttuku_down"  && !Physics.Raycast(front_ray, out rayHit, rayDistance)) /* この処理は上りでは使えない、回転 → 戻る → 落ちる → くっつくのループ */
            {
                Debug.Log("別のオブジェクトに接触しました");
                if (kuttuki_down == true) /* オブジェクトの下にくっついている状態の時 */
                {
                    /* 【フラグ判定切り替え】 */
                    Debug.Log("接触しているオブジェクトがくっつき以外の為初期化");
                    bool_init = true; /* 初期化可能状態に変更 */

                    /* 【床貫通対策の準備】 */
                    Vector3 player_pos = this.transform.position;
                    Vector3 plus_pos = new Vector3(0, -1, 0);

                    /* 【貫通しないように下にポジションを移動】 */
                    this.transform.position = player_pos + plus_pos;
                }
                else
                {
                    /* 【フラグ判定切り替え】 */
                    Debug.Log("接触しているオブジェクトがくっつき以外の為初期化");
                    bool_init = true; /* 初期化可能状態に変更 */
                }
            }
        }
        /* くっつき状態で足元のレイが当たっていなくてオブジェクトから離れた時 */
        else if (bool_ray_hit == true && collider_exit == true)
        {
            /*離れてから時間経過が長い場合の処理*/
            kuttuki_time++;
            if (jump == true)
            {

                if (kuttuki_time > 53) //47はジャンプの滞空時間
                {
                    /* 【フラグ判定切り替え】 */
                    bool_exit = true;
                    Debug.Log("ジャンプ後一定時間離れている為初期化");
                    bool_init = true;
                }
            }
            else if(jump == false)
            {
                //if (kuttuki_time > 5) //回転時に離れた時
                //{
                //    /* 【フラグ判定切り替え】 */
                //    bool_exit = true;
                //    bool_init = true;
                //}
                kuttuki_time = 0;

                /* 【外側回転処理】 */
                if (obj_tag != "kuttuku_down" && jump == false && bool_exit == false && bool_on_collision == true && bool_turn == false && kuttuki_To_kuttuki == false)
                {
                    
                        Debug.Log("外側回転処理入りました");
                        /* 【外側回転処理】 */
                        switch (move_type)
                        {
                            case Newkuttuki_move_state.move_up:
                                if (transform.rotation.z != 0)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Physics.gravity = gravity_down;

                                    bool_turn = true;
                                    collider_exit = false;

                                    /* 【足元のレイが当たらずbool_turnが初期化されない問題対策】 */
                                    Vector3 position = this.transform.localPosition;

                                    if (player.right != 0)
                                    {
                                        kuttuki_pos = new Vector3(0.1f, -move_y, 0);
                                    }
                                    else if (player.left != 0)
                                    {
                                        kuttuki_pos = new Vector3(-0.1f, -move_y, 0);
                                    }

                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                break;

                            case Newkuttuki_move_state.move_down:
                                if (transform.rotation.z != 180)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Physics.gravity = gravity_up;

                                    bool_turn = true;
                                    kuttuki_down = true;
                                    collider_exit = false;

                                    /* 【足元のレイが当たらずbool_turnが初期化されない問題対策】 */
                                    Vector3 position = this.transform.localPosition;

                                    if (player.right != 0)
                                    {
                                        kuttuki_pos = new Vector3(-0.1f, move_y, 0);
                                    }
                                    else if (player.left != 0)
                                    {
                                        kuttuki_pos = new Vector3(0.1f, move_y, 0);
                                    }

                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                break;

                            case Newkuttuki_move_state.move_right:
                                if (Mathf.Abs(transform.rotation.z) != 90)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Physics.gravity = gravity_left;

                                    bool_turn = true;
                                    kuttuki_down = false;
                                    collider_exit = false;

                                    /* 【足元のレイが当たらずbool_turnが初期化されない問題対策】 */
                                    Vector3 position = this.transform.localPosition;

                                    if (player.right != 0)
                                    {
                                        kuttuki_pos = new Vector3(-move_y, -0.1f, 0);
                                    }
                                    else if (player.left != 0)
                                    {
                                        kuttuki_pos = new Vector3(-move_y, 0.1f, 0);
                                    }

                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                break;

                            case Newkuttuki_move_state.move_left:
                                if (Mathf.Abs(transform.rotation.z) != 90)
                                {
                                    Quaternion rotation = this.transform.localRotation;
                                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                                    Quaternion q = this.transform.localRotation;

                                    this.transform.localRotation = q * rot;

                                    Physics.gravity = gravity_right;

                                    bool_turn = true;
                                    kuttuki_down = false;
                                    collider_exit = false;

                                    /* 【足元のレイが当たらずbool_turnが初期化されない問題対策】 */
                                    Vector3 position = this.transform.localPosition;

                                    if (player.right != 0)
                                    {
                                        kuttuki_pos = new Vector3(move_y, 0.1f, 0);
                                    }
                                    else if (player.left != 0)
                                    {
                                        kuttuki_pos = new Vector3(move_y, -0.1f, 0);
                                    }

                                    this.transform.localPosition = position + kuttuki_pos;
                                }
                                break;
                        }
                    
                    
                }
            }
        }
    }
    //void Kuttuki_type_down() /* 【下にしかくっつけないタイプの処理】 */
    //{

    //}
    void Change_shoes() /* 【靴を切り替えたときの処理】 */
    {
        Physics.gravity = gravity_down;

        /* 【重要】オブジェクト下にくっついている状態で靴を切り替えると貫通する問題対策 */
        if (kuttuki_down == true)
        {
            /* 【床貫通対策の準備】 */
            Vector3 player_pos = this.transform.position;
            Vector3 plus_pos = new Vector3(0, -1, 0);

            /* 【貫通しないように下にポジションを移動】 */
            this.transform.position = player_pos + plus_pos;

            /* 【回転させる準備】 */
            Quaternion rot = Quaternion.AngleAxis(180f, Vector3.forward);
            Quaternion q = this.transform.localRotation;

            /* 【実際に回転させる】 */
            this.transform.localRotation = q * rot;

            kuttuki_down = false; /* オブジェクト下のくっつき判定解除 */
            
        }
        /* 【くっつき状態判定だったら】 */ 
        if(bool_ray_hit == true)
        {
            Debug.Log("くっつかない原因かも");
            Debug.Log("靴を切り替えたので初期化");
            bool_init = true; /* 初期化可能状態に変更 */
        }
    }

    private void Initialized() /* 【未くっつき状態に戻す処理】 */
    {
        if (bool_init == true/* && bool_ray_hit == false*/) /* 未くっつき状態で初期化可能な時 */
        {
            Debug.Log("初期化処理に入りました");

            /* 【重力の初期化】 */
            Physics.gravity = gravity_down;

            /* 【フラグ判定切り替え】 */
            bool_init = false; /* 初期化不能にする */
            bool_ray_hit = false; /* 未くっつき状態にする */
            bool_on_collision = false; /* くっついたことのない状態にする */
            kuttuki_down = false; /* 下にくっついていたという判定解除 */
            kuttuki_To_kuttuki = false; /* くっつきからくっつきに移動した判定解除 */
            collider_exit = false; /* くっつき状態でのオブジェクトから離れた判定解除 */
            bool_exit = false;

            /* 【くっつきobjから離れた後の時間の初期化】 */
            kuttuki_time = 0;

            /* 【くっつき時の移動状態を初期化】 */
            move_type = Newkuttuki_move_state.none; /* 初期化しておかないと過去の移動状態が保存されたままになる */

            if (obj_tag == "kuttuku_down")
            {
                if(transform.rotation.z != 180f || transform.rotation.z != 0f)
                {
                    /* 【回転させる準備】 */
                    Quaternion rot = Quaternion.AngleAxis(180f, Vector3.forward);
                    Quaternion q = this.transform.localRotation;

                    /* 【実際に回転させる】 */
                    this.transform.localRotation = q * rot;
                }

                if (kuttuki_down == true) /* オブジェクトの下にくっついている状態の時 */
                {
                    /* 【フラグ判定切り替え】 */
                    bool_init = true; /* 初期化可能状態に変更 */

                    /* 【床貫通対策の準備】 */
                    Vector3 player_pos = this.transform.position;
                    Vector3 plus_pos = new Vector3(0, -1, 0);

                    /* 【貫通しないように下にポジションを移動】 */
                    this.transform.position = player_pos + plus_pos;

                    /* メモ:右入力字 左入力時のポジション移動を入れるといいかも？ */
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jump = false;
        /* まだくっつき判定で一度離れて再びオブジェクトにくっついた時 */
        if ((collision.gameObject.tag == "kuttuku" || collision.gameObject.tag == "kuttuku_down") &&  bool_ray_hit == true && collider_exit == true)
        {
            bool_on_collision = true;
            kuttuki_time = 0;
            collider_exit = false; /* 離れた時のフラグをfalseに */
            bool_kinematic = false;
        }
        else if ((collision.gameObject.tag == "kuttuku" || collision.gameObject.tag == "kuttuku_down") && bool_ray_hit == true)
        {
            bool_on_collision = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        kuttuki_time = 0;
        //rb.velocity = Vector3.zero;
        collider_exit = false; /* 離れた時のフラグをfalseに */
    }
    private void OnCollisionExit(Collision collision)
    {
        Debug.Log(collision.collider.tag);
        if (collision.collider.tag == "kuttuku_down")
        {
            Debug.Log("kuttuku_downから離れました");
            bool_init = true;
        }
    }
    private void FootRayExit()
    {
        /* くっつき状態で足元のレイがオブジェクトから離れたら */
        if(bool_ray_hit == true && !Physics.Raycast(foot_ray, out rayHit, rayDistance))
        {
            Debug.Log("離れました");
            collider_exit = true;
        }
    }
    public override void Jump()
    {
        //bool_kinematic = true;
        //rb.isKinematic = false;

        jump = true;
    }
}
