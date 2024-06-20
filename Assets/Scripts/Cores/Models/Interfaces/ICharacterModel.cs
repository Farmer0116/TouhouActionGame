using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// キャラクタに関するモデル
    /// </summary>
    public interface ICharacterModel
    {
        int Id { get; set; }
        string Name { get; set; }
        float Health { get; set; }
        float Attack { get; set; }
        float Speed { get; set; }

        GameObject CharacterInstance { get; }
        CompositeDisposable DespawnDisposables { get; }

        Subject<GameObject> OnSpawnSubject { get; }
        Subject<GameObject> OnDespawnSubject { get; }

        GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale);
        void Despawn();
    }
}