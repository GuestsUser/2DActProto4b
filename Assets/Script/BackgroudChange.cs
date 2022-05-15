using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroudChange : MonoBehaviour
{
    [Header("背景がフェードで切り替わるスクリプト")]
    /* 背景マテリアル */
    [Tooltip("Stage1の背景マテリアル")] [SerializeField] private Material stage1_Back;
    [Tooltip("Stage2の背景マテリアル")] [SerializeField] private Material stage2_Back;
    [Tooltip("Stage3の背景マテリアル")] [SerializeField] private Material stage3_Back;

    /* 透明度 */
    [Tooltip("Stage1の背景透明度")] [SerializeField, Range(0, 1)] private float opacity1;
    [Tooltip("Stage1の背景透明度")] [SerializeField, Range(0, 1)] private float opacity2;
    [Tooltip("Stage1の背景透明度")] [SerializeField, Range(0, 1)] private float opacity3;

    /* 背景切り替えポイント */
    [Tooltip("Stage1に近い方の座標")] [SerializeField] private GameObject changepoint1_1;
    [Tooltip("Stage2に近い方の座標")] [SerializeField] private GameObject changepoint1_2;
    [Tooltip("Stage2に近い方の座標２")] [SerializeField] private GameObject changepoint2_1;
    [Tooltip("Stage3に近い方の座標")] [SerializeField] private GameObject changepoint2_2;

    /* プレイヤー */
    [Tooltip("Playerオブジェクト")] [SerializeField] private GameObject player;

    /* プレイヤーとStageオブジェクトとの距離 */
    [Tooltip("changepoint1_1とのdistance")] [SerializeField] private float p_to_1;
    [Tooltip("changepoint1_2とのdistance")] [SerializeField] private float p_to_2_1;
    [Tooltip("changepoint2_1とのdistance")] [SerializeField] private float p_to_2_2;
    [Tooltip("changepoint2_2とのdistance")] [SerializeField] private float p_to_3;

    /* Stageオブジェクト間の距離 */
    [Tooltip("Stage1とStage2のdistance")] [SerializeField] private float one_to_two;
    [Tooltip("Stage2とStage3のdistance")] [SerializeField] private float two_to_three;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Haruko");

        /* Stageオブジェクトは動かないのでStartで計算しておく */
        //one_to_two = Mathf.Abs(Vector3.Distance(changepoint1_1.transform.position, changepoint1_2.transform.position));
        //two_to_three = Mathf.Abs(Vector3.Distance(changepoint2_1.transform.position, changepoint2_2.transform.position));
        one_to_two = Xdistance(changepoint1_1, changepoint1_2);
        two_to_three = Xdistance(changepoint2_1, changepoint2_2);


        /* 透明度の初期化 */
        opacity1 = 1; /* playerの初期位置によりstage1は先に表示 */
        opacity2 = 0;
        opacity3 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /* Playerと各ステージオブジェクトとの距離を計算 */
        distance();

        /* 背景マテリアルの透明度のフェード */
        ChangeAlpha();

        stage1_Back.color = new Color(1, 1, 1, opacity1);
        stage2_Back.color = new Color(1, 1, 1, opacity2);
        stage3_Back.color = new Color(1, 1, 1, opacity3);
    }

    void distance()/* Playerと各ステージオブジェクトとの距離を計算 */
    {
        /* Stage1～2 */
        p_to_1 = Xdistance(player, changepoint1_1);
        p_to_2_1 = Xdistance(player, changepoint1_2);

        /* Stage2～3 */
        p_to_2_2 = Xdistance(player, changepoint2_1);
        p_to_3 = Xdistance(player, changepoint2_2);
    }

    void ChangeAlpha()
    {
        /* 各x座標の値を取得 */
        var p_pos_x = player.transform.position.x; 

        var stage1_pos_x = changepoint1_1.transform.position.x;
        var stage2_pos_x_1 = changepoint1_2.transform.position.x;

        var stage2_pos_x_2 = changepoint2_1.transform.position.x;
        var stage3_pos_x = changepoint2_2.transform.position.x;

        /* 入力方向を取得 */
        var right = player.GetComponent<PlayerMove>().right;
        var left = player.GetComponent<PlayerMove>().left;

        /* stage1とstage2の間の時 */
        if (p_pos_x >= stage1_pos_x && p_pos_x <= stage2_pos_x_1)
        {
            
            opacity3 = 0; /* 2～3の背景にフェードさせたいので0に設定 */
            
            if (p_to_1 < one_to_two)
            {
                opacity1 = 1 - (p_to_1 / (one_to_two));
                opacity2 = 1 - opacity1;
            }
        }
        /* stage2とstage3の間の時 */
        else if (p_pos_x >= stage2_pos_x_2 && p_pos_x <= stage3_pos_x) 
        {
            opacity1 = 0; /* 1～2の背景にフェードさせたいので0に設定 */

            if (p_to_2_2 < two_to_three)
            {
                opacity2 = 1 - (p_to_2_2 / (two_to_three));
                opacity3 = 1 - opacity2;
            }
        }
    }
    
    
    float Xdistance(GameObject pos1, GameObject pos2) /* 2つのオブジェクト間(X軸)の距離を正の値で返してくれる関数 */
    {
        var x_pos1 = pos1.transform.position.x;
        var x_pos2 = pos2.transform.position.x;

        var distance = 0f; /* 初期化 */

        if (x_pos1 > x_pos2)
        {
            distance = x_pos1 - x_pos2;
        }
        else
        {
            distance = x_pos2 - x_pos1;
        }
        return distance;
    }
}
