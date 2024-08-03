using Types.Character;
using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// キャラクタに関するモデル
    /// </summary>
    public interface ICharacterModel
    {
        // パラメータ
        CharacterModelParam CharacterModelParam { get; set; }

        // 状態
        bool IsLockOn { get; }
        Transform LockOnTarget { get; }
        bool IsFlight { get; }

        // イベント
        Subject<GameObject> OnSpawn { get; }
        Subject<GameObject> OnDespawn { get; }
        Subject<Transform> OnLockOn { get; }
        Subject<Unit> OnUnLock { get; }
        Subject<Unit> OnEnableFlight { get; }
        Subject<Unit> OnDisableFlight { get; }
        Subject<Unit> OnNormalAttack { get; }
        Subject<Unit> OnMagicAttack { get; }
        Subject<Unit> OnJump { get; }
        Subject<Unit> OnAscend { get; }
        Subject<Unit> OnDescend { get; }

        // インスタンス
        GameObject CharacterInstance { get; }

        // Dispose
        CompositeDisposable DespawnDisposables { get; }

        // 関数
        GameObject Spawn(Vector3 position, Quaternion rotation);
        void Despawn();
        void LockOn(Transform transform);
        void UnLock();
        void EnableFlight();
        void DisableFlight();
        void NormalAttack();
        void MagicAttack();
        void Jump();
        void Ascend();
        void Descend();
    }

    // 初期化パラメータ
    public class CharacterModelParam
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ControllerType ControllerType { get; set; }
        public GameObject Model { get; set; }
        public float Health { get; set; }
        public float Attack { get; set; }

        public CharacterModelParam
        (
            int id,
            string name,
            GameObject model,
            float health,
            float attack,
            ControllerType controllerType = ControllerType.Non
        )
        {
            Id = id;
            Name = name;
            ControllerType = controllerType;
            Model = model;
            Health = health;
            Attack = attack;
        }
    }
}