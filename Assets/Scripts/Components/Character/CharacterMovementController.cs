using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

namespace Components.Character
{
    public enum CharacterState
    {
        Default,
        Flight,
        DefaultDodge,
        FlightDodge
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion Rotation;
        public bool JumpDown;
        public bool JumpHeld;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool CrouchHeld;
        public bool EnableRun;
        public bool EnableLockOn;
        public bool EnableFlight;
        public bool DodgeDown;
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
        public bool IsRun;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    public class CharacterMovementController : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("地上での移動")]
        public float MaxStableMoveSpeed = 10f;
        public float MaxStableRunMoveSpeed = 15f;
        public float StableMovementSharpness = 10f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;

        [Header("空中での移動（飛行時を除く）")]
        public float MaxAirMoveSpeed = 10f;
        public float MaxAirRunMoveSpeed = 15f;
        public float AirAccelerationSpeed = 50f;
        public float Drag = 0.1f;

        [Header("ジャンプ")]
        public bool AllowJumpingWhenSliding = false;
        public bool AllowDoubleJump = true;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("飛行")]
        public float FlightMoveSpeed = 10f;
        public float FlightRunMoveSpeed = 15f;
        public float FlightSharpness = 15;

        [Header("回避")]
        public float DefaultDodgeSpeed = 25f;
        public float FlightDodgeSpeed = 35f;
        public float MaxDodgeTime = 0.125f;
        public float StoppedTime = 0.125f;

        [Header("その他")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public float CrouchedCapsuleHeight = 1f;
        public bool OrientTowardsGravity = false;

        public CharacterState CurrentCharacterState { get; private set; }

        // ローカル
        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _doubleJumpConsumed = false;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private int _keepJumpingCount = 0;
        private bool _isRun = false;
        private bool _isLockOn = false;
        private bool _jumpInputIsHeld = false;
        private bool _crouchInputIsHeld = false;
        private Vector3 _currentChargeVelocity;
        private bool _isStopped;
        private bool _mustStopVelocity = false;
        private float _timeSinceStartedCharge = 0;
        private float _timeSinceStopped = 0;
        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;

        private void Awake()
        {
            // motorにcharacterControllerを割り当てる
            Motor.CharacterController = this;

            // 初期状態を処理する
            TransitionToState(CharacterState.Default);
        }

        /// <summary>
        /// PlayerInputControllerによって毎フレーム呼び出され、キャラクターに入力内容を伝える
        /// </summary>
        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// 状態に入ったときのイベント
        /// </summary>
        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        Motor.SetGroundSolvingActivation(true);
                        break;
                    }
                case CharacterState.Flight:
                    {
                        Motor.SetGroundSolvingActivation(false);
                        break;
                    }
                case CharacterState.DefaultDodge:
                    {
                        _isStopped = false;
                        _timeSinceStartedCharge = 0f;
                        _timeSinceStopped = 0f;
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        _isStopped = false;
                        _timeSinceStartedCharge = 0f;
                        _timeSinceStopped = 0f;
                        break;
                    }
            }
        }

        /// <summary>
        /// 状態から抜けるときのイベント
        /// </summary>
        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Flight:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// PlayerInputControllerによって毎フレーム呼び出され、キャラクターに入力内容を伝える
        /// </summary>
        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            // 同時入力が許されないインプットのキャンセル
            if (inputs.DodgeDown && inputs.EnableFlight) inputs.EnableFlight = false;

            // 特定のStateの場合はインプットをキャンセル
            if (CurrentCharacterState == CharacterState.DefaultDodge || CurrentCharacterState == CharacterState.FlightDodge) inputs.EnableFlight = false;

            // flightモード
            if (inputs.EnableFlight)
            {
                if (CurrentCharacterState == CharacterState.Default) TransitionToState(CharacterState.Flight);
                else if (CurrentCharacterState == CharacterState.Flight) TransitionToState(CharacterState.Default);
            }

            // Clamp入力
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            // 平面に沿ったベクトルを計算
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.Rotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.Rotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            _jumpInputIsHeld = inputs.JumpHeld;
            _crouchInputIsHeld = inputs.CrouchHeld;
            _isRun = inputs.EnableRun;
            _isLockOn = inputs.EnableLockOn;

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // Move and look inputs
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        // ロックオン時はTowardsCameraと同様の計算
                        if (_isLockOn) _lookInputVector = cameraPlanarDirection;
                        else
                        {
                            switch (OrientationMethod)
                            {
                                case OrientationMethod.TowardsCamera:
                                    _lookInputVector = cameraPlanarDirection;
                                    break;
                                case OrientationMethod.TowardsMovement:
                                    _lookInputVector = _moveInputVector.normalized;
                                    break;
                            }
                        }

                        // Jumping input
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                        }

                        // Crouching input
                        if (inputs.CrouchDown)
                        {
                            _shouldBeCrouching = true;

                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                                MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                            }
                        }
                        else if (inputs.CrouchUp)
                        {
                            _shouldBeCrouching = false;
                        }

                        break;
                    }
                case CharacterState.Flight:
                    {
                        // ロックオン時はTowardsCameraと同様の計算
                        if (_isLockOn)
                        {
                            _lookInputVector = cameraPlanarDirection;
                            _moveInputVector = inputs.Rotation * moveInputVector;
                        }
                        else
                        {
                            switch (OrientationMethod)
                            {
                                case OrientationMethod.TowardsCamera:
                                    _moveInputVector = inputs.Rotation * moveInputVector;
                                    _lookInputVector = cameraPlanarDirection;
                                    break;
                                case OrientationMethod.TowardsMovement:
                                    var yAxisRotation = Quaternion.Euler(new Vector3(0, inputs.Rotation.eulerAngles.y, 0));
                                    _moveInputVector = yAxisRotation * moveInputVector;
                                    _lookInputVector = _moveInputVector.normalized;
                                    break;
                            }
                        }
                        break;
                    }
                case CharacterState.DefaultDodge:
                    {
                        // 回避中の入力
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        // 回避中の入力
                        _moveInputVector = cameraPlanarRotation * moveInputVector;
                        break;
                    }
            }

            // 回避モードは以下の条件出なければ切り替わらない
            // 飛行モード有効の入力と同時に実行できない
            // 前後左右・上昇下降の入力を同時にしないと実行できない
            if (inputs.DodgeDown && !inputs.EnableFlight && (inputs.MoveAxisForward != 0.000 || inputs.MoveAxisRight != 0.000 || inputs.JumpHeld || inputs.CrouchHeld))
            {
                if (CurrentCharacterState == CharacterState.Default) TransitionToState(CharacterState.DefaultDodge);
                else if (CurrentCharacterState == CharacterState.Flight) TransitionToState(CharacterState.FlightDodge);
            }
        }

        /// <summary>
        /// PlayerInputControllerによって毎フレーム呼び出され、キャラクターに入力内容を伝える
        /// </summary>
        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
            _isRun = inputs.IsRun;
        }

        private Quaternion _tmpTransientRot;

        /// <summary>
        /// (KinematicCharacterMotorの更新サイクル中に呼び出される)
        /// キャラクタが移動の更新を開始する前に呼び出される
        /// </summary>
        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DefaultDodge:
                    {
                        _timeSinceStartedCharge += deltaTime;
                        if (_isStopped)
                        {
                            _timeSinceStopped += deltaTime;
                        }
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        _timeSinceStartedCharge += deltaTime;
                        if (_isStopped)
                        {
                            _timeSinceStopped += deltaTime;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// （KinematicCharacterMotorの更新サイクル中に呼び出される）
        /// キャラクターの回転を設定
        /// キャラクタの回転を設定するのはここのみ
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
            {
                // 現在のルック方向からターゲット・ルック方向へのスムーズな補間
                Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                // 現在の回転を設定 (KinematicCharacterMotorで使用される)
                currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
            }

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            // 現在のルック方向からターゲット・ルック方向へのスムーズな補間
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // 現在の回転を設定 (KinematicCharacterMotorで使用される)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            // 重力を反転させるため、現在より回転を上げる
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // ピボットを中心とした回転ではなく、ボトムのヘミセンターを中心とした回転を作るためにポジションを移動させる
                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }
                case CharacterState.Flight:
                    {
                        if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                        {
                            //  現在のルック方向からターゲット・ルック方向へのスムーズな補間
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            // 現在の回転を設定する (KinematicCharacterMotorによって使用される)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }
                        if (OrientTowardsGravity)
                        {
                            // 重力を反転させるため、現在より回転を上げる
                            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// （KinematicCharacterMotorの更新サイクル中に呼び出される）
        /// キャラクターの速度を指示
        /// キャラクタの速度を設定できるのはここのみ
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // 安定した地面
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                            // 斜面での速度の再調整
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                            // 対象のVelocity計算
                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            Vector3 targetMovementVelocity;
                            if (_isRun) targetMovementVelocity = reorientedInput * MaxStableRunMoveSpeed;
                            else targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                            // Velocityでの移動
                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        // 空中移動
                        else
                        {
                            // 移動入力
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                // 入力に基づく空中での速度を制限する
                                if (_isRun)
                                {
                                    if (currentVelocityOnInputsPlane.magnitude < MaxAirRunMoveSpeed)
                                    {
                                        // 入力面上の最大速度を超えないようにaddedVelをクランプする
                                        Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirRunMoveSpeed);
                                        addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                    }
                                    else
                                    {
                                        // 既に超過している速度の方向にaddedVelが向かわないようにする
                                        if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                        {
                                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                        }
                                    }
                                }
                                else
                                {
                                    if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                    {
                                        // 入力面上の最大速度を超えないようにaddedVelをクランプする
                                        Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                        addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                    }
                                    else
                                    {
                                        // 既に超過している速度の方向にaddedVelが向かわないようにする
                                        if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                        {
                                            addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                        }
                                    }
                                }

                                // 斜面の壁を空中で登るのを防ぐ
                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                // 加速度を適用する
                                currentVelocity += addedVelocity;
                            }

                            // 重力
                            currentVelocity += Gravity * deltaTime;

                            // ドラッグ
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }

                        // ジャンプ制御
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;

                        if (!_jumpRequested)
                        {
                            _keepJumpingCount = 0;
                        }

                        if (_jumpRequested)
                        {
                            // ダブルジャンプ制御
                            if (AllowDoubleJump && _keepJumpingCount == 0)
                            {
                                _keepJumpingCount += 1;
                                if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                                {
                                    Motor.ForceUnground(0.1f);

                                    // 戻り速度に加え、ジャンプ状態をリセットする
                                    currentVelocity += (Motor.CharacterUp * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                    _jumpRequested = false;
                                    _doubleJumpConsumed = true;
                                    _jumpedThisFrame = true;
                                }
                            }

                            // 実際にジャンプが許可されているか確認する
                            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                            {
                                // 地面を離れる前にジャンプの方向を計算する
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }

                                // 次の更新時にキャラクターが地面の検出/スナップをスキップするようにする。
                                // この行がなければ、キャラクターはジャンプしようとする際に地面にスナップされたままになる。試しにこの行をコメントアウトしてみてください。
                                Motor.ForceUnground();

                                // 返される速度に加算し、ジャンプの状態をリセットする
                                currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }

                        // 加算される速度を考慮する
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
                case CharacterState.Flight:
                    {
                        float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f) + (_crouchInputIsHeld ? -1f : 0f);
                        // 目標速度へのスムーズな補間
                        Vector3 targetMovementVelocity = Vector3.zero;
                        if (_isRun) targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * FlightRunMoveSpeed;
                        else targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * FlightMoveSpeed;
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-FlightSharpness * deltaTime));
                        break;
                    }
                case CharacterState.DefaultDodge:
                    {
                        // 停止して速度をキャンセルする必要がある場合は、ここで行う
                        if (_mustStopVelocity)
                        {
                            // currentVelocity = Vector3.zero;
                            // _mustStopVelocity = false;
                        }

                        if (_isStopped)
                        {
                            // 停止しているときは、重力以外の速度操作は行わない
                            currentVelocity += Gravity * deltaTime;
                        }
                        else
                        {
                            // 速度は常に一定
                            // float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f);
                            currentVelocity = _moveInputVector.normalized * DefaultDodgeSpeed;
                            currentVelocity += Gravity * deltaTime;
                        }
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        // 停止して速度をキャンセルする必要がある場合は、ここで行う
                        if (_mustStopVelocity)
                        {
                            // currentVelocity = Vector3.zero;
                            // _mustStopVelocity = false;
                        }

                        if (_isStopped)
                        {
                        }
                        else
                        {
                            // 速度は常に一定
                            float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f) + (_crouchInputIsHeld ? -1f : 0f);
                            currentVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * FlightDodgeSpeed;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// (KinematicCharacterMotorの更新サイクル中に呼び出される)
        /// キャラクターの移動更新が完了した後に呼び出される
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        // ジャンプ関連の値を処理する
                        {
                            // 地面に近い状態でのジャンプ猶予期間を処理する
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                            {
                                // 地面の上にいる場合、ジャンプ関連の値をリセットする
                                if (!_jumpedThisFrame)
                                {
                                    _doubleJumpConsumed = false;
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                // ジャンプ可能だった最後の時間を追跡する（猶予期間のため）
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        // しゃがみ解除の処理
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // キャラクターの立った状態の高さでオーバーラップテストを行い、障害物があるか確認する
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                // 障害物がある場合は、しゃがんだ状態の寸法に戻す
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            }
                            else
                            {
                                // 障害物がない場合は、しゃがみを解除する
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }
                case CharacterState.DefaultDodge:
                    {
                        // 経過時間による停止を検出
                        if (!_isStopped && _timeSinceStartedCharge > MaxDodgeTime)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }

                        // 停止フェーズの終了を検出し、デフォルトの動作状態に戻る
                        if (_timeSinceStopped > StoppedTime)
                        {
                            TransitionToState(CharacterState.Default);
                        }
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        // 経過時間による停止を検出
                        if (!_isStopped && _timeSinceStartedCharge > MaxDodgeTime)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }

                        // 停止フェーズの終了を検出し、飛行の動作状態に戻る
                        if (_timeSinceStopped > StoppedTime)
                        {
                            TransitionToState(CharacterState.Flight);
                        }
                        break;
                    }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.DefaultDodge:
                    {
                        // 障害物による停止を検知
                        if (!_isStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }
                        break;
                    }
                case CharacterState.FlightDodge:
                    {
                        // 障害物による停止を検知
                        if (!_isStopped && !hitStabilityReport.IsStable && Vector3.Dot(-hitNormal, _currentChargeVelocity.normalized) > 0.5f)
                        {
                            _mustStopVelocity = true;
                            _isStopped = true;
                        }
                        break;
                    }
            }
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        protected void OnLanded()
        {
        }

        protected void OnLeaveStableGround()
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }
    }
}