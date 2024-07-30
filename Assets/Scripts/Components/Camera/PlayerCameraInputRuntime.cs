using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;
using UniRx;

namespace Components.Camera
{
    public class PlayerCameraInputRuntime : MonoBehaviour
    {
        public Transform TPSCameraTarget { get; private set; }
        public Transform TPSLockOnCameraTarget { get; private set; }

        private IInputSystemModel _inputSystemModel;
        private IPlayerCharacterModel _playerCharacterModel;
        private IPlayerCameraModel _playerCameraModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;
        private Transform _cameraOffset;

        private const string _tpsCameraTargetName = "TPSCameraTarget";
        private const string _tpsLockOnCameraTargetName = "TPSLockOnCameraTarget";

        private CompositeDisposable disposables = new CompositeDisposable();

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            IPlayerCharacterModel playerCharacterModel,
            IPlayerCameraModel playerCameraModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _playerCharacterModel = playerCharacterModel;
            _playerCameraModel = playerCameraModel;
        }

        public void Initialize(CharacterCameraController characterCameraController)
        {
            if (characterCameraController.CameraOffset == null) Debug.LogError("CharacterCameraControllerが設定されていません");
            else _cameraOffset = characterCameraController.CameraOffset;
        }

        void Awake()
        {
            if (_inputSystemModel == null)
            {
                _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            }

            TPSCameraTarget = new GameObject(_tpsCameraTargetName).transform;
            TPSLockOnCameraTarget = new GameObject(_tpsLockOnCameraTargetName).transform;
        }

        void Start()
        {
            if (_zenAutoInjecter != null)
            {
                Destroy(_zenAutoInjecter);
                _zenAutoInjecter = null;
            }

            // ロックオン
            _playerCharacterModel.OnChangeIsLockOn.Subscribe(value =>
            {
                if (value) _playerCameraModel.SwitchCamera(PlayerCameraType.TPSLockOn);
                else _playerCameraModel.SwitchCamera(PlayerCameraType.TPS);
            }).AddTo(disposables);
        }

        void Update()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // TPS
            TPSCameraTarget.position = _cameraOffset.position;
            TPSCameraTarget.rotation = _playerCharacterModel.CharacterRotation;

            // TPSLockOn
            TPSLockOnCameraTarget.localPosition = _cameraOffset.position;
            TPSLockOnCameraTarget.LookAt(_playerCharacterModel.LockOnTarget);
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}