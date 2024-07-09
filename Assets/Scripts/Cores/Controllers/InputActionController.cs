using UnityEngine;
using UnityEngine.InputSystem;
using Cores.Models.Interfaces;
using Zenject;

namespace Cores.Controllers
{
    public class InputActionController : MonoBehaviour
    {
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

        public void OnJump(InputValue value)
        {
            inputSystemModel.SetJump(value.isPressed);
        }

        public void OnLook(InputValue value)
        {
            inputSystemModel.SetLook(value.Get<Vector2>());
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
    }
}
