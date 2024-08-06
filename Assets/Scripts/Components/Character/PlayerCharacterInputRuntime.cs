using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;
using UniRx;

namespace Components.Character
{
    public class PlayerCharacterInputRuntime : MonoBehaviour
    {
        public CharacterMovementController CharacterMovementController { get; private set; }
        private Transform _characterRotationTarget;

        private IInputSystemModel _inputSystemModel;
        private CharacterModelComponent _characterModelComponent;
        private Vector3 _characterFrontVector = Vector3.zero;
        private (Vector2 move, Vector2 look, bool run, bool flight, bool jump, bool ascend, bool descend, bool dodge) _inputState;
        private ZenAutoInjecter _zenAutoInjecter;
        private CompositeDisposable _disposables = new CompositeDisposable();

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _characterRotationTargetName = "CharacterRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel
        )
        {
            _inputSystemModel = inputSystemModel;
        }

        public void Init(CharacterMovementController characterMovementController, CharacterModelComponent characterModelComponent)
        {
            CharacterMovementController = characterMovementController;
            _characterRotationTarget = new GameObject(_characterRotationTargetName).transform;
            _characterRotationTarget.rotation = characterModelComponent.Center.rotation;
            _characterModelComponent = characterModelComponent;
        }

        void Awake()
        {
            if (_inputSystemModel == null)
            {
                _zenAutoInjecter = gameObject.AddComponent<ZenAutoInjecter>();
            }
        }

        void Start()
        {
            if (_zenAutoInjecter != null)
            {
                Destroy(_zenAutoInjecter);
                _zenAutoInjecter = null;
            }

            SetUpInput();
            SetUpModelEvent();
        }

        void Update()
        {
            // OrientationMethodの変更をモデルコンポーネントに適応
            if (_characterModelComponent.OrientationMethod != CharacterMovementController.OrientationMethod)
            {
                switch (CharacterMovementController.OrientationMethod)
                {
                    case OrientationMethod.TowardsCamera:
                        _characterModelComponent.OrientationMethod = OrientationMethod.TowardsCamera;
                        break;
                    case OrientationMethod.TowardsMovement:
                        _characterModelComponent.OrientationMethod = OrientationMethod.TowardsMovement;
                        break;
                }
            }

            HandleCharacterInput();
        }

        /// <summary>
        /// 入力とモデルのイベントの紐づけ
        /// </summary>
        private void SetUpInput()
        {
            // 移動
            _inputSystemModel.Move.Subscribe(value =>
            {
                _inputState.move = value;
            }).AddTo(_disposables);

            // 回転
            _inputSystemModel.Look.Subscribe(value =>
            {
                _inputState.look = value;
            }).AddTo(_disposables);

            // 走る
            _inputSystemModel.Run.Subscribe(value =>
            {
                _inputState.run = value;
                if (value) _characterModelComponent.CharacterModel.EnableRun();
                else _characterModelComponent.CharacterModel.DisableRun();
            }).AddTo(_disposables);

            // 飛行
            _inputSystemModel.Flight.Where(flag => flag).Subscribe(flag =>
            {
                if (_characterModelComponent.CharacterModel.IsFlight)
                {
                    _inputState.flight = true;
                    _characterModelComponent.CharacterModel.DisableFlight();
                }
                else
                {
                    _inputState.flight = true;
                    _characterModelComponent.CharacterModel.EnableFlight();
                }
            }).AddTo(_disposables);

            // ジャンプ
            _inputSystemModel.Jump.Subscribe(value =>
            {
                _inputState.jump = value;
                if (value) _characterModelComponent.CharacterModel.Jump();
            }).AddTo(_disposables);

            // 上昇
            _inputSystemModel.Jump.Subscribe(value =>
            {
                _inputState.ascend = value;
                if (value) _characterModelComponent.CharacterModel.Ascend();
            }).AddTo(_disposables);

            // 下降
            _inputSystemModel.Crouch.Subscribe(value =>
            {
                _inputState.descend = value;
                if (value) _characterModelComponent.CharacterModel.Descend();
            }).AddTo(_disposables);

            // 回避
            _inputSystemModel.Dodge.Subscribe(value =>
            {
                _inputState.dodge = value;
                if (value) _characterModelComponent.CharacterModel.Dodge();
            }).AddTo(_disposables);
        }

        /// <summary>
        /// モデルのイベントと紐づけ
        /// </summary>
        private void SetUpModelEvent()
        {
            _characterModelComponent.CharacterModel.OnEnableFlight.Subscribe(_ =>
            {
            }).AddTo(_disposables);

            _characterModelComponent.CharacterModel.OnDisableFlight.Subscribe(_ =>
            {
            }).AddTo(_disposables);

            _characterModelComponent.CharacterModel.OnJump.Subscribe(_ =>
            {
            }).AddTo(_disposables);
        }

        private void HandleCharacterInput()
        {
            // Motorへの入力変数定義
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // ロックオン時の回転
            if (_characterModelComponent.CharacterModel.IsLockOn && _characterModelComponent.CharacterModel.LockOnTarget != null)
            {
                // モデルに登録されたのターゲットの方向を回転情報として入力
                _characterRotationTarget.position = _characterModelComponent.Center.position;
                _characterRotationTarget.LookAt(_characterModelComponent.CharacterModel.LockOnTarget);
                _characterFrontVector = _characterRotationTarget.rotation.eulerAngles;

                characterInputs.Rotation = _characterRotationTarget.rotation;
                characterInputs.EnableLockOn = _characterModelComponent.CharacterModel.IsLockOn;
            }
            // 非ロックオン時の回転
            else
            {
                // 手動での回転入力
                _characterRotationTarget.position = _characterModelComponent.EyeLevel.position;
                _characterFrontVector.y += _inputState.look.x;
                _characterFrontVector.x += _inputState.look.y;
                _characterFrontVector.x = Mathf.Clamp(_characterFrontVector.x, _minViewField, _maxViewField);
                _characterRotationTarget.rotation = Quaternion.Euler(new Vector3(_characterFrontVector.x, _characterFrontVector.y, 0));

                characterInputs.Rotation = _characterRotationTarget.rotation;
            }

            // モデルコンポーネントに格納
            _characterModelComponent.HeadRotation = _characterRotationTarget.rotation;

            characterInputs.MoveAxisForward = _inputState.move.y;
            characterInputs.MoveAxisRight = _inputState.move.x;
            characterInputs.JumpDown = _inputState.jump;
            characterInputs.EnableRun = _inputState.run;
            if (_characterModelComponent.CharacterModel.IsFlight) characterInputs.JumpHeld = _inputState.ascend;
            if (_characterModelComponent.CharacterModel.IsFlight) characterInputs.CrouchHeld = _inputState.descend;
            if (_inputState.flight) // flightは1フレ内単入力
            {
                characterInputs.EnableFlight = true;
                _inputState.flight = false;
            }
            if (_inputState.dodge) // dodgeは1フレ内単入力
            {
                characterInputs.DodgeDown = true;
                _inputState.dodge = false;
            }

            // キャラクターへ入力情報のセット
            CharacterMovementController.SetInputs(ref characterInputs);
        }

        void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}