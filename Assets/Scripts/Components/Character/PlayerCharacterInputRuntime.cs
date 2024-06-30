using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Zenject;
using Cores.Models.Interfaces;
using static Zenject.ZenAutoInjecter;

namespace Components.Character
{
    public class PlayerCharacterInputRuntime : MonoBehaviour
    {
        public CharacterMovementController CharacterMovementController { get { return _characterMovementController; } private set { _characterMovementController = value; } }
        [SerializeField] private CharacterMovementController _characterMovementController;
        public Transform RotateTarget { get { return _rotateTarget; } private set { _rotateTarget = value; } }
        [SerializeField] private Transform _rotateTarget;
        public Transform LookTarget { get { return _lookTarget; } }
        [SerializeField] private Transform _lookTarget;

        // public ExampleCharacterCamera CharacterCamera;

        private IInputSystemModel _inputSystemModel;
        private Vector3 _lookCharacterVector = Vector3.zero;
        private const float maxViewField = 90f;
        private const float minViewField = -90f;

        private const string _rotateTargetName = "RotateTarget";

        [Inject]
        private void construct(
            IInputSystemModel inputSystemModel
        )
        {
            _inputSystemModel = inputSystemModel;
            var injecter = GetComponent<ZenAutoInjecter>();
        }

        public void Initialize(CharacterMovementController characterMovementController)
        {
            CharacterMovementController = characterMovementController;
            RotateTarget = new GameObject(_rotateTargetName).transform;
            if (characterMovementController.CameraTarget != null) _lookTarget = characterMovementController.CameraTarget;
            else Debug.LogError("キャラクターのカメラのフォロー対象が設定されていません");
        }

        void Awake()
        {
            if (_inputSystemModel == null)
            {
                this.gameObject.AddComponent<ZenAutoInjecter>();
            }
        }

        void Start()
        {
            // Cursor.lockState = CursorLockMode.Locked;

            // // Tell camera to follow transform
            // CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // // Ignore the character's collider(s) for camera obstruction checks
            // CharacterCamera.IgnoredColliders.Clear();
            // CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        void Update()
        {
            // if (Input.GetMouseButtonDown(0))
            // {
            //     Cursor.lockState = CursorLockMode.Locked;
            // }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // // Handle rotating the camera along with physics movers
            // if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            // {
            //     CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            //     CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            // }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // // Create the look input vector for the camera
            // float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            // float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            // Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // // Prevent moving the camera while the cursor isn't locked
            // if (Cursor.lockState != CursorLockMode.Locked)
            // {
            //     lookInputVector = Vector3.zero;
            // }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            // float scrollInput = -Input.GetAxis(MouseScrollInput);
            // #if UNITY_WEBGL
            //         scrollInput = 0f;
            // #endif

            // // Apply inputs to the camera
            // CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // // Handle toggling zoom level
            // if (Input.GetMouseButtonDown(1))
            // {
            //     CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 0f) ? CharacterCamera.DefaultDistance : 0f;
            // }
        }

        private void HandleCharacterInput()
        {
            Components.Character.PlayerCharacterInputs characterInputs = new Components.Character.PlayerCharacterInputs();

            RotateTarget.localPosition = transform.position;

            _lookCharacterVector.y += _inputSystemModel.Look.Value.x;
            _lookCharacterVector.x += _inputSystemModel.Look.Value.y;
            _lookCharacterVector.x = Mathf.Clamp(_lookCharacterVector.x, minViewField, maxViewField);

            RotateTarget.localRotation = Quaternion.Euler(new Vector3(_lookCharacterVector.x, _lookCharacterVector.y, 0));
            characterInputs.Rotation = RotateTarget.rotation;

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = _inputSystemModel.Move.Value.y;
            characterInputs.MoveAxisRight = _inputSystemModel.Move.Value.x;
            characterInputs.JumpDown = _inputSystemModel.Jump.Value;
            // characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            // characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            // Apply inputs to character
            CharacterMovementController.SetInputs(ref characterInputs);
        }
    }
}