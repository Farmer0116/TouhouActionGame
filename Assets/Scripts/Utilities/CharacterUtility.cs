using Types.Character;
using UnityEngine;

namespace Utilities
{
    public static class CharacterUtility
    {
        public static GameObject SpawnCharacter(ControllerType controllerType, GameObject character, Vector3 position, Quaternion rotation)
        {
            var instance = GameObject.Instantiate(character, position, rotation);
            return instance;
        }
    }
}
