using Cinemachine;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    public interface IPlayerCameraModel
    {
        CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; }

        CinemachineVirtualCamera SpawnTPSCamera(Transform rotationTarget, Transform lookTarget);
        void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera);
    }
}