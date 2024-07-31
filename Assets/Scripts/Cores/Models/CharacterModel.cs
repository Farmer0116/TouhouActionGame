using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Utilities;
using Types.Character;
using Zenject;

namespace Cores.Models
{
    /// <summary>
    /// キャラクタの抽象モデル
    /// </summary>
    public abstract class CharacterModel : ICharacterModel
    {
        public CharacterModel
        (
            CharacterModelParam characterModelParam
        )
        {
            _characterModelParam = characterModelParam;
        }

        // パラメータ
        public CharacterModelParam CharacterModelParam { get { return _characterModelParam; } set { _characterModelParam = value; } }
        private CharacterModelParam _characterModelParam;
        // 行動パターン
        public bool IsNormalAttack { get; set; } = false;
        public bool IsMagicAttack { get; set; } = false;
        public bool IsLockOn { get => OnChangeIsLockOn.Value; set { OnChangeIsLockOn.Value = value; } }
        public Transform LockOnTarget { get; set; }
        // インスタンス
        public GameObject CharacterInstance { get { return _characterInstance; } set { _characterInstance = value; } }
        private GameObject _characterInstance = null;
        // イベント
        public ReactiveProperty<bool> OnChangeIsLockOn { get; private set; } = new ReactiveProperty<bool>(false);
        public Subject<GameObject> OnSpawnSubject => _onSpawnSubject;
        public Subject<GameObject> OnDespawnSubject => _onDespawnSubject;
        private Subject<GameObject> _onSpawnSubject = new Subject<GameObject>();
        private Subject<GameObject> _onDespawnSubject = new Subject<GameObject>();
        // Dispose
        public CompositeDisposable DespawnDisposables { get { return _despawnDisposables; } }
        private CompositeDisposable _despawnDisposables = new CompositeDisposable();

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
#if UNITY_EDITOR
            Debug.Log($"{_characterModelParam.Name}を{position}に{rotation}を向いて生成します");
#endif
            _characterInstance = CharacterUtility.SpawnCharacter(_characterModelParam.ControllerType, _characterModelParam.Model, position, rotation, this);
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