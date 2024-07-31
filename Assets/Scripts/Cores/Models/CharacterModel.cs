using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Utilities;

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
            CharacterModelParam = characterModelParam;
        }

        // パラメータ
        public CharacterModelParam CharacterModelParam { get; set; }
        // 行動パターン
        public bool IsNormalAttack { get; set; } = false;
        public bool IsMagicAttack { get; set; } = false;
        public bool IsLockOn { get; private set; } = false;
        public Transform LockOnTarget { get; private set; } = null;
        // イベント
        public Subject<GameObject> OnSpawnSubject { get; private set; } = new Subject<GameObject>();
        public Subject<GameObject> OnDespawnSubject { get; private set; } = new Subject<GameObject>();
        public Subject<Transform> OnLockOn { get; private set; } = new Subject<Transform>();
        public Subject<Unit> OnUnLock { get; private set; } = new Subject<Unit>();
        // インスタンス
        public GameObject CharacterInstance { get; private set; }
        // Dispose
        public CompositeDisposable DespawnDisposables { get; private set; } = new CompositeDisposable();

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
#if UNITY_EDITOR
            Debug.Log($"{CharacterModelParam.Name}を{position}に{rotation}を向いて生成します");
#endif
            CharacterInstance = CharacterUtility.SpawnCharacter(CharacterModelParam.ControllerType, CharacterModelParam.Model, position, rotation, this);
            OnSpawnSubject.OnNext(CharacterInstance);
            return CharacterInstance;
        }

        public void Despawn()
        {
#if UNITY_EDITOR
            Debug.Log($"{CharacterModelParam.Name}を削除します");
#endif
            if (CharacterInstance != null)
            {
                OnDespawnSubject.OnNext(CharacterInstance);
                DespawnDisposables.Dispose();
                GameObject.Destroy(CharacterInstance);
                CharacterInstance = null;
            }
            else
            {
                Debug.LogError("モデルに対応したキャラクタが生成されていません");
            }
        }

        public void LockOn(Transform transform)
        {
            IsLockOn = true;
            LockOnTarget = transform;
            OnLockOn.OnNext(transform);
        }

        public void UnLock()
        {
            IsLockOn = false;
            LockOnTarget = null;
            OnUnLock.OnNext(new Unit());
        }
    }
}