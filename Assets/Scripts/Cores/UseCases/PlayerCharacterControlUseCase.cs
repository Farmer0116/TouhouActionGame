using Cores.Models.Interfaces;
using Cores.UseCases.Interfaces;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Cores.UseCases
{
    public class PlayerCharacterControlUseCase : IPlayerCharacterControlUseCase
    {
        private IInputSystemModel _inputSystemModel;
        private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;
        private CompositeDisposable _disposables = new CompositeDisposable();

        public PlayerCharacterControlUseCase
        (
            IInputSystemModel inputSystemModel,
            ISpawningPlayerCharacterModel spawningPlayerCharacterModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
        }

        public async UniTask Begin()
        {
        }

        public void Finish()
        {
            _disposables.Dispose();
        }
    }
}
