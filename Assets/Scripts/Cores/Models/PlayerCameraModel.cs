using Cinemachine;
using Cores.Models.Interfaces;
using ScriptableObjects.Camera;
using UniRx;
using UnityEngine;
using Utilities;

namespace Cores.Models
{
    public class PlayerCameraModel : IPlayerCameraModel
    {
        public CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; private set; }

        private PlayerCameraAsset _playerCameraAsset;

        public PlayerCameraModel
        (
            PlayerCameraAsset playerCameraAsset
        )
        {
            _playerCameraAsset = playerCameraAsset;
        }

        public CinemachineVirtualCamera SpawnTPSCamera(Transform rotationTarget, Transform lookTarget)
        {
            return CameraUtility.SpawnCharacterCamera(_playerCameraAsset.TPSCamera, rotationTarget, lookTarget);
        }

        public void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
        {
            CurrentCinemachineVirtualCamera = cinemachineVirtualCamera;
        }
    }
}