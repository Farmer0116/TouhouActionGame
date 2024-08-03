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

        // 状態
        public bool IsNormalAttack { get; set; } = false;
        public bool IsMagicAttack { get; set; } = false;
        public bool IsLockOn { get; private set; } = false;
        public Transform LockOnTarget { get; private set; } = null;
        public bool IsFlight { get; private set; } = false;

        // イベント
        public Subject<GameObject> OnSpawn { get; private set; } = new Subject<GameObject>();
        public Subject<GameObject> OnDespawn { get; private set; } = new Subject<GameObject>();
        public Subject<Transform> OnLockOn { get; private set; } = new Subject<Transform>();
        public Subject<Unit> OnUnLock { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnEnableFlight { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnDisableFlight { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnJump { get; private set; } = new Subject<Unit>();
        public Subject<Unit> OnAscend { get; } = new Subject<Unit>();
        public Subject<Unit> OnDescend { get; private set; } = new Subject<Unit>();

        // インスタンス
        public GameObject CharacterInstance { get; private set; }

        // Dispose
        public CompositeDisposable DespawnDisposables { get; private set; } = new CompositeDisposable();

        // 関数
        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
#if UNITY_EDITOR
            Debug.Log($"{CharacterModelParam.Name}を{position}に{rotation}を向いて生成します");
#endif
            CharacterInstance = CharacterUtility.SpawnCharacter(CharacterModelParam.ControllerType, CharacterModelParam.Model, position, rotation, this);
            OnSpawn.OnNext(CharacterInstance);
            return CharacterInstance;
        }

        public void Despawn()
        {
#if UNITY_EDITOR
            Debug.Log($"{CharacterModelParam.Name}を削除します");
#endif
            if (CharacterInstance != null)
            {
                OnDespawn.OnNext(CharacterInstance);
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

        public void EnableFlight()
        {
            IsFlight = true;
            OnEnableFlight.OnNext(new Unit());
        }

        public void DisableFlight()
        {
            IsFlight = false;
            OnDisableFlight.OnNext(new Unit());
        }

        public void Jump()
        {
            OnJump.OnNext(new Unit());
        }

        public void Ascend()
        {
            OnAscend.OnNext(new Unit());
        }

        public void Descend()
        {
            OnDescend.OnNext(new Unit());
        }
    }
}