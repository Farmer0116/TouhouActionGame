using Cores.Models.Interfaces;
using UnityEngine;

namespace Components.Character
{
    public class CharacterModelComponent : MonoBehaviour
    {
        public ICharacterModel CharacterModel;

        public Transform Center;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;
        [HideInInspector] public Quaternion CharacterRotation;
    }
}
