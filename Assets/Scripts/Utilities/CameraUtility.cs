using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public static class CameraUtility
    {
        public static CinemachineVirtualCamera SpawnCharacterCamera(CinemachineVirtualCamera virtualCamera, Transform rotateTarget, Transform lookTarget)
        {
            var camera = GameObject.Instantiate(virtualCamera);
            camera.Follow = rotateTarget;
            camera.LookAt = lookTarget;
            return camera;
        }
    }
}
