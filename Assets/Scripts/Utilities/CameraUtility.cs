using Cinemachine;
using Components.Camera;
using UnityEngine;

namespace Utilities
{
    public static class CameraUtility
    {
        public static (CinemachineVirtualCamera tps, CinemachineVirtualCamera tpsLockOn) SpawnALLCameras(GameObject character, CinemachineVirtualCamera tpsCamera, CinemachineVirtualCamera tpsLockOnCamera)
        {
            var tps = GameObject.Instantiate(tpsCamera);
            var tpsLockOn = GameObject.Instantiate(tpsLockOnCamera);

            var characterCameraController = character.GetComponent<CharacterCameraController>();

            var playerCameraInputRuntime = character.AddComponent<PlayerCameraInputRuntime>();
            playerCameraInputRuntime.Initialize(characterCameraController);

            tps.Follow = playerCameraInputRuntime.TPSCameraTarget;
            tps.LookAt = playerCameraInputRuntime.TPSCameraTarget;

            tpsLockOn.Follow = playerCameraInputRuntime.TPSLockOnCameraTarget;
            tpsLockOn.LookAt = playerCameraInputRuntime.TPSLockOnCameraTarget;
            return (tps, tpsLockOn);
        }
    }
}
