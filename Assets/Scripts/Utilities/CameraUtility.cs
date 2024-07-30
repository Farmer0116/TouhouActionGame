using Cinemachine;
using Components.Camera;
using UnityEngine;

namespace Utilities
{
    public static class CameraUtility
    {
        public static CinemachineVirtualCamera SpawnPlayerCharacterCamera(GameObject character, CinemachineVirtualCamera virtualCamera)
        {
            var camera = GameObject.Instantiate(virtualCamera);

            var characterCameraController = character.GetComponent<CharacterCameraController>();

            var playerCameraInputRuntime = character.AddComponent<PlayerCameraInputRuntime>();
            playerCameraInputRuntime.Initialize(characterCameraController);

            camera.Follow = playerCameraInputRuntime.CameraRotationTarget;
            camera.LookAt = playerCameraInputRuntime.CameraRotationTarget;
            return camera;
        }
    }
}
