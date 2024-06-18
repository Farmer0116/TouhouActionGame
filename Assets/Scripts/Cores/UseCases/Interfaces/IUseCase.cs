using Cysharp.Threading.Tasks;

namespace Base.Domains.UseCases
{
    public interface IUseCase
    {
        UniTask Begin();
        void Finish();
    }
}