using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;
using Cinemachine;
using Utilities;
using ScriptableObjects.Camera;

namespace Components.Camera
{
    public class PlayerCharacterCameraInputRuntime : MonoBehaviour
    {
        public Transform CameraRotationTarget { get; private set; }
        public Transform Target { get; private set; }

        private PlayerCharacterCameraAsset _playerCharacterCameraAsset;

        private IInputSystemModel _inputSystemModel;
        private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;
        private ISpawningCameraModel _spawningCameraModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _cameraRotationTargetName = "CameraRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            ISpawningPlayerCharacterModel spawningPlayerCharacterModel,
            ISpawningCameraModel spawningCameraModel,
                        PlayerCharacterCameraAsset playerCharacterCameraAsset
        )
        {
            _inputSystemModel = inputSystemModel;
            _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
            _playerCharacterCameraAsset = playerCharacterCameraAsset;
            _spawningCameraModel = spawningCameraModel;
        }

        public void Initialize(CharacterCameraController characterCameraController)
        {
            CameraRotationTarget = new GameObject(_cameraRotationTargetName).transform;
            if (characterCameraController.CameraTarget != null) Target = characterCameraController.CameraTarget;
            else Debug.LogError("CharacterCameraControllerが設定されていません");

            // カメラの生成
            _spawningCameraModel.SetCurrentCamera(CameraUtility.SpawnCharacterCamera(_playerCharacterCameraAsset.TPSCamera, CameraRotationTarget, CameraRotationTarget));
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

            if (_spawningPlayerCharacterModel.IsLockOn.Value && _spawningPlayerCharacterModel.LockOnTarget.Value != null)
            {
                CameraRotationTarget.localPosition = Target.position;
                CameraRotationTarget.LookAt(_spawningPlayerCharacterModel.LockOnTarget.Value);
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