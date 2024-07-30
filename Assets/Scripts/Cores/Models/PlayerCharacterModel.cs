using Cores.Models.Interfaces;
using UnityEngine;

namespace Cores.Models
{
    public class PlayerCharacterModel : IPlayerCharacterModel
    {
        public ICharacterModel CharacterModel { get; set; }
        public bool IsNormalAttack { get; set; }
        public bool IsMagicAttack { get; set; }
        public bool IsLockOn { get; set; }
        public Transform LockOnTarget { get; set; }
        public OrientationMethod OrientationMethod { get; set; } = OrientationMethod.TowardsCamera;
    }
}
