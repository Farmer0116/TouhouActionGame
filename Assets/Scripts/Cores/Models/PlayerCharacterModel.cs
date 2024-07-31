using Cores.Models.Interfaces;

namespace Cores.Models
{
    public class PlayerCharacterModel : IPlayerCharacterModel
    {
        public ICharacterModel CharacterModel { get; set; }
    }
}
