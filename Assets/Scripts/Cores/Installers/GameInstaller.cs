using UnityEngine;
using Zenject;
using Cores.Models;
using Cores.Models.Interfaces;
using Cores.UseCases;
using Cores.UseCases.Interfaces;

namespace Cores.Installers
{
    public class GameInstaller : MonoInstaller
    {
        [field: Header("メイン")]
        [SerializeField] private GameObject main = default;

        public GameObject Main => main;

        public override void InstallBindings()
        {
            // if (Container.HasBinding<ITest>() && main != null)
            // {
            //     main.SetActive(false);
            // }

            // UseCase
            Container.Bind<IPlayerCharacterControlUseCase>().To<PlayerCharacterControlUseCase>().AsCached().IfNotBound();

            // Model
            Container.Bind<IInputSystemModel>().To<InputSystemModel>().AsCached().IfNotBound();
            Container.Bind<ISpawningPlayerCharacterModel>().To<SpawningPlayerCharacterModel>().AsCached().IfNotBound();
            Container.Bind<ISpawningCameraModel>().To<SpawningCameraModel>().AsCached().IfNotBound();

            // Factory
            Container.BindFactory<CharacterModelParam, ReimuModel, ReimuModel.Factory>();
        }
    }
}