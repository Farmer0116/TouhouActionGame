using Cinemachine;
using Cores.Models.Interfaces;
using ScriptableObjects.Camera;
using UnityEngine;
using Utilities;

namespace Cores.Models
{
    public class PlayerCameraModel : IPlayerCameraModel
    {
        public PlayerCameraType CurrentCameraType { get; private set; }
        public CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; private set; }
        public CinemachineVirtualCamera TPSCamera { get; private set; }
        public CinemachineVirtualCamera TPSLockOnCamera { get; private set; }

        private PlayerCameraAsset _playerCameraAsset;

        public PlayerCameraModel
        (
            PlayerCameraAsset playerCameraAsset
        )
        {
            _playerCameraAsset = playerCameraAsset;
        }

        public void SwitchCamera(PlayerCameraType playerCameraType)
        {
            CurrentCameraType = playerCameraType;
            switch (playerCameraType)
            {
                case PlayerCameraType.TPS:
                    CurrentCinemachineVirtualCamera = TPSCamera;
                    TPSCamera.Priority = 1;
                    TPSLockOnCamera.Priority = 0;
                    break;
                case PlayerCameraType.TPSLockOn:
                    CurrentCinemachineVirtualCamera = TPSLockOnCamera;
                    TPSCamera.Priority = 0;
                    TPSLockOnCamera.Priority = 1;
                    break;
            }
        }

        public (CinemachineVirtualCamera tps, CinemachineVirtualCamera tpsLockOn) SpawnAllCameras(GameObject character)
        {
            if (TPSCamera != null || TPSLockOnCamera != null)
            {
                Debug.LogError("カメラはすでに生成されています");
                return (TPSCamera, TPSLockOnCamera);
            }
            var cameras = CameraUtility.SpawnALLCameras(character, _playerCameraAsset.TPSCamera, _playerCameraAsset.TPSLockOnCamera);
            TPSCamera = cameras.tps;
            TPSLockOnCamera = cameras.tpsLockOn;

            return cameras;
        }

        public void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
        {
            CurrentCinemachineVirtualCamera = cinemachineVirtualCamera;
        }
    }
}