using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;

namespace Cores.Models
{
    public class PlayerCharacterModel : IPlayerCharacterModel
    {
        public ICharacterModel CharacterModel { get; set; }
        public Quaternion CharacterRotation { get; set; }
        public bool IsNormalAttack { get; set; } = false;
        public bool IsMagicAttack { get; set; } = false;
        public bool IsLockOn { get => OnChangeIsLockOn.Value; set { OnChangeIsLockOn.Value = value; } }
        public Transform LockOnTarget { get; set; }
        public OrientationMethod OrientationMethod { get; set; } = OrientationMethod.TowardsCamera;

        public ReactiveProperty<bool> OnChangeIsLockOn { get; private set; } = new ReactiveProperty<bool>(false);
    }
}
