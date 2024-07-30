using Cinemachine;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// プレーヤーのカメラの状態を保持するモデル
    /// </summary>
    public interface IPlayerCameraModel
    {
        CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; }

        CinemachineVirtualCamera SpawnTPSCamera(GameObject character);
        void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera);
    }
}