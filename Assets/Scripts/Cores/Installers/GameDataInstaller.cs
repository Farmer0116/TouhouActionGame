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
        [SerializeField] private PlayerCameraAsset _playerCameraAsset;

        public override void InstallBindings()
        {
            Container.BindInstance(_characterParamAsset).AsCached();
            Container.BindInstance(_playerCameraAsset).AsCached();
        }
    }
}