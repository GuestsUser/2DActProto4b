/*カメラのY座標を固定するスクリプト*/
using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraXFixed : CinemachineExtension
{
    [Header("ステージ端にプレイヤーが到達するとx座標を固定する")]
    [Tooltip("終端オブジェクト、格納順は始端終端問わない、どちらを先に入れてもいい")] [SerializeField] GameObject[] limitObj;
    [Tooltip("limitObjにこの距離分接近したらカメラX固定")] [SerializeField] float limitX = 8;
    [Tooltip("カメラ固定にはプレイヤーとlimitObjのz座標の違いがこの値内に収まってなければならない")] [SerializeField] float zoneZ = 2;

    private Transform player; /* プレイヤーはバーチャルカメラのfollowから受け取る */
    private List<float> fixedX; /* limitObjに接近した場合のx固定位置、limitObj[0]の左側固定位置、limitObj[1]の右側固定位置、limitObj[1]の左側～、という具合に格納されている */
    private void Start()
    {
        CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        player = vcam.Follow;

        fixedX = new List<float>(limitObj.Length * 2);
        for(int i = 0; i < limitObj.Length; i++)
        {
            for(int j = 0; j < 2; j++) { fixedX.Add(limitObj[i].transform.position.x - limitX * (1 - 2 * j)); } /* limitObjの中心位置からj=0なら左、j=1なら右の座標固定位置を割り出す */
        }

    }


    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,          /*cinemachineのvirtualcameraを使用する*/
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 camPos = state.RawPosition;
            for (int i = 0; i < limitObj.Length; i++)
            {
                float playerDiff = player.position.x - limitObj[i].transform.position.x; /* プレイヤーと固定位置との差分 */

                if (Mathf.Abs(player.position.z - limitObj[i].transform.position.z) <= zoneZ && Mathf.Abs(playerDiff) <= limitX && Mathf.Abs(limitObj[i].transform.position.x - camPos.x) <= limitX)
                {
                    camPos.x = fixedX[i * 2 + Convert.ToInt32(playerDiff > 0)];
                    state.RawPosition = camPos;
                    break;
                }
            }
        }
    }
}
