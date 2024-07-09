using ScriptableObjects.Character;
using UniRx;
using UnityEngine;
using Utilities;

namespace Components.Combat
{
    public class CombatInput
    {
        public CombatInput(bool isNormalAttack = false, bool isMagicAttack = false, Vector3 targetPoint = default)
        {
            IsNormalAttack = isNormalAttack;
            IsMagicAttack = isMagicAttack;
            TargetPoint = targetPoint;
        }

        public bool IsNormalAttack;
        public bool IsMagicAttack;
        public Vector3 TargetPoint;
    }

    public class CombatMotor : MonoBehaviour
    {
        public AttackCombAsset NormalComb;
        public AttackCombAsset MagicComb;
        public Transform NormalSpawnPoint;
        public Transform MagicSpawnPoint;

        public IReadOnlyReactiveProperty<bool> OnNormalAttack => _onNormalAttack;
        public IReadOnlyReactiveProperty<bool> OnMagicAttack => _onMagicAttack;
        private ReactiveProperty<bool> _onNormalAttack = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _onMagicAttack = new ReactiveProperty<bool>();

        private Vector3 _targetPoint = Vector3.zero;
        private int _normalCombCount = 0;
        private int _magicCombCount = 0;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public void SetInput(CombatInput combatInput)
        {
            _targetPoint = combatInput.TargetPoint;
            _onNormalAttack.Value = combatInput.IsNormalAttack;
            _onMagicAttack.Value = combatInput.IsMagicAttack;
        }

        void Start()
        {
            OnNormalAttack
            .Where(flag => flag)
            .Subscribe(flag =>
            {
                // 魔法攻撃
                var normal = CombatUtility.SpawnAttack(
                    NormalComb.AttackCombs[_normalCombCount].PlayableDirector,
                    NormalSpawnPoint.position,
                    NormalSpawnPoint.rotation,
                    CombatUtility.PlayerToEnemyMask,
                    true
                );

                _normalCombCount++;

                if (_normalCombCount > NormalComb.AttackCombs.Count - 1) _normalCombCount = 0;

                _onNormalAttack.Value = false;
            }).AddTo(_disposables);

            OnMagicAttack
            .Where(flag => flag)
            .Subscribe(flag =>
            {
                // 魔法攻撃
                var magic = CombatUtility.SpawnAttack(
                    MagicComb.AttackCombs[_magicCombCount].PlayableDirector,
                    MagicSpawnPoint.position,
                    MagicSpawnPoint.rotation,
                    CombatUtility.PlayerToEnemyMask,
                    true
                );

                _magicCombCount++;

                if (_magicCombCount > MagicComb.AttackCombs.Count - 1) _magicCombCount = 0;

                _onMagicAttack.Value = false;
            }).AddTo(_disposables);
        }

        void OnDestroy()
        {
            _disposables.Clear();
        }
    }
}
