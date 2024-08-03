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
        // 行動パターン
        bool IsNormalAttack { get; set; }
        bool IsMagicAttack { get; set; }
        bool IsLockOn { get; }
        Transform LockOnTarget { get; }
        bool IsFlight { get; }
        bool IsAscending { get; }
        bool IsDescending { get; }
        // イベント
        Subject<GameObject> OnSpawnSubject { get; }
        Subject<GameObject> OnDespawnSubject { get; }
        Subject<Transform> OnLockOn { get; }
        Subject<Unit> OnUnLock { get; }
        Subject<Unit> OnFlightEnabled { get; }
        Subject<Unit> OnFlightDisabled { get; }
        Subject<Unit> OnStartAscending { get; }
        Subject<Unit> OnEndAscending { get; }
        Subject<Unit> OnStartDescending { get; }
        Subject<Unit> OnEndDescending { get; }
        // インスタンス
        GameObject CharacterInstance { get; }
        // Dispose
        CompositeDisposable DespawnDisposables { get; }
        // 関数
        GameObject Spawn(Vector3 position, Quaternion rotation);
        void Despawn();
        void LockOn(Transform transform);
        void UnLock();
        void FlightEnabled();
        void FlightDisabled();
        void StartAscending();
        void EndAscending();
        void StartDescending();
        void EndDescending();
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