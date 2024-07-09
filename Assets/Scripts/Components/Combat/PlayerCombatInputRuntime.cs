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

        private void Awake()
        {
            if (_inputSystemModel == null) _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            _combatMotor = gameObject.GetComponent<CombatMotor>();
            if (_combatMotor == null) Debug.LogError("CombatMotorが取得できませんでした");
        }

        private void Start()
        {
            if (_zenAutoInjecter != null) Destroy(_zenAutoInjecter);

            // _inputSystemModel.NormalAttack.Where(flag => flag).Subscribe(flag =>
            // {
            //     _combatMotor.SetInput(new CombatInput(isNormalAttack: true));
            // }).AddTo(_disposables);

            _inputSystemModel.MagicAttack.Where(flag => flag).Subscribe(flag =>
            {
                _combatMotor.SetInput(new CombatInput(isMagicAttack: true));
            }).AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Clear();
        }
    }
}
