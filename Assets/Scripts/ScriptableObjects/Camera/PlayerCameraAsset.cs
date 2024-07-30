using Cinemachine;
using UnityEngine;

namespace ScriptableObjects.Camera
{
    [CreateAssetMenu(fileName = "PlayerCameraAsset", menuName = "ScriptableObjects/CreatePlayerCameraAsset")]
    public class PlayerCameraAsset : ScriptableObject
    {
        [SerializeField] public CinemachineVirtualCamera TPSCamera;
        [SerializeField] public CinemachineVirtualCamera TPSLockOnCamera;
    }
}