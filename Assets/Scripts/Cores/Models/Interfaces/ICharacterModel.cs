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
        CharacterModelParam CharacterModelParam { get; set; }

        GameObject CharacterInstance { get; }
        CompositeDisposable DespawnDisposables { get; }

        Subject<GameObject> OnSpawnSubject { get; }
        Subject<GameObject> OnDespawnSubject { get; }

        GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale);
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
        public float Speed { get; set; }

        public CharacterModelParam
        (
            int id,
            string name,
            GameObject model,
            float health,
            float attack,
            float speed,
            ControllerType controllerType = ControllerType.Non
        )
        {
            Id = id;
            Name = name;
            ControllerType = controllerType;
            Model = model;
            Health = health;
            Attack = attack;
            Speed = speed;
        }
    }
}