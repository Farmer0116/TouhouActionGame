using Cinemachine;
using UnityEngine;

namespace ScriptableObjects.Camera
{
    [CreateAssetMenu(fileName = "PlayerCharacterCameraAsset", menuName = "ScriptableObjects/CreatePlayerCharacterCameraAsset")]
    public class PlayerCharacterCameraAsset : ScriptableObject
    {
        [SerializeField] public CinemachineVirtualCamera TPSCamera;
    }
}