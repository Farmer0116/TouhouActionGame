using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;

namespace Components.Character
{
    public class PlayerCharacterInputRuntime : MonoBehaviour
    {
        public CharacterMovementController CharacterMovementController { get { return _characterMovementController; } private set { _characterMovementController = value; } }
        [SerializeField] private CharacterMovementController _characterMovementController;
        public Transform CharacterRotationTarget { get { return _characterRotationTarget; } private set { _characterRotationTarget = value; } }
        [SerializeField] private Transform _characterRotationTarget;

        private IInputSystemModel _inputSystemModel;
        private IPlayerCharacterModel _playerCharacterModel;
        private Vector3 _characterFrontVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _characterRotationTargetName = "CharacterRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            IPlayerCharacterModel playerCharacterModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _playerCharacterModel = playerCharacterModel;
        }

        public void Initialize(CharacterMovementController characterMovementController)
        {
            CharacterMovementController = characterMovementController;
            CharacterRotationTarget = new GameObject(_characterRotationTargetName).transform;
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
        }

        void Update()
        {
            if (_playerCharacterModel.OrientationMethod.ToString() != CharacterMovementController.OrientationMethod.ToString())
            {
                switch (CharacterMovementController.OrientationMethod)
                {
                    case OrientationMethod.TowardsCamera:
                        _playerCharacterModel.OrientationMethod = Cores.Models.Interfaces.OrientationMethod.TowardsCamera;
                        break;
                    case OrientationMethod.TowardsMovement:
                        _playerCharacterModel.OrientationMethod = Cores.Models.Interfaces.OrientationMethod.TowardsMovement;
                        break;
                }
            }
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            Components.Character.PlayerCharacterInputs characterInputs = new Components.Character.PlayerCharacterInputs();

            if (_playerCharacterModel.IsLockOn && _playerCharacterModel.LockOnTarget != null)
            {
                CharacterRotationTarget.position = CharacterMovementController.RotationTargetOffset.position;
                CharacterRotationTarget.LookAt(_playerCharacterModel.LockOnTarget);
                characterInputs.Rotation = CharacterRotationTarget.rotation;

                _characterFrontVector = CharacterRotationTarget.rotation.eulerAngles;
            }
            else
            {
                CharacterRotationTarget.position = CharacterMovementController.RotationTargetOffset.position;
                _characterFrontVector.y += _inputSystemModel.Look.Value.x;
                _characterFrontVector.x += _inputSystemModel.Look.Value.y;
                _characterFrontVector.x = Mathf.Clamp(_characterFrontVector.x, _minViewField, _maxViewField);
                CharacterRotationTarget.rotation = Quaternion.Euler(new Vector3(_characterFrontVector.x, _characterFrontVector.y, 0));
                characterInputs.Rotation = CharacterRotationTarget.rotation;
            }

            // モデルに回転情報を共有
            _playerCharacterModel.CharacterRotation = CharacterRotationTarget.rotation;

            // characterInputsへの代入
            characterInputs.MoveAxisForward = _inputSystemModel.Move.Value.y;
            characterInputs.MoveAxisRight = _inputSystemModel.Move.Value.x;
            characterInputs.JumpDown = _inputSystemModel.Jump.Value;
            characterInputs.IsRun = _inputSystemModel.Run.Value;

            // Apply inputs to character
            CharacterMovementController.SetInputs(ref characterInputs);
        }
    }
}