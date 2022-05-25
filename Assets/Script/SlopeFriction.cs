using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeFriction : MonoBehaviour
{
    [Header("滑る床から落とされた時の力を抑えるスクリプト")]
    [Tooltip("滑る床から落とされた時、吹っ飛びの力が消えるまでの時間(正確には吹っ飛びにvelocityは使用してないが……)")] [SerializeField] float frictionTime = 0.1f;
    [Tooltip("吹っ飛ばされた時の移動量がこの値以下の時はこの値に補正される)")] [SerializeField] float velocityBorder = 0.5f;
    private Rigidbody rb; /* リジットボディ保持 */
    private PlayerMove pm; /* 移動用コンポーネント保持 */

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /* 床が消えると困るのでこのコルーチンは別の場所に移すべきかも */
    public IEnumerator CourseOutFriction(float x_force, float y_force, Vector3 move) /* 床を抜けると凄まじい力が掛かるのでオリジナル摩擦 */
    {
        StartCoroutine(JumpSystem.Restriction()); /* ジャンプ禁止化 */
        StartCoroutine(PlayerMove.MoveRestriction());
        rb.velocity = Vector3.zero;
        float count = 0; /* 実行された時、1瞬プレイヤーの動きが止まるようなら初期値にTime.deltaTimeを設定し動かしてみる */

        /* 各軸の摩擦コルーチン実行状況 */
        bool x_run = false;
        bool y_run = false;

        Vector3 pos = Vector3.zero;
        float power = 0;

        RaycastHit hit;
        Vector3 direction = move.normalized; /* 方向化 */
        Vector3 xDirection = direction.x >= 0 ? Vector3.right : Vector3.left;
        Vector3 yDirection = direction.y >= 0 ? Vector3.up : Vector3.down;

        /* 滑る力を掛けられていた方向と同じ方向に動いていた(押し戻された若しくは駆け下りていた)場合摩擦コルーチンを開始(力が掛かっていなかった場合開始しない) */
        if (x_force != 0 && (move.x > 0) == (x_force > 0)) { x_run = true; }
        if (y_force != 0 && (move.y > 0) == (y_force > 0)) { y_run = true; }
        if ( !(x_run||y_run))
        {
            EndProcess();
            yield break;
        }
        if (move.x + move.y < velocityBorder)
        {
            float total = Mathf.Abs(move.x) + Mathf.Abs(move.y);
            float[] ration_xy = { 0,0 };
            if (total == 0)
            {
                EndProcess();
                yield break;
                total = Mathf.Abs(x_force) + Mathf.Abs(y_force);
                ration_xy[0] = x_force / total;
                ration_xy[1] = y_force / total;
            }
            else
            {
                ration_xy[0] = move.x / total;
                ration_xy[1] = move.y / total;
            }
            move.x = velocityBorder * ration_xy[0];
            move.y = velocityBorder * ration_xy[1];
        }


        /* 終了条件は他にもプレイヤーのダッシュやノックバック、死亡、他の移動床に乗った場合 */
        while (count<frictionTime)
        {
            power = 1 - Mathf.Sin(90 / frictionTime * count * Mathf.Deg2Rad); /* 今回の移動量に掛ける値、countがfrictionTimeに近づくとこの値も1から0に近づいてゆき、ブレーキ表現になる */
            pos = transform.position;
            if (x_run)
            {
                float velocity= move.x * power;
                if (Physics.Raycast(pos, xDirection, out hit, Mathf.Abs(velocity))) /* 他colliderと接触があった場合 */
                {
                    pos.x = hit.point.x; /* 移動量を衝突点までに抑える */
                    x_run = false; /* 次回移動移動しない */
                }
                else { pos.x += velocity; }
                
            }
            if (y_run) 
            {
                Vector3 area = pos;
                area.y += 0.2f; /* 足元そのままだと床と始点が一致し列挙されないので少し上げる */
                float velocity = move.y * power;
                if (Physics.Raycast(area, yDirection, out hit, Mathf.Abs(velocity))) /* 他colliderと接触があった場合 */
                {
                    pos.y = hit.point.y; /* 移動量を衝突点までに抑える */
                    y_run = false; /* 次回移動移動しない */
                }
                else { pos.y += velocity; }
                
            }

            transform.position = pos;
            count += Time.deltaTime;
            yield return StartCoroutine(TimeScaleYield.TimeStop());
            
        }
        EndProcess();

        void EndProcess()
        {
            rb.velocity /= 5; /* 斜めにぶつかったりした場合の跳ね返りの軽減 */
            JumpSystem.RestrictionRelease(); /* ジャンプ禁止化解除 */
            PlayerMove.MoveRestrictionRelease();
            //PlayerMove.RotateRestrictionRelease();
        }
    }
}
