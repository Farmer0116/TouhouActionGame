using Components.Character;
using Types.Character;
using UnityEngine;

namespace Utilities
{
    public static class CharacterUtility
    {
        public static GameObject SpawnCharacter(ControllerType controllerType, GameObject character, Vector3 position, Quaternion rotation)
        {
            var instance = GameObject.Instantiate(character, position, rotation);
            var characterMovementController = instance.GetComponent<CharacterMovementController>();
            switch (controllerType)
            {
                case ControllerType.Player:
                    var input = instance.AddComponent<PlayerCharacterInputRuntime>();
                    input.SetCharacterMovementController(characterMovementController);
                    break;
                case ControllerType.Enemy:
                    break;
                case ControllerType.Neutral:
                    break;
                case ControllerType.Non:
                    break;
            }
            return instance;
        }
    }
}
