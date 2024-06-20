using UniRx;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// 空間に生成されているプレーヤー向けのキャラクター
    /// </summary>
    public interface ISpawningPlayerCharacterModel
    {
        ReactiveProperty<ICharacterModel> CharacterModel { get; set; }

        void Add(ICharacterModel characterModel);
        void Remove();
    }
}
