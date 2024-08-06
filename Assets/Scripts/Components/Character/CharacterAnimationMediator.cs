using Cores.Models.Interfaces;
using KinematicCharacterController;
using UniRx;
using UnityEngine;
using Zenject;

namespace Components.Character
{
    public class CharacterAnimationMediator : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterModelComponent _characterModelComponent;
        [SerializeField] private KinematicCharacterMotor _kinematicCharacterMotor;
        [SerializeField] private CharacterMovementController _characterMovementController;

        [SerializeField] private Vector3 _currentVelocity;
        [SerializeField] private Vector3 _currentStableNormalizedVelocity;
        [SerializeField] private Vector3 _currentFlightNormalizedVelocity;
        [SerializeField] private int _speed;
        [SerializeField] private bool _isInputHorizontal;
        [SerializeField] private bool _isOnGround;
        [SerializeField] private bool _isFlight;
        [SerializeField] private bool _isLockOn;

        private IInputSystemModel _inputSystemModel;

        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        private void construct
        (
            IInputSystemModel inputSystemModel
        )
        {
            // 操作者がプレーヤーの時
            if (_characterModelComponent.CharacterModel.CharacterModelParam.ControllerType == Types.Character.ControllerType.Player) _inputSystemModel = inputSystemModel;
        }

        void Start()
        {
            _characterModelComponent.CharacterModel.OnDodge.Subscribe(_ =>
            {
                _animator.SetTrigger(AnimationType.Dodge.ToString());
            }).AddTo(_disposables);
        }

        void Update()
        {
            // 必要な情報の格納
            _currentVelocity = transform.InverseTransformDirection(_kinematicCharacterMotor.Velocity);
            _currentStableNormalizedVelocity = new Vector3(_currentVelocity.x / _characterMovementController.MaxStableRunMoveSpeed, 0, _currentVelocity.z / _characterMovementController.MaxStableRunMoveSpeed);
            _currentFlightNormalizedVelocity = new Vector3(_currentVelocity.x / _characterMovementController.FlightRunMoveSpeed, 0, _currentVelocity.z / _characterMovementController.FlightRunMoveSpeed);
            _isOnGround = _kinematicCharacterMotor.LastGroundingStatus.IsStableOnGround;
            _isFlight = _characterModelComponent.CharacterModel.IsFlight;
            _isLockOn = _characterModelComponent.CharacterModel.IsLockOn;
            if (_isInputHorizontal) _speed = _characterModelComponent.CharacterModel.IsRun ? 2 : 1;
            else _speed = 0;
            if (_characterModelComponent.CharacterModel.CharacterModelParam.ControllerType == Types.Character.ControllerType.Player)
            {
                _isInputHorizontal = _inputSystemModel.Move.Value.x == 0 && _inputSystemModel.Move.Value.y == 0 ? false : true;
            }

            // 飛行の移動速度
            if (_isFlight)
            {
            }
            // 通常の移動速度
            else
            {
            }

            // 移動速度
            _animator.SetFloat(AnimationType.VelocityX.ToString(), _currentVelocity.x);
            _animator.SetFloat(AnimationType.VelocityY.ToString(), _currentVelocity.y);
            _animator.SetFloat(AnimationType.VelocityZ.ToString(), _currentVelocity.z);

            // 速度段階
            _animator.SetFloat(AnimationType.Speed.ToString(), _speed);

            // 入力の有無
            _animator.SetBool(AnimationType.IsInputHorizontal.ToString(), _isInputHorizontal);

            // 地面への接触
            _animator.SetBool(AnimationType.IsOnGround.ToString(), _isOnGround);

            // 飛行フラグ
            _animator.SetBool(AnimationType.IsFlight.ToString(), _isFlight);

            // ロックオンフラグ
            _animator.SetBool(AnimationType.IsLockOn.ToString(), _isLockOn);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }

    public enum AnimationType
    {
        VelocityX,
        VelocityY,
        VelocityZ,
        Speed,
        Dodge,
        IsInputHorizontal,
        IsOnGround,
        IsFlight,
        IsLockOn
    }
}
