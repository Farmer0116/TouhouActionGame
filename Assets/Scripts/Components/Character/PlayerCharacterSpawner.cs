using Cores.Models;
using Cores.Models.Interfaces;
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
        // ファクトリー
        private ReimuModel.Factory _reimuFactory;
        // モデル
        private IPlayerCharacterModel _playerCharacterModel;
        private IPlayerCameraModel _playerCameraModel;

        private ICharacterModel _character;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct
        (
            CharacterParamAsset characterParamAsset,
            ReimuModel.Factory factory,
            IPlayerCharacterModel playerCharacterModel,
            IPlayerCameraModel playerCameraModel
        )
        {
            _characterParamAsset = characterParamAsset;
            _reimuFactory = factory;
            _playerCharacterModel = playerCharacterModel;
            _playerCameraModel = playerCameraModel;
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

            // スポーンイベント
            _character.OnSpawnSubject.Subscribe(root =>
            {
                _playerCharacterModel.CharacterModel = _character;
                var vc = _playerCameraModel.SpawnTPSCamera(root);
                _playerCameraModel.SetCurrentCamera(vc);
            }).AddTo(_disposables);

            // デスポーンイベント
            _character.OnDespawnSubject.Subscribe(root =>
            {
                // _playerCharacterModel.Remove();
            }).AddTo(_disposables);

            // キャラクター生成
            _character.Spawn(
                transform.position,
                transform.rotation
            );
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

        void Destroy()
        {
            if (_disposables != null) _disposables.Clear();
        }
    }
}