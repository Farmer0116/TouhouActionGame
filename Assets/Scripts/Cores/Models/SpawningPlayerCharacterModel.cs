using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;

namespace Cores.Models
{
    public class SpawningPlayerCharacterModel : ISpawningPlayerCharacterModel
    {
        public ReactiveProperty<ICharacterModel> CharacterModel { get { return _characterModel; } set { _characterModel = value; } }

        private ReactiveProperty<ICharacterModel> _characterModel = new ReactiveProperty<ICharacterModel>();

        public void Add(ICharacterModel characterModel)
        {
            _characterModel.Value = characterModel;
            if (_characterModel.Value != null)
            {
                Debug.LogWarning("すでにキャラクターを生成済みです。そのため上書きを実行しました。");
            }
        }

        public void Remove()
        {
            if (_characterModel.Value != null)
            {
                _characterModel.Value = null;
            }
            else
            {
                Debug.LogWarning("削除対象となるキャラクターが格納されていません。");
            }
        }
    }
}
