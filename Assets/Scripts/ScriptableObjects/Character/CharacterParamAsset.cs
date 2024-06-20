using System.Collections.Generic;
using UnityEngine;


namespace ScriptableObjects.Character
{
    [CreateAssetMenu(fileName = "CharacterParamAsset", menuName = "ScriptableObjects/CreateCharacterParamAsset")]
    public class CharacterParamAsset : ScriptableObject
    {
        public List<CharacterParam> CharacterParamList = new List<CharacterParam>();
    }
}