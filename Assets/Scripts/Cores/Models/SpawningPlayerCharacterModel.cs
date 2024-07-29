using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;

namespace Cores.Models
{
    public class SpawningPlayerCharacterModel : ISpawningPlayerCharacterModel
    {
        public ReactiveProperty<ICharacterModel> CharacterModel { get; private set; } = new ReactiveProperty<ICharacterModel>();
        public ReactiveProperty<bool> IsNormalAttack { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsMagicAttack { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<bool> IsLockOn { get; private set; } = new ReactiveProperty<bool>(false);
        public ReactiveProperty<Transform> LockOnTarget { get; private set; } = new ReactiveProperty<Transform>();

        public void Add(ICharacterModel characterModel)
        {
            CharacterModel.Value = characterModel;
            if (CharacterModel.Value != null)
            {
                Debug.LogWarning("すでにキャラクターを生成済みです。そのため上書きを実行しました。");
            }
        }

        public void Remove()
        {
            if (CharacterModel.Value != null)
            {
                CharacterModel.Value = null;
            }
            else
            {
                Debug.LogWarning("削除対象となるキャラクターが格納されていません。");
            }
        }
    }
}
