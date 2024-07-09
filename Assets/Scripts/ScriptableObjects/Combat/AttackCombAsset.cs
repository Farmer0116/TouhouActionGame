using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects.Character
{
    [CreateAssetMenu(fileName = "AttackCombAsset", menuName = "ScriptableObjects/CreateAttackCombAsset")]
    public class AttackCombAsset : ScriptableObject
    {
        public List<AttackParam> AttackCombs = new List<AttackParam>();
    }
}
