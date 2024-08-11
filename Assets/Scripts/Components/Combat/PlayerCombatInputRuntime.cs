using System.Linq;
using Components.Character;
using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace Components.Combat
{
    public class PlayerCombatInputRuntime : MonoBehaviour
    {
        private CombatExecutor _combatExecutor;
        private IInputSystemModel _inputSystemModel;
        private CharacterModelComponent _characterModelComponent;
        private (bool normalAttack, bool magicAttack, bool lockOn) _inputState;
        private ZenAutoInjecter _zenAutoInjecter;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel
        )
        {
            _inputSystemModel = inputSystemModel;
            var injecter = GetComponent<ZenAutoInjecter>();
        }

        public void Init(CharacterModelComponent characterModelComponent)
        {
            _characterModelComponent = characterModelComponent;
        }

        private void Awake()
        {
            if (_inputSystemModel == null) _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            _combatExecutor = gameObject.GetComponent<CombatExecutor>();
            if (_combatExecutor == null) Debug.LogError("CombatExecutorが取得できませんでした");
        }

        private void Update()
        {
            // 魔法攻撃
            _combatExecutor.SetInput(new CombatInput(isMagicAttack: _inputState.magicAttack));
        }

        private void Start()
        {
            if (_zenAutoInjecter != null)
            {
                Destroy(_zenAutoInjecter);
                _zenAutoInjecter = null;
            }

            SetUpInput();
            SetUpModelEvent();
        }

        /// <summary>
        /// 入力とモデルのイベントの紐づけ
        /// </summary>
        private void SetUpInput()
        {
            // 通常攻撃
            _inputSystemModel.NormalAttack.Subscribe(value =>
            {
                _inputState.normalAttack = value;
                _characterModelComponent.CharacterModel.NormalAttack();
            }).AddTo(_disposables);

            // 魔法攻撃
            _inputSystemModel.MagicAttack.Subscribe(value =>
            {
                _inputState.magicAttack = value;
                _characterModelComponent.CharacterModel.MagicAttack();
            }).AddTo(_disposables);

            // ロックオン
            _inputSystemModel.LockOn.Subscribe(value =>
            {
                _inputState.lockOn = value;
                if (value)
                {
                    if (_characterModelComponent.CharacterModel.IsLockOn)
                    {
                        _characterModelComponent.CharacterModel.UnLock();
                    }
                    else
                    {
                        // todo: 取得する敵を選別
                        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                        if (enemies.Count() > 0)
                        {
                            var target = enemies[0].GetComponent<CombatExecutor>().Target;
                            _characterModelComponent.CharacterModel.LockOn(target);
                        }
                    }
                }
            }).AddTo(_disposables);
        }

        /// <summary>
        /// モデルのイベントと紐づけ
        /// </summary>
        private void SetUpModelEvent()
        {
            // 通常攻撃
            _characterModelComponent.CharacterModel.OnNormalAttack.Subscribe(_ =>
            {
                _combatExecutor.SetInput(new CombatInput(isNormalAttack: true));
            }).AddTo(_disposables);

            // 魔法攻撃
            _characterModelComponent.CharacterModel.OnMagicAttack.Subscribe(_ =>
            {
            }).AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
