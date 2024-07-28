using UnityEngine;
using Zenject;
using Cores.Models;
using Cores.Models.Interfaces;

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

            // Model
            Container.Bind<IInputSystemModel>().To<InputSystemModel>().AsCached().IfNotBound();
            Container.Bind<ISpawningPlayerCharacterModel>().To<SpawningPlayerCharacterModel>().AsCached().IfNotBound();
            Container.Bind<ISpawningCameraModel>().To<SpawningCameraModel>().AsCached().IfNotBound();

            // Factory
            Container.BindFactory<CharacterModelParam, ReimuModel, ReimuModel.Factory>();
        }
    }
}