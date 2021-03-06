using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum State /*プレイヤーの移動状態*/
{
    idle,
    walk,
    run,
}
//[SerializeField]private DashSystem dashsystem;


public class PlayerMove : Padinput
{
    Vector3 player_pos2;

    //[SerializeField] NewKuttuki kuttuku;
    [SerializeField] DashSystem dash;

    /*プレイヤーの移動状態*/
    public State state;
    public bool idle;

    /*左右の入力値*/
    public float right;
    public float left;

    /*X軸の入力値*/
    float input_xAxis { get { return right + left; } }

    /*x軸の入力絶対値*/
    float input_abs { get { return System.Math.Abs(input_xAxis); } }

    /*実際にプレイヤーにかけるx値*/
    float move_x;
    /*move_xの最大値*/
    [SerializeField] private float max_move_x; /*通常時13fがいいかも 速くして15f～20f*/

    /*動きベクトルにかけるスピード値*/
    float speed = 5f;
    //float max_speed = 50f;

    /*動きのベクトル*/
    Vector3 move;

    Quaternion player_direction; //プレイヤーの方向
    bool bool_left_direction; //true:左に向いています
    bool bool_right_direction; //true:右に向いています

    /*イージング用*/
    [SerializeField] private float eas_time = 2f;/*イージングをかける時間*/
    private float run_time;/*走り始めてからの時間*/

    //アニメーション用
    [SerializeField] Animator animator;
    [SerializeField] float anim_speed;

    /*Kuttukuで使うプレイヤーポジション*/
    //[SerializeField] GameObject player;
    //public Vector3 player_pos {get{ return player.transform.position; } }



    /*壁のめり込み対策(しゅんver)*/
    [SerializeField] public float rayDistance = 1.2f;
    public Vector3 rayPosition; /*レイキャストの位置*/
    public Ray ray;    /*正面*/
    public Ray ray_back;

    public RaycastHit rayHit;
    Vector3 obj_pos;
    float obj_width;
    GameObject obj;
    Vector3 player_pos;
    bool hit_wall_right;
    bool hit_wall_left;

    bool kuttuki_hit_wall;

    float distance;

    //public float height;

    private static bool movePermit = true; /* キー入力による操作許可 */
    private static bool rotatePermit = true; /* 方向転換許可 */

    /* 4/18 仲里追加 */
    private static int banTaskCountMove = 0; /* 移動禁止コルーチン起動数 */
    private static int banTaskCountRotate = 0; /* 方向転換禁止コルーチン起動数 */

    /* 仲里追加以上 */

    private void Start() /*初期化*/
    {
        this.transform.localRotation = Quaternion.Euler(0, 0, 0);
        max_move_x = 13f;

        /*falseだとくっついた時にプレイヤーが下を向いてしまう*/
        bool_left_direction = true;
        bool_right_direction = true;
        /*falseだとくっついた時にプレイヤーが下を向いてしまう*/
        //move = Vector3.zero;
        idle = true;
        //height = 0;

        anim_speed = 0.1f;

        right = 1; /*最初何も入力していない状態でくっつきの靴に切り替えると入力と移動する方向が逆になる*/
        kuttuki_hit_wall = false;
        dash = GetComponent<DashSystem>();
    }

    /*オーバーライド関数(自動で呼び出される 呼び出しタイミングはコントローラー割り当てがされているボタン、スティックが入力された時)*/
    public override void Move() /*動かす時の左スティック入力状態をここで取得*/
    {
        if (Gamepad.current.leftStick.x.ReadValue() > 0)
        {
            right = Gamepad.current.leftStick.x.ReadValue();
            left = 0;
        }


        if (Gamepad.current.leftStick.x.ReadValue() < 0)
        {
            left = Gamepad.current.leftStick.x.ReadValue();
            right = 0;
        }



        if (input_abs <= 0.5f)
        {
            
            var max_input = 0.5f; /* 歩き時の入力の最大値 */

            state = State.walk; /*歩き*/

            /* 【走り→歩きのアニメーション遷移】 */
            //アニメーションの1～0.1の値をinput値で操作する
            anim_speed = input_abs / max_input;

            /*壁のめり込み対策(しゅんver)*/
            if (hit_wall_right == false && hit_wall_left == false)
            {
                move_x = 2f; /*プレイヤーを回転させれば符号を変える必要はない*/
            }
            else
            {
                move_x = 0;
            }
            //move_x = 2f; /*上の処理を入れる場合コメントアウト*/

        }
        else if (input_abs > 0.5f)
        {
            state = State.run; /*走り*/

        }


    }
    public override void MoveStop()/*プレイヤーの動きを止める処理*/
    {

        state = State.idle; /*止まっている*/
        move_x = 0;
        move = Vector3.zero;

        /*歩きアニメーションに戻すための処理*/
        anim_speed = 0.1f;
        player_pos2 = transform.position;
    }
    /*オーバーライド関数*/

