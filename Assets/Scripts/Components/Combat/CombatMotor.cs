using System;
using System.Collections.Generic;
using Cores.Models.Interfaces;
using ScriptableObjects.Character;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
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

        public AttackCombAsset NormalCombo;
        public AttackCombAsset MagicCombo;
        public Transform NormalSpawnPoint;
        public Transform MagicSpawnPoint;

        public IReadOnlyReactiveProperty<bool> OnNormalAttack => _onNormalAttack;
        public IReadOnlyReactiveProperty<bool> OnMagicAttack => _onMagicAttack;
        private ReactiveProperty<bool> _onNormalAttack = new ReactiveProperty<bool>();
        private ReactiveProperty<bool> _onMagicAttack = new ReactiveProperty<bool>();

        [SerializeField] private List<PlayableDirector> _normalAttackInstance = new List<PlayableDirector>();
        [SerializeField] private List<PlayableDirector> _magicAttackInstance = new List<PlayableDirector>();

        private Vector3 _targetPoint = Vector3.zero;
        // _combFlag: コンボ無効＆コンボ未入力：0 / コンボ有効＆コンボ未入力：1 / コンボ有効＆コンボ入力済：2
        private (int _comboFlag, int _comboCount) _combo = (0, 0);
        // 
        private enum CurrentAttackingType { non, normal, magic };
        private CurrentAttackingType _currentAttackFlag = CurrentAttackingType.non;

        private CompositeDisposable _disposables = new CompositeDisposable();

        public void SetInput(CombatInput combatInput)
        {
            _targetPoint = combatInput.TargetPoint;
            _onNormalAttack.Value = combatInput.IsNormalAttack;
            _onMagicAttack.Value = combatInput.IsMagicAttack;
        }

        void Start()
        {
            // 通常攻撃
            OnNormalAttack
            .Where(flag => flag)
            .Subscribe(flag =>
            {
                _onNormalAttack.Value = false;
                // if ((!_isCombEnabled && _comb._combCount == 0) || (_isCombEnabled && _comb._combCount > 0))
                if (CanContinueCombo(NormalCombo.AttackCombs.Count, CurrentAttackingType.normal))
                {
                    StartCombo(CurrentAttackingType.normal);

                    // 生成
                    var normal = CombatUtility.SpawnAttack(
                        NormalCombo.AttackCombs[_combo._comboCount].PlayableDirector,
                        NormalSpawnPoint.position,
                        NormalSpawnPoint.rotation,
                        CombatUtility.PlayerToEnemyMask,
                        true
                    );
                    _normalAttackInstance.Add(normal);

                    // コンボ継続の場合は特定のイベント削除
                    if (_combo._comboCount > 0)
                    {
                        var beforeAttack = _normalAttackInstance[_normalAttackInstance.IndexOf(normal) - 1].gameObject;
                        if (beforeAttack != null)
                        {
                            CombatUtility.RemoveCheckComboEvent(beforeAttack, OnComboChecked);
                            CombatUtility.RemoveFinishAttackEvent(beforeAttack, OnFinishAttack);
                        }
                    }

                    // イベント登録
                    Action removeEL = () => { _normalAttackInstance.Remove(normal); };
                    CombatUtility.SetEnableComboEvent(normal.gameObject, OnComboEnabled);
                    CombatUtility.SetCheckComboEvent(normal.gameObject, OnComboChecked);
                    CombatUtility.SetFinishAttackEvent(normal.gameObject, OnFinishAttack);
                    CombatUtility.SetFinishAttackEvent(normal.gameObject, removeEL);

                    _combo._comboCount++;
                }
            }).AddTo(_disposables);

            // 魔法攻撃
            OnMagicAttack
            .Where(flag => flag)
            .Subscribe(flag =>
            {
                _onMagicAttack.Value = false;
                // if ((_combo._comboFlag == 0 && _combo._comboCount == 0) || (_combo._comboFlag == 1 && _combo._comboCount > 0))
                if (CanContinueCombo(MagicCombo.AttackCombs.Count, CurrentAttackingType.magic))
                {
                    StartCombo(CurrentAttackingType.magic);

                    // 生成
                    var magic = CombatUtility.SpawnAttack(
                        MagicCombo.AttackCombs[_combo._comboCount].PlayableDirector,
                        MagicSpawnPoint.position,
                        MagicSpawnPoint.rotation,
                        CombatUtility.PlayerToEnemyMask,
                        true
                    );
                    _magicAttackInstance.Add(magic);

                    // コンボ継続の場合は特定のイベント削除
                    if (_combo._comboCount > 0)
                    {
                        var beforeAttack = _magicAttackInstance[_magicAttackInstance.IndexOf(magic) - 1].gameObject;
                        if (beforeAttack != null)
                        {
                            CombatUtility.RemoveCheckComboEvent(beforeAttack, OnComboChecked);
                            CombatUtility.RemoveFinishAttackEvent(beforeAttack, OnFinishAttack);
                        }
                    }

                    // イベント
                    Action removeEL = () => { _magicAttackInstance.Remove(magic); };
                    CombatUtility.SetEnableComboEvent(magic.gameObject, OnComboEnabled);
                    CombatUtility.SetCheckComboEvent(magic.gameObject, OnComboChecked);
                    CombatUtility.SetFinishAttackEvent(magic.gameObject, OnFinishAttack);
                    CombatUtility.SetFinishAttackEvent(magic.gameObject, removeEL);

                    _combo._comboCount++;
                }
            }).AddTo(_disposables);
        }

        private bool CanContinueCombo(int limitCombCount, CurrentAttackingType type)
        {
            // コンボなし || コンボ中かつ未入力時
            if ((_combo._comboFlag == 0 && _combo._comboCount == 0) || (_combo._comboFlag == 1 && _combo._comboCount > 0))
            {
                // 既定のコンボ内の時
                if (limitCombCount > _combo._comboCount)
                {
                    // 攻撃していない || 他の種類の攻撃中でないとき
                    if (_currentAttackFlag == type || _currentAttackFlag == CurrentAttackingType.non)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// コンボ入力の開始
        /// </summary>
        private void StartCombo(CurrentAttackingType type)
        {
            _currentAttackFlag = type;
            if (_combo._comboFlag == 1)
            {
                _combo._comboFlag = 2;
            }
        }

        /// <summary>
        /// コンボ有効化イベント
        /// </summary>
        private void OnComboEnabled()
        {
            if (_combo._comboCount == 1)
            {
                _combo._comboFlag = 1;
            }

            switch (_combo._comboFlag)
            {
                case 2:
                    _combo._comboFlag = 1;
                    break;
            }
        }

        /// <summary>
        /// コンボ入力確認イベント
        /// </summary>
        private void OnComboChecked()
        {
            switch (_combo._comboFlag)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }

        /// <summary>
        /// コンボ終了イベント
        /// </summary>
        private void OnFinishAttack()
        {
            _currentAttackFlag = CurrentAttackingType.non;
            _combo._comboCount = 0;
            _combo._comboFlag = 0;
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
