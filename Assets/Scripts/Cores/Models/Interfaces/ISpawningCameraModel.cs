using Cinemachine;
using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    public interface ISpawningCameraModel
    {
        CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; }

        Subject<CinemachineVirtualCamera> OnChangeVirtualCamera { get; }

        void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera);
    }
}