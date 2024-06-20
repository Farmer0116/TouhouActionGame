using Cores.Models;
using Cores.Models.Interfaces;
using ScriptableObjects.Character;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class DebugCharacterSpawner : MonoBehaviour
{
    private CharacterModel.Factory _factory;
    private ICharacterModel _characterModel;

    public CharacterParamAsset _characterParamAsset;

    [Inject]
    public void Construct(CharacterModel.Factory factory)
    {
        _factory = factory;
    }

    void Start()
    {
        var characterParam = _characterParamAsset.CharacterParamList[0];

        _characterModel = _factory.Create(new CharacterModel.CharacterModelParam(
            characterParam.Id,
            characterParam.Name,
            characterParam.Model,
            characterParam.Health,
            characterParam.Attack,
            characterParam.Speed
        ));
        _characterModel.Spawn(
            Vector3.zero,
            quaternion.identity,
            Vector3.one
        );
    }
}
