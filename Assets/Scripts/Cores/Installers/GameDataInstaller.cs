using ScriptableObjects.Camera;
using ScriptableObjects.Character;
using UnityEngine;
using Zenject;

namespace Cores.Installers
{
    [CreateAssetMenu(fileName = "GameDataInstaller", menuName = "Installers/GameDataInstaller")]
    public class GameDataInstaller : ScriptableObjectInstaller<GameDataInstaller>
    {
        [SerializeField] private CharacterParamAsset _characterParamAsset;
        [SerializeField] private PlayerCharacterCameraAsset _playerCharacterCameraAsset;

        public override void InstallBindings()
        {
            Container.BindInstance(_characterParamAsset).AsCached();
            Container.BindInstance(_playerCharacterCameraAsset).AsCached();
        }
    }
}