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
    public class EnemyCharacterSpawner : MonoBehaviour
    {
        [SerializeField] private CharacterNameType CharacterNameType = CharacterNameType.Reimu;

        // データ
        private CharacterParamAsset _characterParamAsset;
        // ファクトリー
        private ReimuModel.Factory _reimuFactory;
        // モデル
        private ICharacterModel _character;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct
        (
            CharacterParamAsset characterParamAsset,
            PlayerCharacterCameraAsset playerCharacterCameraAsset,
            ReimuModel.Factory factory
        )
        {
            _characterParamAsset = characterParamAsset;
            _reimuFactory = factory;
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

            // キャラクタールート生成
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
                ControllerType.Enemy
            ));
        }

        void Destroy()
        {
            if (_disposables != null) _disposables.Clear();
        }
    }
}