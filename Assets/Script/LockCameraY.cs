/*カメラのY座標を固定するスクリプト*/
using UnityEngine;
using Cinemachine;

[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class LockCameraY : CinemachineExtension
{
    [Tooltip("カメラのY座標を固定する値")]
    public float m_YPosition = 10;      

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,          /*cinemachineのvirtualcameraを使用する*/
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            var pos = state.RawPosition;
            pos.y = m_YPosition;        /*Y座標をインスペクター欄で設定した値に固定*/
            state.RawPosition = pos;
        }
    }
}
