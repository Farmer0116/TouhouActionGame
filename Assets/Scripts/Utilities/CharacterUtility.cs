using Components.Character;
using Components.Combat;
using Types.Character;
using UnityEngine;

namespace Utilities
{
    public static class CharacterUtility
    {
        public static GameObject SpawnCharacter(ControllerType controllerType, GameObject character, Vector3 position, Quaternion rotation)
        {
            var root = GameObject.Instantiate(character, position, rotation);
            var characterMovementController = root.GetComponent<CharacterMovementController>();
            switch (controllerType)
            {
                case ControllerType.Player:
                    SetupPlayerCharacter(root, characterMovementController);
                    break;
                case ControllerType.Enemy:
                    break;
                case ControllerType.Neutral:
                    break;
                case ControllerType.Non:
                    break;
            }
            return root;
        }

        /// <summary>
        /// プレーヤー向けのキャラクターセットアップ
        /// </summary>
        private static void SetupPlayerCharacter(GameObject character, CharacterMovementController characterMovementController)
        {
            var input = character.AddComponent<PlayerCharacterInputRuntime>();
            var combat = character.AddComponent<PlayerCombatInputRuntime>();
            input.Initialize(characterMovementController);
        }
    }
}
