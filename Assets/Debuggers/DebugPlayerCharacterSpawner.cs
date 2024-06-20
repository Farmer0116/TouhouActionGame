using Cores.Models;
using Cores.Models.Interfaces;
using ScriptableObjects.Character;
using Types.Character;
using UniRx;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

public class DebugPlayerCharacterSpawner : MonoBehaviour
{
    private CharacterModel.Factory _factory;
    private ICharacterModel _characterModel;
    private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public CharacterParamAsset _characterParamAsset;

    [Inject]
    public void Construct
    (
        CharacterModel.Factory factory,
        ISpawningPlayerCharacterModel spawningPlayerCharacterModel
    )
    {
        _factory = factory;
        _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
    }

    void Start()
    {
        var characterParam = _characterParamAsset.CharacterParamList[0];

        _characterModel = _factory.Create(new CharacterModelParam(
            characterParam.Id,
            characterParam.Name,
            characterParam.Model,
            characterParam.Health,
            characterParam.Attack,
            characterParam.Speed,
            ControllerType.Player
        ));

        // スポーン時にSpawingModelに追加
        _characterModel.OnSpawnSubject.Subscribe(root =>
        {
            _spawningPlayerCharacterModel.Add(_characterModel);
        }).AddTo(_disposables);

        // デスポーン時にSpawingModelから削除
        _characterModel.OnDespawnSubject.Subscribe(root =>
        {
            // _spawningPlayerCharacterModel.Remove();
        }).AddTo(_disposables);

        // キャラクタールート生成
        _characterModel.Spawn(
            Vector3.zero,
            quaternion.identity,
            Vector3.one
        );
    }

    void Destroy()
    {
        if (_disposables != null) _disposables.Clear();
    }
}
