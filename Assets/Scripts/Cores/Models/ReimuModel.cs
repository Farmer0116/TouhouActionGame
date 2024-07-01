using Cores.Models.Interfaces;
using Zenject;

namespace Cores.Models
{
    public class ReimuModel : CharacterModel, IReimuModel
    {
        public class Factory : PlaceholderFactory<CharacterModelParam, ReimuModel> { }

        public ReimuModel(CharacterModelParam characterModelParam) : base(characterModelParam)
        {
            CharacterModelParam = characterModelParam;
        }
    }
}
