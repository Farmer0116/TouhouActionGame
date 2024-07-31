using Components.Character;
using Components.Combat;
using Cores.Models.Interfaces;
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

        public static GameObject SpawnCharacter(ControllerType controllerType, GameObject character, Vector3 position, Quaternion rotation, ICharacterModel characterModel)
        {
            var root = GameObject.Instantiate(character, position, rotation);
            var characterModelComponent = root.GetComponent<CharacterModelComponent>();
            characterModelComponent.CharacterModel = characterModel;

            switch (controllerType)
            {
                case ControllerType.Player:
                    SetupPlayerCharacter(root);
                    break;
                case ControllerType.Enemy:
                    SetupEnemyCharacter(root);
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
        private static void SetupPlayerCharacter(GameObject character)
        {
            character.tag = PlayerTag;
            character.layer = PlayerLayer;

            var characterMovementController = character.GetComponent<CharacterMovementController>();
            var characterModelComponent = character.GetComponent<CharacterModelComponent>();

            var input = character.AddComponent<PlayerCharacterInputRuntime>();
            var combat = character.AddComponent<PlayerCombatInputRuntime>();
            input.Init(characterMovementController, characterModelComponent);
            combat.Init(characterModelComponent);
        }

        /// <summary>
        /// 敵キャラクターセットアップ
        /// </summary>
        private static void SetupEnemyCharacter(GameObject character)
        {
            character.tag = EnemyTag;
            character.layer = EnemyLayer;
        }
    }
}
