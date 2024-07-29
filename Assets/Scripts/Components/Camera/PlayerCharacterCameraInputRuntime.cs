using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;

namespace Components.Camera
{
    public class PlayerCharacterCameraInputRuntime : MonoBehaviour
    {
        public Transform CameraRotationTarget { get; private set; }
        public Transform Target { get; private set; }

        private IInputSystemModel _inputSystemModel;
        private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _cameraRotationTargetName = "CameraRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            ISpawningPlayerCharacterModel spawningPlayerCharacterModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
        }

        public void Initialize(CharacterCameraController characterCameraController)
        {
            CameraRotationTarget = new GameObject(_cameraRotationTargetName).transform;
            if (characterCameraController.CameraTarget != null) Target = characterCameraController.CameraTarget;
            else Debug.LogError("CharacterCameraControllerが設定されていません");
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
                if (_spawningPlayerCharacterModel.OrientationMethod == OrientationMethod.TowardsCamera)
                {
                    CameraRotationTarget.localPosition = Target.position;
                    CameraRotationTarget.LookAt(_spawningPlayerCharacterModel.LockOnTarget.Value);
                    characterInputs.Rotation = CameraRotationTarget.rotation;
                }
                else
                {
                    CameraRotationTarget.localPosition = Target.position;
                    _lookCharacterVector.y += _inputSystemModel.Look.Value.x;
                    _lookCharacterVector.x += _inputSystemModel.Look.Value.y;
                    _lookCharacterVector.x = Mathf.Clamp(_lookCharacterVector.x, _minViewField, _maxViewField);
                    CameraRotationTarget.localRotation = Quaternion.Euler(new Vector3(_lookCharacterVector.x, _lookCharacterVector.y, 0));
                }
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