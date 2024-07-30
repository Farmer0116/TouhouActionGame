using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;
using Cinemachine;
using Utilities;
using ScriptableObjects.Camera;

namespace Components.Camera
{
    public class PlayerCameraInputRuntime : MonoBehaviour
    {
        public Transform CameraRotationTarget { get; private set; }
        public Transform Target { get; private set; }

        private PlayerCameraAsset _playerCameraAsset;

        private IInputSystemModel _inputSystemModel;
        private IPlayerCharacterModel _playerCharacterModel;
        private IPlayerCameraModel _playerCameraModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _cameraRotationTargetName = "CameraRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            IPlayerCharacterModel playerCharacterModel,
            IPlayerCameraModel playerCameraModel,
            PlayerCameraAsset playerCameraAsset
        )
        {
            _inputSystemModel = inputSystemModel;
            _playerCharacterModel = playerCharacterModel;
            _playerCameraAsset = playerCameraAsset;
            _playerCameraModel = playerCameraModel;
        }

        public void Initialize(CharacterCameraController characterCameraController)
        {
            CameraRotationTarget = new GameObject(_cameraRotationTargetName).transform;
            if (characterCameraController.CameraTarget != null) Target = characterCameraController.CameraTarget;
            else Debug.LogError("CharacterCameraControllerが設定されていません");

            // カメラの生成
            _playerCameraModel.SetCurrentCamera(CameraUtility.SpawnCharacterCamera(_playerCameraAsset.TPSCamera, CameraRotationTarget, CameraRotationTarget));
        }

        void Awake()
        {
            if (_inputSystemModel == null)
            {
                _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            }
        }

        void Start()
        {
            if (_zenAutoInjecter != null)
            {
                Destroy(_zenAutoInjecter);
                _zenAutoInjecter = null;
            }
        }

        void Update()
        {
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            Components.Character.PlayerCharacterInputs characterInputs = new Components.Character.PlayerCharacterInputs();

            if (_playerCharacterModel.IsLockOn.Value && _playerCharacterModel.LockOnTarget.Value != null)
            {
                CameraRotationTarget.localPosition = Target.position;
                CameraRotationTarget.LookAt(_playerCharacterModel.LockOnTarget.Value);
                characterInputs.Rotation = CameraRotationTarget.rotation;
            }
            else
            {
                // 回転
                CameraRotationTarget.localPosition = Target.position;
                _lookCharacterVector.y += _inputSystemModel.Look.Value.x;
                _lookCharacterVector.x += _inputSystemModel.Look.Value.y;
                _lookCharacterVector.x = Mathf.Clamp(_lookCharacterVector.x, _minViewField, _maxViewField);
                CameraRotationTarget.localRotation = Quaternion.Euler(new Vector3(_lookCharacterVector.x, _lookCharacterVector.y, 0));
            }
        }
    }
}