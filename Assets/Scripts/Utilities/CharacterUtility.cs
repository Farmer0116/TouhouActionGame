using Components.Character;
using Components.Combat;
using Types.Character;
using UnityEngine;

namespace Utilities
{
    public static class CharacterUtility
    {
        public static string PlayerTag = "Player";
        public static string EnemyTag = "Enemy";

        public static LayerMask PlayerLayer { get { return LayerMask.NameToLayer("Player"); } }
        public static LayerMask EnemyLayer { get { return LayerMask.NameToLayer("Enemy"); } }

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
                    SetupEnemyCharacter(root, characterMovementController);
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
            character.tag = PlayerTag;
            character.layer = PlayerLayer;

            var input = character.AddComponent<PlayerCharacterInputRuntime>();
            var combat = character.AddComponent<PlayerCombatInputRuntime>();
            input.Initialize(characterMovementController);
        }

        /// <summary>
        /// 敵キャラクターセットアップ
        /// </summary>
        private static void SetupEnemyCharacter(GameObject character, CharacterMovementController characterMovementController)
        {
            character.tag = EnemyTag;
            character.layer = EnemyLayer;
        }
    }
}
