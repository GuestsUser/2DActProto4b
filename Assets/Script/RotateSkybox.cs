using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    /* プレイヤーの移動状況を使用するためPlayerMoveを用意 */
    [SerializeField] private PlayerMove player;

    public float rot_speed = 1.2f; /* 基本的なskyboxの回転速度 */
    [SerializeField] private float state_speed; /* プレイヤーの移動によって変わるrot_speedにかける値 */
    
    private float idle_time;
    float count;
    float play_point;
    bool stop = false;

    void Start()
    {
        stop = false;
        count = 0;
    }
    void Update()
    {
        switch (player.state)
        {
            case State.walk:
                if(state_speed == 1.2f)
                {
                    count = count * 1.5f;
                }
                state_speed = 0.8f;
                if (stop == true)
                {
                    stop = false;
                }
                
                break;

            case State.run:
                if(state_speed == 0.8f)
                {
                    count = count * (0.8f/1.2f); 
                }
                state_speed = 1.2f;
                stop = false;
                break;

            case State.idle:
                if(stop == false)
                {
                    idle_time = count;
                    stop = true;
                }
                break;
        }
        
        if(player.state != State.idle)
        {
            if (player.right != 0)
            {
                count++;
            }
            else
            {
                count--;
            }
            RenderSettings.skybox.SetFloat("_Rotation", count * rot_speed * (state_speed/10));
        }
        else
        {
            play_point = count * rot_speed *(state_speed / 10);
            RenderSettings.skybox.SetFloat("_Rotation", play_point);
        }
    }
}
