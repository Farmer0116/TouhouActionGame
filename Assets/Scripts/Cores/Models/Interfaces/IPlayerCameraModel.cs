using Cinemachine;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// プレーヤーのカメラの状態を保持するモデル
    /// </summary>
    public interface IPlayerCameraModel
    {
        PlayerCameraType CurrentCameraType { get; }
        CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; }

        CinemachineVirtualCamera TPSCamera { get; }
        CinemachineVirtualCamera TPSLockOnCamera { get; }

        void SwitchCamera(PlayerCameraType playerCameraType);
        (CinemachineVirtualCamera tps, CinemachineVirtualCamera tpsLockOn) SpawnAllCameras(GameObject character);
        void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera);
    }

    public enum PlayerCameraType
    {
        TPS,
        TPSLockOn
    }
}