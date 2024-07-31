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
        bool IsLockOn { get; set; }
        Transform LockOnTarget { get; set; }
        // イベント
        Subject<GameObject> OnSpawnSubject { get; }
        Subject<GameObject> OnDespawnSubject { get; }
        ReactiveProperty<bool> OnChangeIsLockOn { get; }
        // インスタンス
        GameObject CharacterInstance { get; }
        // Dispose
        CompositeDisposable DespawnDisposables { get; }

        GameObject Spawn(Vector3 position, Quaternion rotation);
        void Despawn();
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