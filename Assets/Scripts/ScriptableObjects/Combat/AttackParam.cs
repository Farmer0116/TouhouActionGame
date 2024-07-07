using Types.Combat;
using UnityEngine;
using UnityEngine.Playables;

namespace ScriptableObjects.Character
{
    [System.Serializable]
    public class AttackParam
    {
        [SerializeField]
        public PlayableDirector PlayableDirector;

        [SerializeField]
        public AttackType AttackType;

        [SerializeField]
        public float Damage;
    }
}