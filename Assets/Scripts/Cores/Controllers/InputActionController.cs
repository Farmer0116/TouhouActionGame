using UnityEngine;
using UnityEngine.InputSystem;
using Cores.Models.Interfaces;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

namespace Cores.Controllers
{
    public class InputActionController : MonoBehaviour
    {
        [SerializeField] private float stickRunThreshold = 0.9f;
        private IInputSystemModel inputSystemModel;

        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        [Inject]
        private void construct(IInputSystemModel inputSystemModel)
        {
            this.inputSystemModel = inputSystemModel;
        }

        public void OnMove(InputValue value)
        {
            inputSystemModel.SetMove(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            inputSystemModel.SetLook(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            inputSystemModel.SetJump(value.isPressed);
        }

        public void OnCrouch(InputValue value)
        {
            inputSystemModel.SetCrouch(value.isPressed);
        }

        public void OnLockOn(InputValue value)
        {
            inputSystemModel.SetLockOn(value.isPressed);
        }

        public void OnSneak(InputValue value)
        {
            inputSystemModel.SetSneak(value.isPressed);
        }

        public void OnRun(InputValue value)
        {
            inputSystemModel.SetRun(value.isPressed);
        }

        public void OnNormalAttack(InputValue value)
        {
            inputSystemModel.SetNormalAttack(value.isPressed);
        }

        public void OnMagicAttack(InputValue value)
        {
            inputSystemModel.SetMagicAttack(value.isPressed);
        }

        public void OnRunStick(InputValue value)
        {
            var input = value.Get<Vector2>();
            var distance = input.magnitude;
            if (distance > stickRunThreshold)
            {
                inputSystemModel.SetRun(true);
            }
            else inputSystemModel.SetRun(false);
        }

        public void OnFlight(InputValue value)
        {
            SetDelay(inputSystemModel.SetFlight);
        }

        public void OnDodge(InputValue value)
        {
            inputSystemModel.SetDodge(value.isPressed);
        }

        private async Task SetDelay(Action<bool> action)
        {
            action(true);
            await UniTask.DelayFrame(1);
            action(false);
        }
    }
}
