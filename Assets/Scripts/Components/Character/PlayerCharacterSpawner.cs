using System;
using Cinemachine;
using Cores.Models;
using Cores.Models.Interfaces;
using ScriptableObjects.Camera;
using ScriptableObjects.Character;
using Types.Character;
using UniRx;
using UnityEngine;
using Zenject;

namespace Components.Character
{
    public class PlayerCharacterSpawner : MonoBehaviour
    {
        [SerializeField] private CharacterNameType CharacterNameType = CharacterNameType.Reimu;

        // データ
        private CharacterParamAsset _characterParamAsset;
        private PlayerCharacterCameraAsset _playerCharacterCameraAsset;
        // ファクトリー
        private ReimuModel.Factory _reimuFactory;
        // モデル
        private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;
        private ISpawningCameraModel _spawningCameraModel;

        private ICharacterModel _character;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct
        (
            CharacterParamAsset characterParamAsset,
            PlayerCharacterCameraAsset playerCharacterCameraAsset,
            ReimuModel.Factory factory,
            ISpawningPlayerCharacterModel spawningPlayerCharacterModel,
            ISpawningCameraModel spawningCameraModel
        )
        {
            _characterParamAsset = characterParamAsset;
            _playerCharacterCameraAsset = playerCharacterCameraAsset;
            _reimuFactory = factory;
            _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
            _spawningCameraModel = spawningCameraModel;
        }

        void Start()
        {
            var characterParam = _characterParamAsset.CharacterParamList.Find(param => param.NameType == CharacterNameType);
            if (characterParam == null)
            {
                Debug.LogError("指定されたキャラクターのデータが存在しません");
                return;
            }

            switch (characterParam.NameType)
            {
                case CharacterNameType.Reimu:
                    _character = CreateReimuModel(characterParam);
                    break;
                default:
                    _character = CreateReimuModel(characterParam);
                    break;
            }

            // スポーン時にSpawingModelに追加
            _character.OnSpawnSubject.Subscribe(root =>
            {
                _spawningPlayerCharacterModel.Add(_character);
            }).AddTo(_disposables);

            // デスポーン時にSpawingModelから削除
            _character.OnDespawnSubject.Subscribe(root =>
            {
                // _spawningPlayerCharacterModel.Remove();
            }).AddTo(_disposables);

            // キャラクタールート生成
            _character.Spawn(
                transform.position,
                transform.rotation
            );

            // カメラの生成
            var playerCharacterInput = _character.CharacterInstance.GetComponent<PlayerCharacterInputRuntime>();
            if (playerCharacterInput == null)
            {
                Debug.LogError("キャラクターにPlayerCharacterInputRuntimeがアタッチされていません");
                return;
            }
            _spawningCameraModel.SetCurrentCamera(SpawnCharacterCamera(_playerCharacterCameraAsset, playerCharacterInput.CameraRotationTarget, playerCharacterInput.LookTarget));
        }

        private ICharacterModel CreateReimuModel(CharacterParam characterParam)
        {
            return _reimuFactory.Create(new CharacterModelParam(
                characterParam.Id,
                characterParam.NameType.ToString(),
                characterParam.Model,
                characterParam.Health,
                characterParam.Attack,
                ControllerType.Player
            ));
        }

        private CinemachineVirtualCamera SpawnCharacterCamera(PlayerCharacterCameraAsset playerCharacterCameraAsset, Transform rotateTarget, Transform lookTarget)
        {
            var camera = GameObject.Instantiate(playerCharacterCameraAsset.TPSCamera);
            camera.Follow = rotateTarget;
            camera.LookAt = lookTarget;
            return camera;
        }

        void Destroy()
        {
            if (_disposables != null) _disposables.Clear();
        }
    }
}