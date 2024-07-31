using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;
using UniRx;
using Components.Character;

namespace Components.Camera
{
    public class PlayerCameraInputRuntime : MonoBehaviour
    {
        public Transform TPSCameraTarget { get; private set; }
        public Transform TPSLockOnCameraTarget { get; private set; }

        private IPlayerCameraModel _playerCameraModel1;
        private CharacterModelComponent _characterModelComponent;
        private ZenAutoInjecter _zenAutoInjecter;

        private const string _tpsCameraTargetName = "TPSCameraTarget";
        private const string _tpsLockOnCameraTargetName = "TPSLockOnCameraTarget";

        private CompositeDisposable disposables = new CompositeDisposable();

        [Inject]
        private void construct(
            IPlayerCameraModel playerCameraModel1
        )
        {
            _playerCameraModel1 = playerCameraModel1;
        }

        public void Init(CharacterModelComponent characterModelComponent)
        {
            _characterModelComponent = characterModelComponent;
        }

        void Awake()
        {
            if (_playerCameraModel1 == null)
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
            _characterModelComponent.CharacterModel.OnLockOn.Subscribe(value =>
            {
                _playerCameraModel1.SwitchCamera(PlayerCameraType.TPSLockOn);
            }).AddTo(disposables);

            // アンロック
            _characterModelComponent.CharacterModel.OnUnLock.Subscribe(value =>
            {
                _playerCameraModel1.SwitchCamera(PlayerCameraType.TPS);
            }).AddTo(disposables);
        }

        void Update()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // TPS
            TPSCameraTarget.position = _characterModelComponent.Center.position;
            if (!_characterModelComponent.CharacterModel.IsLockOn)
            {
                TPSCameraTarget.rotation = _characterModelComponent.CharacterRotation;
            }

            // TPSLockOn
            TPSLockOnCameraTarget.position = _characterModelComponent.Center.position;
            TPSLockOnCameraTarget.LookAt(_characterModelComponent.CharacterModel.LockOnTarget);
        }

        void OnDestroy()
        {
            disposables.Dispose();
        }
    }
}