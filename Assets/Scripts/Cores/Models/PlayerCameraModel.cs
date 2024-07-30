using Cinemachine;
using Cores.Models.Interfaces;
using ScriptableObjects.Camera;
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

        public CinemachineVirtualCamera SpawnTPSCamera(GameObject character)
        {
            return CameraUtility.SpawnPlayerCharacterCamera(character, _playerCameraAsset.TPSCamera);
        }

        public void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
        {
            CurrentCinemachineVirtualCamera = cinemachineVirtualCamera;
        }
    }
}