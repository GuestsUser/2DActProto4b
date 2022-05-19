/*カメラのY座標を固定するスクリプト*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Cinemachine;

public class CameraXFixed : CinemachineExtension
{
    [Header("ステージ端にプレイヤーが到達するとx座標を固定する")]
    [Tooltip("終端オブジェクト、格納順は始端終端問わない、どちらを先に入れてもいい")] [SerializeField] GameObject[] limitObj;
    [Tooltip("limitObjにこの距離分接近したらカメラX固定")] [SerializeField] float limitX = 8;
    [Tooltip("カメラ固定にはプレイヤーとlimitObjのz座標の違いがこの値内に収まってなければならない")] [SerializeField] float zoneZ = 2;

    [HideInInspector] [SerializeField] private Transform player; /* プレイヤーはバーチャルカメラのfollowから受け取る */
    [HideInInspector] [SerializeField] private CinemachineVirtualCamera vcam;
    private Vector3 adjust; /* カメラ補正値取得 */
    private bool run = false; /* 既に固定実行中かどうか判定 */


    private Camera cam;
    
    private List<float> fixedX; /* limitObjに接近した場合のx固定位置、limitObj[0]の左側固定位置、limitObj[1]の右側固定位置、limitObj[1]の左側～、という具合に格納されている */


    
    private Vector3 fixedPos = Vector3.zero; /* カメラ固定位置 */
   


    private void Reset()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        player = vcam.Follow; /* プレイヤー取得 */
    }

    private void Start()
    {
        CinemachineTransposer body = vcam.GetCinemachineComponent<CinemachineTransposer>();
        adjust = body.m_FollowOffset; /* カメラ補正値取得 */
        cam = GetComponent<Camera>();

        fixedX = new List<float>(limitObj.Length * 2);
        for(int i = 0; i < limitObj.Length; i++)
        {
            for(int j = 0; j < 2; j++) { fixedX.Add(limitObj[i].transform.position.x - limitX * (1 - 2 * j) + adjust.x); } /* limitObjの中心位置からj=0なら左、j=1なら右の座標固定位置を割り出す */
        }

    }


    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,          /*cinemachineのvirtualcameraを使用する*/
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            
            Vector3 camPos = state.RawPosition;

            if (run)
            {
                camPos.x = fixedPos.x;
                //camPos.y = player.position.y+adjust.y;
                state.RawPosition = camPos;
                return;
            }

            for (int i = 0; i < limitObj.Length; i++)
            {
                Vector3 playerDiff = player.position - limitObj[i].transform.position; /* プレイヤーと固定位置との差分 */

                if (Mathf.Abs(playerDiff.z) <= zoneZ && Mathf.Abs(playerDiff.x) <= limitX && Mathf.Abs(limitObj[i].transform.position.x - camPos.x) <= limitX)
                {
                    fixedPos = camPos;
                    StartCoroutine(FixedHold(limitObj[i].transform));
                    break;
                }
            }
        }
    }

    IEnumerator FixedHold(Transform obj)
    {
        //vcam.Follow = null;
        run = true;
        while (true)
        {
            yield return StartCoroutine(TimeScaleYield.TimeStop());

            Vector3 diff = player.position - obj.position;
            float camDiff = Mathf.Abs(transform.position.x - obj.position.x);
            if(Mathf.Abs(diff.x) > limitX || Mathf.Abs(diff.z) > zoneZ || camDiff > limitX) { break; } /* 後置条件、固定x位置をプレイヤーが超える又は固定z位置を超える事で固定解除 */
        }
        //vcam.Follow = player;
        run = false;
    }
}
