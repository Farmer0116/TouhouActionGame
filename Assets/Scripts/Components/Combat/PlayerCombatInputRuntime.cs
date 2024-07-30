using System.Linq;
using Cores.Models.Interfaces;
using UniRx;
using UnityEngine;
using Zenject;

namespace Components.Combat
{
    public class PlayerCombatInputRuntime : MonoBehaviour
    {
        private CombatMotor _combatMotor;
        private IInputSystemModel _inputSystemModel;
        private IPlayerCharacterModel _playerCharacterModel;
        private ZenAutoInjecter _zenAutoInjecter;
        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            IPlayerCharacterModel playerCharacterModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _playerCharacterModel = playerCharacterModel;
            var injecter = GetComponent<ZenAutoInjecter>();
        }

        private void Awake()
        {
            if (_inputSystemModel == null) _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            _combatMotor = gameObject.GetComponent<CombatMotor>();
            if (_combatMotor == null) Debug.LogError("CombatMotorが取得できませんでした");
        }

        private void Update()
        {
            // 魔法攻撃
            _combatMotor.SetInput(new CombatInput(isMagicAttack: _inputSystemModel.MagicAttack.Value));
            _playerCharacterModel.IsMagicAttack = _inputSystemModel.MagicAttack.Value;
        }

        private void Start()
        {
            if (_zenAutoInjecter != null)
            {
                Destroy(_zenAutoInjecter);
                _zenAutoInjecter = null;
            }

            // 通常攻撃
            _inputSystemModel.NormalAttack.Subscribe(flag =>
            {
                if (flag) _combatMotor.SetInput(new CombatInput(isNormalAttack: true));
                _playerCharacterModel.IsNormalAttack = flag;
            }).AddTo(_disposables);

            // _inputSystemModel.MagicAttack.Where(flag => flag).Subscribe(flag =>
            // {
            //     _combatMotor.SetInput(new CombatInput(isMagicAttack: true));
            // }).AddTo(_disposables);

            // ロックオン
            _inputSystemModel.LockOn.Where(flag => flag).Subscribe(flag =>
            {
                // todo: 取得する敵を選別
                var enemies = GameObject.FindGameObjectsWithTag("Enemy");
                if (enemies.Count() > 0) _playerCharacterModel.LockOnTarget = enemies[0].GetComponent<CombatMotor>().Target;

                _playerCharacterModel.IsLockOn = !_playerCharacterModel.IsLockOn;
            }).AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
