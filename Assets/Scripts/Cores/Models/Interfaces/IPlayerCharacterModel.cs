using UniRx;
using UnityEngine;

namespace Cores.Models.Interfaces
{
    /// <summary>
    /// プレーヤーキャラクタを保持するモデル
    /// </summary>
    public interface IPlayerCharacterModel
    {
        ICharacterModel CharacterModel { get; set; }
    }


}
