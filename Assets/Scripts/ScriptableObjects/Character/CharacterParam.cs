using Types.Character;
using UnityEngine;

namespace ScriptableObjects.Character
{
    [System.Serializable]
    public class CharacterParam
    {
        [SerializeField]
        public int Id;

        [SerializeField]
        public CharacterNameType NameType;

        [SerializeField]
        public GameObject Model;

        [SerializeField]
        public float Health;

        [SerializeField]
        public float Attack;

        [SerializeField]
        public float Speed;
    }
}