using ScriptableObjects.Character;
using UnityEngine;
using Zenject;

namespace Cores.Installers
{
    [CreateAssetMenu(fileName = "GameDataInstaller", menuName = "Installers/GameDataInstaller")]
    public class GameDataInstaller : ScriptableObjectInstaller<GameDataInstaller>
    {
        [SerializeField] private CharacterParamAsset _characterParamAsset;

        public override void InstallBindings()
        {
            Container.BindInstance(_characterParamAsset).AsCached();
        }
    }
}