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
    /* プレイヤー */
    [SerializeField] private Vector3 p_pos;

    /* くっついてるオブジェクト */
    [SerializeField] private Vector3 obj_pos;

    /* グラビティ4方向 */
    Vector3 gravity_up = new Vector3(0, 9.8f, 0);
    Vector3 gravity_down = new Vector3(0, -9.8f, 0);
    Vector3 gravity_right = new Vector3(9.8f, 0, 0);
    Vector3 gravity_left = new Vector3(-9.8f, 0, 0);

    /* フラグ */
    public bool collider_exit;                          /* true:オブジェクトから離れました false:くっついている、または、ある程度時間が経過した*/
    public bool bool_ray_hit;                           /* true:くっつき状態 false:未くっつき状態 */
    [SerializeField] private bool kuttuki_To_kuttuki;   /* true:くっついている状態から別のくっつきに移った false:移っていない */
    [SerializeField] private bool kuttuki_down;         /* true:オブジェクトの下にくっついている false:オブジェクトの下にくっついていない 靴切り替え時の床貫通対策時に追加 */
    [SerializeField] private bool jump;                 /* true:ジャンプ中です false:ジャンプしていません */
    [SerializeField] private bool bool_turn;            /* true:これ以上回転出来ません false:回転できます */
    [SerializeField] private bool bool_init;            /* true:初期化出来ます false:初期化できません */
    [SerializeField] private bool bool_kinematic;
    [SerializeField] private bool bool_on_collision;
    [SerializeField] private bool bool_float;
    [SerializeField] private bool bool_exit;

    /*グラビティの方向を変更するのに必要*/
    Newkuttuki_move_state move_type;
    Newkuttuki_move_state old_move_type;
    PlayerMove player;
    Vector3 now_gravity;
    
    /* 【プレイヤーの移動状態の取得】 */
    private Vector3 _prevPosition;
    private Vector3 now_position;

    /* 【プレイヤーの回転状態の取得】 */
    Quaternion now_rot;
    Quaternion old_rot;

    int kuttuki_time; /*くっつきオブジェクトから離れたときの時間*/
    /*くっつき時の浮く問題解決に必要*/
    float move_x = 0.5f;

    //bool kuttuki_

    /*ジャンプ移動の不具合修正に必要*/
    

    /*レイキャスト用変数*/
    public Vector3 rayPosition; /*レイキャストの位置*/

    [SerializeField] private ChangeShoes change_shoes;  /*能力切り替え用変数_ChangeShoes*/

    /*脳筋式レイキャスト変数*/
    public Ray foot_ray;     /*足元*/
    public Ray front_ray;    /*正面*/

    public RaycastHit rayHit;

    /*追加部分*/
    
    //bool kuttuki_left; /*true:左入力をしている状態でくっつきオブジェクトにくっつく*/
    /*追加部分*/

    /*（レイキャスト）可視光線の長さ*/
    [SerializeField] public float rayDistance = 0.5f;


    /*レイキャスト用変数*/

    public Rigidbody rb;    /*みんな大好きリジッドボデー*/

    /*くっついた時の浮き状態を改善するためのもの*/
    Vector3 kuttuki_pos;

    /* 滑る問題解決 */
    Animator animator;


    /* 既にくっついてる時の回転に必要 */
    Quaternion kuttuki_rot;

    void Start()
    {
        change_shoes = GetComponent<ChangeShoes>();

        player = GetComponent<PlayerMove>();

        rb = GetComponent<Rigidbody>(); /*リジッドボデー*/

        bool_ray_hit = false;

        collider_exit = false;
        move_type = Newkuttuki_move_state.none;
        // 初期位置を保持
        now_position = transform.position;
        _prevPosition = transform.position;

        jump = false;
        //kuttuki_left = false;
        move_x = 0.5f;
        kuttuki_To_kuttuki = false;
        kuttuki_down = false;

        animator = GetComponent<Animator>();
        bool_turn = false;
        bool_init = false;

        now_rot = transform.localRotation;
        old_rot = now_rot;

        bool_kinematic = false;
        bool_on_collision = false;
        bool_float = false;
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
        /* 【外側回転で複数回連続で回転してしまう問題対策】 */
        if(bool_ray_hit == true )
        {
            if (Physics.Raycast(foot_ray,out rayHit, rayDistance)) 
            {
                Debug.Log("回転を可能にする");
                bool_turn = false;
            }
        }
        /* 【内側回転で複数回連続で回転してしまう問題対策】 */
        if (kuttuki_To_kuttuki == true)
        {
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
                }
            }

        }
        else
        {
            /* 壁のくっつきオブジェから下にあるくっつきオブジェクトに
            ぶつかったときに浮いてしまう対策用フラグ */
            bool_float = true;
        }

        /* 【滑ってしまう問題対策２(未完成)】 */
        /* 変なタイミングで回転したり浮いてしまう問題を確認 */
        if (bool_ray_hit == true)
        {

            if (jump == false && bool_kinematic == false && bool_on_collision == true)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            else if (jump == true)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
        else
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }

        /* 【プレイヤーの現在の回転状態の取得】 */
        now_rot = transform.localRotation;

        /* 移動状態の取得 */
        MoveState();

        /* レイの設定 */
        RaySet();

        if(bool_init == false) /* 初期化フラグがfalseの時 */
        {
            /* 靴がマグネットの時の処理 */
            if (change_shoes.type == ShoesType.Magnet_Shoes)
            {
                Kuttuki();
            }
            /* それ以外の靴に切り替わったとき */
            else
            {
                Change_shoes();
            }
        }
        else if(bool_init == true) /* 初期化フラグがtrueの時 */
        {
            /* 初期化処理 */
            Initialized();
        }

        // 前フレーム位置を更新
        _prevPosition = now_position;
        old_rot = now_rot;
        old_move_type = move_type;

    }/*Updateの終了部分*/
    void MoveState() /* 移動状態の判定処理 */
    {
        /*【パターン１】*/
        //if (bool_ray_hit == true)
        //{

        //    // deltaTimeが0の場合は何もしない
        //    if (Mathf.Approximately(Time.deltaTime, 0))
        //        return;
        //    // 現在位置取得
        //    now_position = transform.position;
        //    // 現在速度計算
        //    var velocity = (now_position - _prevPosition) / Time.deltaTime;

        //    /*上下*/
        //    if (Physics.gravity == new Vector3(9.8f, 0, 0) || Physics.gravity == new Vector3(-9.8f, 0, 0))
        //    {
        //        Debug.Log("上下の移動判定がとられるはず");
        //        if (velocity.y > 0.0f) /*上に進んでいる*/
        //        {
        //            move_type = Newkuttuki_move_state.move_up;
        //        }
        //        else if (velocity.y < 0.0f)/*下に進んでいる*/
        //        {
        //            move_type = Newkuttuki_move_state.move_down;
        //        }
        //        /*上下*/
        //    }

        //    /*左右*/
        //    if (Physics.gravity == new Vector3(0, 9.8f, 0) || Physics.gravity == new Vector3(0, -9.8f, 0))
        //    {
        //        if (velocity.x > 0.0f) /*上に進んでいる*/
        //        {
        //            move_type = Newkuttuki_move_state.move_right;
        //        }
        //        else if (velocity.x < 0.0f)/*下に進んでいる*/
        //        {
        //            move_type = Newkuttuki_move_state.move_left;
        //        }
        //    }
        //    /*左右*/

        //    if (velocity == Vector3.zero)
        //    {
        //        move_type = Newkuttuki_move_state.none;
        //    }

        //    // 現在速度をログ出力
        //    //print($"velocity = {velocity}");

        //    Debug.Log(move_type);
        //    Debug.Log(velocity.x);
        //}

        /* 【パターン2】 */
        if (bool_ray_hit == true)
        {
            /* 【上下の移動状態の取得】 */
            if (Mathf.Abs(Physics.gravity.x) == 9.8f) /* 重力は左右のどちらかの時 */
            {
               if(this.transform.position.y > obj_pos.y)
               {
                    move_type = Newkuttuki_move_state.move_up;
               }
               else if(this.transform.position.y < obj_pos.y)
               {
                    move_type = Newkuttuki_move_state.move_down;
               }
            }

            /* 【左右の移動状態の取得】 */
            if (Mathf.Abs(Physics.gravity.y) == 9.8f)/* 重力は上下のどちらかの時 */
            {
                if (this.transform.position.x > obj_pos.x)
                {
                    move_type = Newkuttuki_move_state.move_right;
                }
                else if(this.transform.position.x < obj_pos.x)
                {
                    move_type = Newkuttuki_move_state.move_left;
                }
            }
            /*左右*/
            Debug.Log(move_type);
        }
    }
    void RotState()
    {
        now_rot = transform.localRotation;
    }
    void RaySet()
    {
        rayPosition = transform.position;    /*レイキャストの位置*/

        foot_ray = new Ray(rayPosition, transform.up * -1f);     /*足元*/
        front_ray = new Ray(rayPosition, transform.right * 1f);  /*正面*/

        /* デバッグ用の可視光線*/
        Debug.DrawRay(rayPosition, foot_ray.direction * rayDistance, Color.yellow);
        Debug.DrawRay(rayPosition, front_ray.direction * rayDistance, Color.yellow);
    }
    void Kuttuki()
    {
        Debug.Log("くっつき処理に入っています");

        /* 離れたときのフラグ変更処理 */
        FootRayExit();

        /* 正面のレイが当たったとき */
        Front_Ray();

        /* 足元のレイが当たったとき */
        Foot_Ray();

    }
    void Front_Ray()
    {
        /* 【正面のレイが当たっている時】*/
        if(Physics.Raycast(front_ray,out rayHit, rayDistance))
        {
            /* 【まだくっつき判定前で、レイが当たったオブジェクトのタグがくっつくの時】 */
            if (bool_ray_hit == false && rayHit.collider.tag == "kuttuku")
            {

                Debug.Log("はじめてくっつく処理に入っています");

                /* 【プレイヤーの浮き対策】 */
                Vector3 position = this.transform.position;

                /* 【重力を変更する】 */
                if (player.right != 0) /* 壁が右の時 */
                {
                    move_x = 0.2f;
                }
                else /* 壁が左の時 */
                {
                    move_x = -0.2f;
                }

                /* 【プレイヤーの浮き対策】 */
                Vector3 kuttuki_pos = new Vector3(move_x, 0, 0); /* これでくっつきと同時に壁の方向に近づき直ぐくっつける */
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

                /* 【オブジェクト位置取得】 */
                obj_pos = rayHit.transform.position;
            }
            /* 【地面にぶつかったとき】 */
            else if (rayHit.collider.tag == "ground" && bool_ray_hit == true && move_type == Newkuttuki_move_state.move_down)
            {
                /* 【フラグ判定切り替え】 */
                bool_init = true;
            }
            /* 【くっつき状態でもう一つのくっつきオブジェクトに接触した場合】 */
            else if (bool_ray_hit == true && rayHit.collider.tag == "kuttuku" && bool_turn == false)
            {
                /* 【フラグ判定切り替え】 */
                kuttuki_To_kuttuki = true;
                if (kuttuki_To_kuttuki == true)
                {
                    /* 【内側回転処理】 */
                    switch (move_type)
                    {
                        case Newkuttuki_move_state.move_up:
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

                                /* 【プレイヤーポジション取得】 */
                                p_pos = transform.position;

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

                                /* *****【重要】↓の処理を入れると上に浮いてしまう***** */

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


                                /* *****【重要】↑の処理を入れると上に浮いてしまう***** */

                                /* 【プレイヤーポジション取得】 */
                                p_pos = transform.position;

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
                                p_pos = transform.position;

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
                                p_pos = transform.position;

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
            }
        }
    }
    void Foot_Ray()
    {
        /* 【足元のレイが当たっている時】 */
        if (Physics.Raycast(foot_ray, out rayHit, rayDistance))
        {
            /* 【重要】下の処理がないと、くっつきオブジェクトの上で一度未くっつきになった後オブジェクトの角で回転出来ない */
            /* 【そのレイが当たっているオブジェクトがkuttukuタグだった時】 */
            if (rayHit.collider.tag == "kuttuku")
            {
                bool_ray_hit = true; /* くっつき状態にする */
                obj_pos = rayHit.transform.position;
            }
            /* 下の処理はくっつき状態でそのまま移動していて足元がくっつきオブジェクトから別のオブジェクトに変わったときの処理 */
            else if (bool_ray_hit == true && rayHit.collider.tag != "kuttuku" && !Physics.Raycast(front_ray, out rayHit, rayDistance)) /* この処理は上りでは使えない、回転 → 戻る → 落ちる → くっつくのループ */
            {
                //Debug.Log(rayHit.collider.tag);
                if (kuttuki_down == true) /* オブジェクトの下にくっついている状態の時 */
                {
                    /* 【フラグ判定切り替え】 */
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

                if (kuttuki_time > 47) //47はジャンプの滞空時間
                {
                    /* 【フラグ判定切り替え】 */
                    bool_exit = true;
                    bool_init = true;
                }
            }
            else if(jump == false)
            {
                if (kuttuki_time > 5) //回転時に離れた時
                {
                    /* 【フラグ判定切り替え】 */
                    bool_exit = true;
                    bool_init = true;
                }

                /* 【外側回転処理】 */
                if (bool_exit == false && bool_on_collision == true && bool_turn == false && kuttuki_To_kuttuki == false)
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
                                    kuttuki_pos = new Vector3(0.1f, 0, 0);
                                }
                                else if (player.left != 0)
                                {
                                    kuttuki_pos = new Vector3(-0.1f, 0, 0);
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
                                    kuttuki_pos = new Vector3(-0.1f, 0, 0);
                                }
                                else if (player.left != 0)
                                {
                                    kuttuki_pos = new Vector3(0.1f, 0, 0);
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
                                    kuttuki_pos = new Vector3(0, -0.1f, 0);
                                }
                                else if (player.left != 0)
                                {
                                    kuttuki_pos = new Vector3(0, 0.1f, 0);
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
                                    kuttuki_pos = new Vector3(0, 0.1f, 0);
                                }
                                else if (player.left != 0)
                                {
                                    kuttuki_pos = new Vector3(0, -0.1f, 0);
                                }

                                this.transform.localPosition = position + kuttuki_pos;
                            }
                            break;
                    }
                }
            }
        }
    }
    void Change_shoes()
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
            Quaternion rot = Quaternion.AngleAxis(180f, Vector3.right);
            Quaternion q = this.transform.localRotation;

            /* 【実際に回転させる】 */
            this.transform.localRotation = q * rot;

            kuttuki_down = false; /* オブジェクト下のくっつき判定解除 */
            
        }
        /* 【くっつき状態判定だったら】 */ 
        if(bool_ray_hit == true)
        {
            Debug.Log("くっつかない原因かも");
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
            //kuttuki_To_kuttuki = false; /* くっつきからくっつきに移動した判定解除 */
            collider_exit = false; /* くっつき状態でのオブジェクトから離れた判定解除 */
            bool_exit = false;

            /* 【くっつきobjから離れた後の時間の初期化】 */
            kuttuki_time = 0;

            /* 【くっつき時の移動状態を初期化】 */
            move_type = Newkuttuki_move_state.none; /* 初期化しておかないと過去の移動状態が保存されたままになる */
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        jump = false;
        /* まだくっつき判定で一度離れて再びオブジェクトにくっついた時 */
        if (collision.gameObject.tag == "kuttuku" && bool_ray_hit == true && collider_exit == true)
        {
            bool_on_collision = true;
            kuttuki_time = 0;
            collider_exit = false; /* 離れた時のフラグをfalseに */
            //kuttuki_To_kuttuki = false;
            bool_kinematic = false;
            Debug.Log("離れた後にくっついた判定");
        }
        else if (collision.gameObject.tag == "kuttuku" && bool_ray_hit == true)
        {
            bool_on_collision = true;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        kuttuki_time = 0;
        collider_exit = false; /* 離れた時のフラグをfalseに */
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
        bool_kinematic = true;
        rb.isKinematic = false;

        jump = true;
    }
}