    void Update() /*常に処理する内容*/
    {
        //Debug.Log(distance);
        Debug.Log(input_abs);
        if (state == State.idle)
        {
            idle = true;
        }
        else
        {
            idle = false;
        }

        ApplyAnimator();

        //transform.Translate((move / 10) * speed * Time.deltaTime); /*Update内だと壁にめり込む*/

    }
    private void FixedUpdate()
    {
        Turn();
        Run();
        /*壁のめり込み対策(しゅんver)*/
        rayPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        rayPosition.y += 1f;
        ray = new Ray(rayPosition, transform.right * 1f);
        ray_back = new Ray(rayPosition, transform.right * -1f);
        WallHit();
    }
    private void Run()/*走る処理*/
    {
        //Debug.Log(state);/*プレイヤーの状態*/

        switch (state)
        {
            case State.idle: /*止まっている時*/
                run_time = 0;
                move_x = 0;
                break;

            case State.walk: /*歩き*/
                run_time = 0;
                break;

            case State.run: /*走り*/
                RunAccel();
                break;
        }
        /*壁にめり込まないようにする処理(自分版)*/
        if (hit_wall_left == true || hit_wall_right == true)
        {
            move = Vector3.zero;
        }
        else if (hit_wall_left == false && hit_wall_right == false)
        {
            move = new Vector3(move_x, 0, 0);
        }
        /*壁にめり込まないようにする処理(自分版)*/
        /*↓めり込まないようにする処理を使う場合下はコメントアウト*/
        //move = new Vector3(move_x, 0, 0);

        if (state == State.idle)
        {
            move = Vector3.zero;
        }
        //transform.localPosition += ((move / 10) * speed * Time.deltaTime);

        /* movePermitがfalseなら実行しない事で入力があっても移動させない */
        /* この処理だと止めているのに入力に応じてアニメする可能性大 */
        if (movePermit)
        {
            transform.Translate((move / 10) * speed * Time.deltaTime);
        }


    }
    private void Turn() /*振り向き処理*/
    {
        if (rotatePermit) /*くっつき状態ではない場合*/
        {
            /*下の処理の効果：くっついた際にプレイヤーが下を向いてしまう問題を改善*/
            bool_left_direction = true;
            bool_right_direction = true;

            if (right != 0)/*左スティックが右に入力されているとき*/
            {
                /*プレイヤーの向きを右に向いている状態にする処理*/
                if (transform.localRotation != Quaternion.Euler(0, 0, 0))
                {
                    player_direction = Quaternion.Euler(0, 0, 0); /*Quaternion.Eulerで向きを3軸(xyz)まとめて値を指定したものをプレイヤーの向きを入れる変数に代入*/
                    transform.localRotation = player_direction; /*プレイヤーの向きをlocalRotationに代入して回転させる*/
                }
                /*ここにくっつきと同じやり方の回転を入れる*/
                //if(transform.localRotation.y != 0)
                //{
                //    /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
                //    Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定 ※x軸:Vector3.right y軸:Vector3.up z軸:Vector3.foward*/
                //    Quaternion q = this.transform.localRotation; /*これがないと現在の値から+〇度回転させた値にするということが出来ない*/

                //    /*localRotationに値を代入し、実際に回転させる*/
                //    this.transform.localRotation = q * rot;/*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
                //}

            }
            else if (left != 0)/*左スティックが左に入力されているとき*/
            {

                /*プレイヤーの向きを左に向いている状態にする処理*/
                if (transform.localRotation != Quaternion.Euler(0, 180, 0))
                {
                    //Debug.Log("左向くはず");
                    player_direction = Quaternion.Euler(0, 180, 0); /*Quaternion.Eulerで向きを3軸(xyz)まとめて値を指定したものをプレイヤーの向きを入れる変数に代入*/
                    transform.localRotation = player_direction; /*プレイヤーの向きをlocalRotationに代入して回転させる*/
                }


                /*ここにくっつきと同じやり方の回転を入れる*/
                //if (this.transform.localRotation.y != 270f) /*もし、localRotation.yの値が-90fじゃない時*/
                //{
                //    Debug.Log("左向くはず");
                //    /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
                //    Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定*/
                //    Quaternion q = this.transform.localRotation; /*現在の値を保持*/

                //    /*localRotationに値を代入し、実際に回転させる*/
                //    this.transform.localRotation = q * rot; /*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
                //    bool_left_direction = true;/*プレイヤーが左に向いているときのフラグをtrueに*/
                //}
            }
        }
        //else if (kuttuku.bool_ray_hit == true)/*くっつき状態の場合*/
        //{
        //    if (right != 0) /*左スティックが右に入力されている場合*/
        //    {
        //        bool_left_direction = false; /*左に振り向く時に使うフラグをfalseに*/
        //        if (bool_right_direction == false) /*右に振り向く時に必要なフラグがfalseの場合*/
        //        {
        //            /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
        //            Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定 ※x軸:Vector3.right y軸:Vector3.up z軸:Vector3.foward*/
        //            Quaternion q = this.transform.localRotation; /*これがないと現在の値から+〇度回転させた値にするということが出来ない*/
        //            /*【回転させる準備】*/

        //            /*localRotationに値を代入し、実際に回転させる*/
        //            this.transform.localRotation = q * rot;/*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
        //            bool_right_direction = true; /*プレイヤーが右に向いているときのフラグをtrueに*/

        //        }


        //    }
        //    else if (left != 0)/*左スティックが左に入力されている場合*/
        //    {
        //        bool_right_direction = false; /*プレイヤーが右に向いている時にtrueになるフラグをfalseに*/
        //        if (bool_left_direction == false) /*プレイヤーが左に向いているフラグがfalseの時*/
        //        {
        //            if (this.transform.localRotation.y != -90f) /*もし、localRotation.yの値が-90fじゃない時*/
        //            {
        //                /*【回転させる準備】 rotには回転させたい値と軸を指定したものを、qには現在のlocalRotationの値を代入*/
        //                Quaternion rot = Quaternion.AngleAxis(180, Vector3.up); /*y軸で180°回転するように指定*/
        //                Quaternion q = this.transform.localRotation; /*現在の値を保持*/

        //                /*localRotationに値を代入し、実際に回転させる*/
        //                this.transform.localRotation = q * rot; /*【q * rot】にすることで現在の値から〇°回転という処理が出来る*/
        //                bool_left_direction = true;/*プレイヤーが左に向いているときのフラグをtrueに*/
        //            }

        //        }


        //    }
        //}



    }

