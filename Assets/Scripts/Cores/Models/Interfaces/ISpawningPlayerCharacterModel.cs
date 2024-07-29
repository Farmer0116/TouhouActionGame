using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// 空間に生成されているプレーヤー向けのキャラクター
    /// </summary>
    public interface ISpawningPlayerCharacterModel
    {
        ReactiveProperty<ICharacterModel> CharacterModel { get; }
        ReactiveProperty<bool> IsNormalAttack { get; }
        ReactiveProperty<bool> IsMagicAttack { get; }
        ReactiveProperty<bool> IsLockOn { get; }
        ReactiveProperty<Transform> LockOnTarget { get; }

        void Add(ICharacterModel characterModel);
        void Remove();
    }
}
