using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// プレーヤーキャラクタの状態を保持するモデル
    /// </summary>
    public interface IPlayerCharacterModel
    {
        ICharacterModel CharacterModel { get; set; }
        Quaternion CharacterRotation { get; set; }
        bool IsNormalAttack { get; set; }
        bool IsMagicAttack { get; set; }
        bool IsLockOn { get; set; }
        Transform LockOnTarget { get; set; }
        OrientationMethod OrientationMethod { get; set; }

        ReactiveProperty<bool> OnChangeIsLockOn { get; }
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }
}