    private void RunAccel() /*走る時の加速処理*/
    {
        /*追加部分*/ /*プレイヤーを回転させる場合これのみでok*/
        run_time += Time.deltaTime; /*run_timeを少しずつ動かす*/
        if (run_time < eas_time) /*緩急をつけるトータルの時間よりrun_timeが小さければまだ加速処理*/
        {
            move_x = ExpOut(run_time, eas_time, 2f, max_move_x); /*move_xに2f～max_move_xの値を入れる処理(加速)*/

            /*↓プレイヤーの加速処理とアニメーションを合わせる為に追加した処理*/
            anim_speed = ExpOut(run_time, eas_time, anim_speed, 1f); /*ブレンドツリーに代入するスピードの値を0.1f～１fで加速させる処理*/
        }
        else /*緩急をつける時間を越えたら*/
        {
            move_x = max_move_x; /*最大値を維持*/

            /*↓プレイヤーの加速処理とアニメーションを合わせる為に追加した処理*/
            anim_speed = 1f; /*走りのアニメーションを維持するために1を代入*/
        }
    }
    public static float ExpOut(float t, float totaltime, float min, float max) /*加速処理に使う関数*/
    {
        max -= min;
        return t == totaltime ? max + min : max * (-Mathf.Pow(2, -10 * t / totaltime) + 1) + min;
    }
    void ApplyAnimator()
    {
        //var speed = Mathf.Abs(input_abs);
        //animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetFloat("Speed", anim_speed, 0.1f, Time.deltaTime);

        //if (speed == 0)
        //{
        //    state = State.idle;
        //    Debug.Log("止まり状態");
        //}
        //else if (speed <= 0.7f)
        //{
        //    state = State.walk;
        //    Debug.Log("歩き状態");
        //}
        //else if (speed > 0.7f)
        //{
        //    state = State.run;
        //    Debug.Log("走り状態");
        //}
    }
    /*壁にめり込まないようにする処理(自分版)*/
    private void WallHit()
    {
        /* 正面から出ているレイが当たったら */
        if (Physics.Raycast(ray, out rayHit, rayDistance))
        {
            //Debug.Log("れいが当たってる処理は正常です");
            if (rayHit.collider.tag == "kuttuku" || rayHit.collider.tag == "ground")
            {
                /*プレイヤーの位置と幅を取得*/
                var p_width = transform.lossyScale.x / 2; /*2で割ることにより壁に当たるほうのみの幅を出せる*/
                var p_pos = new Vector3((transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);

                /*レイが当たっているオブジェクトの位置を取得*/
                obj_pos = rayHit.transform.position;

                /* 右に入力されている時 */
                if (right != 0)
                {
                    p_pos.x = transform.localPosition.x - p_width;
                }
                /* 左に入力されている時 */
                else if (left != 0)
                {
                    p_pos.x = transform.localPosition.x + p_width;
                }


                if (obj_pos.x < p_pos.x) /*プレイヤーがオブジェクトの右側の時*/
                {
                    Debug.Log("左側のオブジェクトにレイが当たっています");
                    distance = (p_pos.x - obj_pos.x);
                }
                else if (p_pos.x < obj_pos.x) /*プレイヤーがオブジェクトの左側の時*/
                {
                    Debug.Log("右側のオブジェクトにレイが当たっています");
                    distance = (obj_pos.x - p_pos.x);
                }




                /*ポジションをめり込まないようにする処理*/
                if (right != 0 && distance <= 1.2f)
                {
                    Debug.Log("ここ通っていればめり込まないはず");
                    //player_oldpos = this.transform.position;
                    player_pos = new Vector3(obj_pos.x - 0.8f, transform.position.y, transform.position.z);
                    transform.position = player_pos;
                    hit_wall_right = true;
                }
                else if (left != 0 && distance <= 1.2f)
                {
                    Debug.Log("ここ通っていればめり込まないはず");
                    //player_oldpos = this.transform.position;
                    player_pos = new Vector3(obj_pos.x + 0.8f, transform.position.y, transform.position.z);
                    transform.position = player_pos;
                    hit_wall_left = true;
                }

                //hit_wall = true;
            }

        }
        else
        {
            hit_wall_right = false;
            hit_wall_left = false;
        }
            //if (kuttuku.bool_ray_hit == true && Physics.gravity == new Vector3(0, -9.8f, 0))
            //{
            //    if (Physics.Raycast(ray_back, out rayHit, rayDistance))
            //    {
            //        if (rayHit.collider.tag == "kuttuku" || rayHit.collider.tag == "ground")
            //        {
            //            /*プレイヤーの位置と幅を取得*/
            //            var p_width = transform.lossyScale.x / 2; /*2で割ることにより壁に当たるほうのみの幅を出せる*/
            //            var p_pos = new Vector3((transform.localPosition.x), transform.localPosition.y, transform.localPosition.z);

            //            /*レイが当たっているオブジェクトの位置を取得*/
            //            obj_pos = rayHit.transform.position;

            //            if (right != 0)
            //            {
            //                p_pos.x = transform.localPosition.x - p_width;
            //                //transform.position = new Vector3(rayHit.transform.position.x + 0.8f, transform.position.y, transform.position.z);
            //            }
            //            else if (left != 0)
            //            {
            //                p_pos.x = transform.localPosition.x + p_width;
            //                //transform.position = new Vector3(rayHit.transform.position.x - 0.8f, transform.position.y, transform.position.z);
            //            }

            //            /*プレイヤーとオブジェクトの間の距離を出す*/
            //            if (obj_pos.x < p_pos.x) /*プレイヤーがオブジェクトの右側の時*/
            //            {
            //                Debug.Log("左側のオブジェクトにレイが当たっています");
            //                distance = (p_pos.x - obj_pos.x);
            //            }
            //            else if (p_pos.x < obj_pos.x) /*プレイヤーがオブジェクトの左側の時*/
            //            {
            //                Debug.Log("右側のオブジェクトにレイが当たっています");
            //                distance = (obj_pos.x - p_pos.x);
            //            }

            //            /*ポジションをめり込まないようにする処理*/
            //            if (right != 0 && distance <= 1.2f)
            //            {

            //                if (kuttuki_hit_wall == false)
            //                {

            //                    Debug.Log("くっつき専用フラグtrue");
            //                    kuttuki_hit_wall = true;
            //                    player_pos = new Vector3(obj_pos.x + 0.8f, transform.position.y, transform.position.z);
            //                    transform.position = player_pos;
            //                }
            //                //player_oldpos = this.transform.position;

            //            }
            //            else if (left != 0 && distance <= 1.2f)
            //            {
            //                //player_oldpos = this.transform.position;
            //                if (kuttuki_hit_wall == false)
            //                {
            //                    Debug.Log("くっつき専用フラグtrue");
            //                    kuttuki_hit_wall = true;
            //                    player_pos = new Vector3(obj_pos.x - 0.8f, transform.position.y, transform.position.z);
            //                    transform.position = player_pos;
            //                }

            //            }
            //        }
            //    }
            //    else
            //    {
            //        kuttuki_hit_wall = false;
            //    }
            //}
        }
    ///*壁にめり込まないようにする処理(自分版)*/
    //private void DontMove() /* 操作していない時勝手に動かないようにする処理 */
    //{
    //    if (kuttuku.bool_ray_hit == true && idle == true)
    //    {
    //        Debug.Log("勝手に動かない処理");
    //        transform.position = player_pos2;
    //    }
    //}

    /* 4/18 仲里追加 */
    public static IEnumerator MoveRestriction() /* 移動禁止化コルーチン */
    {
        banTaskCountMove++; /* コルーチン実行数増加 */
        if (!movePermit) { yield break; } /* コルーチンを複数起動させないため既に禁止状態なら抜ける */
        movePermit = false; /* 禁止化 */
        while (banTaskCountMove > 0) { yield return null; } /* 実行数が0になるまで待機 */
        movePermit = true; /* 制限解除 */
    }
    public static IEnumerator RotateRestriction() /* 方向転換禁止化コルーチン */
    {
        banTaskCountRotate++; /* コルーチン実行数増加 */
        if (!rotatePermit) { yield break; } /* コルーチンを複数起動させないため既に禁止状態なら抜ける */
        rotatePermit = false; /* 禁止化 */
        while (banTaskCountRotate > 0) { yield return null; } /* 実行数が0になるまで待機 */
        rotatePermit = true; /* 制限解除 */

    }
    public static void MoveRestrictionRelease() /* 禁止化解除 */
    {
        if (banTaskCountMove <= 0) { return; } /* 禁止化コルーチンを実行せず実行されると */
        banTaskCountMove--; /* 実行数削減 */
    }
    
    public static void RotateRestrictionRelease() /* 禁止化解除 */
    {
        if (banTaskCountRotate <= 0) { return; } /* 禁止化コルーチンを実行せず実行されると */
        banTaskCountRotate--; /* 実行数削減 */
    }
    /* 仲里追加以上 */
}
