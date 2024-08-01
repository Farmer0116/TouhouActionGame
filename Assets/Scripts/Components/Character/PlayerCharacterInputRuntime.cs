using UnityEngine;
using Zenject;
using Cores.Models.Interfaces;

namespace Components.Character
{
    public class PlayerCharacterInputRuntime : MonoBehaviour
    {
        public CharacterMovementController CharacterMovementController { get { return _characterMovementController; } private set { _characterMovementController = value; } }
        [SerializeField] private CharacterMovementController _characterMovementController;
        public Transform CharacterRotationTarget { get; private set; }

        private IInputSystemModel _inputSystemModel;
        private CharacterModelComponent _characterModelComponent;
        private Vector3 _characterFrontVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

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
            CharacterRotationTarget = new GameObject(_characterRotationTargetName).transform;
            CharacterRotationTarget.rotation = characterModelComponent.Center.rotation;
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
        }

        void Update()
        {
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

        private void HandleCharacterInput()
        {
            Components.Character.PlayerCharacterInputs characterInputs = new Components.Character.PlayerCharacterInputs();

            if (_characterModelComponent.CharacterModel.IsLockOn && _characterModelComponent.CharacterModel.LockOnTarget != null)
            {
                CharacterRotationTarget.position = _characterModelComponent.EyeLevel.position;
                CharacterRotationTarget.LookAt(_characterModelComponent.CharacterModel.LockOnTarget);
                characterInputs.Rotation = CharacterRotationTarget.rotation;

                _characterFrontVector = CharacterRotationTarget.rotation.eulerAngles;
                characterInputs.IsLockOn = _characterModelComponent.CharacterModel.IsLockOn;
            }
            else
            {
                CharacterRotationTarget.position = _characterModelComponent.EyeLevel.position;
                _characterFrontVector.y += _inputSystemModel.Look.Value.x;
                _characterFrontVector.x += _inputSystemModel.Look.Value.y;
                _characterFrontVector.x = Mathf.Clamp(_characterFrontVector.x, _minViewField, _maxViewField);
                CharacterRotationTarget.rotation = Quaternion.Euler(new Vector3(_characterFrontVector.x, _characterFrontVector.y, 0));
                characterInputs.Rotation = CharacterRotationTarget.rotation;
            }

            // モデルに回転情報を共有
            _characterModelComponent.HeadRotation = CharacterRotationTarget.rotation;

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