using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;
using Utilities;

namespace Cores.Models
{
    /// <summary>
    /// キャラクタに関するモデル
    /// </summary>
    public class CharacterModel : ICharacterModel
    {
        public class Factory : PlaceholderFactory<CharacterModelParam, CharacterModel> { }

        public CharacterModel
        (
            CharacterModelParam characterModelParam
        )
        {
            _characterModelParam = characterModelParam;
        }

        // 初期値
        public CharacterModelParam CharacterModelParam { get { return _characterModelParam; } set { _characterModelParam = value; } }

        private CharacterModelParam _characterModelParam;

        // その他
        public GameObject CharacterInstance { get { return _characterInstance; } set { _characterInstance = value; } }
        public CompositeDisposable DespawnDisposables { get { return _despawnDisposables; } }

        private GameObject _characterInstance = null;
        private CompositeDisposable _despawnDisposables = new CompositeDisposable();

        // 機能
        public Subject<GameObject> OnSpawnSubject => _onSpawnSubject;
        public Subject<GameObject> OnDespawnSubject => _onDespawnSubject;

        private Subject<GameObject> _onSpawnSubject = new Subject<GameObject>();
        private Subject<GameObject> _onDespawnSubject = new Subject<GameObject>();

        public GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale)
        {
#if UNITY_EDITOR
            Debug.Log($"{_characterModelParam.Name}を{position}に{rotation}を向いて{scale}のサイズで生成します");
#endif
            _characterInstance = CharacterUtility.SpawnCharacter(_characterModelParam.ControllerType, _characterModelParam.Model, position, rotation);
            // _characterInstance = await Spawn(id, position, rotation, scale);
            // _characterInstance.AddComponent<AudioSource>();
            OnSpawnSubject.OnNext(_characterInstance);
            return _characterInstance;
        }

        public void Despawn()
        {
#if UNITY_EDITOR
            Debug.Log($"{_characterModelParam.Name}を削除します");
#endif
            if (_characterInstance != null)
            {
                OnDespawnSubject.OnNext(_characterInstance);
                DespawnDisposables.Dispose();
                GameObject.Destroy(_characterInstance);
                _characterInstance = null;
            }
            else
            {
                Debug.LogError("モデルに対応したキャラクタが生成されていません");
            }
        }
    }
}