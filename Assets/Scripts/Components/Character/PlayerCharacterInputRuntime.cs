using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Zenject;
using Cores.Models.Interfaces;
using static Zenject.ZenAutoInjecter;
using UnityEngine.UIElements.Experimental;
using UniRx;
using System.Linq;

namespace Components.Character
{
    public class PlayerCharacterInputRuntime : MonoBehaviour
    {
        public CharacterMovementController CharacterMovementController { get { return _characterMovementController; } private set { _characterMovementController = value; } }
        [SerializeField] private CharacterMovementController _characterMovementController;
        public Transform CameraRotationTarget { get { return _cameraRotationTarget; } private set { _cameraRotationTarget = value; } }
        [SerializeField] private Transform _cameraRotationTarget;
        public Transform CharacterRotationTarget { get { return _characterRotationTarget; } private set { _characterRotationTarget = value; } }
        [SerializeField] private Transform _characterRotationTarget;
        public Transform LookTarget { get { return _lookTarget; } }
        [SerializeField] private Transform _lookTarget;

        private IInputSystemModel _inputSystemModel;
        private ISpawningPlayerCharacterModel _spawningPlayerCharacterModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private ZenAutoInjecter _zenAutoInjecter;

        private const float _maxViewField = 89f;
        private const float _minViewField = -89f;
        private const string _cameraRotationTargetName = "CameraRotationTarget";
        private const string _characterRotationTargetName = "CharacterRotationTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel,
            ISpawningPlayerCharacterModel spawningPlayerCharacterModel
        )
        {
            _inputSystemModel = inputSystemModel;
            _spawningPlayerCharacterModel = spawningPlayerCharacterModel;
        }

        public void Initialize(CharacterMovementController characterMovementController)
        {
            CharacterMovementController = characterMovementController;
            CameraRotationTarget = new GameObject(_cameraRotationTargetName).transform;
            CharacterRotationTarget = new GameObject(_characterRotationTargetName).transform;
            if (characterMovementController.CameraTarget != null) _lookTarget = characterMovementController.CameraTarget;
            else Debug.LogError("キャラクターのカメラのフォロー対象が設定されていません");
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
            HandleCharacterInput();
        }

        private void HandleCharacterInput()
        {
            Components.Character.PlayerCharacterInputs characterInputs = new Components.Character.PlayerCharacterInputs();

            if (_spawningPlayerCharacterModel.IsLockOn.Value && _spawningPlayerCharacterModel.LockOnTarget.Value != null)
            {
                if (CharacterMovementController.OrientationMethod == OrientationMethod.TowardsCamera)
                {
                    CameraRotationTarget.localPosition = LookTarget.position;
                    CameraRotationTarget.LookAt(_spawningPlayerCharacterModel.LockOnTarget.Value);
                    characterInputs.Rotation = CameraRotationTarget.rotation;

                    characterInputs.IsLockOn = true;
                }
                else
                {
                    CameraRotationTarget.localPosition = LookTarget.position;
                    _lookCharacterVector.y += _inputSystemModel.Look.Value.x;
                    _lookCharacterVector.x += _inputSystemModel.Look.Value.y;
                    _lookCharacterVector.x = Mathf.Clamp(_lookCharacterVector.x, _minViewField, _maxViewField);
                    CameraRotationTarget.localRotation = Quaternion.Euler(new Vector3(_lookCharacterVector.x, _lookCharacterVector.y, 0));

                    CharacterRotationTarget.localPosition = LookTarget.position;
                    CharacterRotationTarget.LookAt(_spawningPlayerCharacterModel.LockOnTarget.Value);
                    characterInputs.Rotation = CharacterRotationTarget.rotation;

                    characterInputs.IsLockOn = true;
                }
            }
            else
            {
                // 回転
                CameraRotationTarget.localPosition = LookTarget.position;
                _lookCharacterVector.y += _inputSystemModel.Look.Value.x;
                _lookCharacterVector.x += _inputSystemModel.Look.Value.y;
                _lookCharacterVector.x = Mathf.Clamp(_lookCharacterVector.x, _minViewField, _maxViewField);
                CameraRotationTarget.localRotation = Quaternion.Euler(new Vector3(_lookCharacterVector.x, _lookCharacterVector.y, 0));
                characterInputs.Rotation = CameraRotationTarget.rotation;

                characterInputs.IsLockOn = false;
            }

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