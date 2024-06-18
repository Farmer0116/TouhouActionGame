using UnityEngine;
using UnityEngine.InputSystem;
using Cores.Models.Interfaces;
using Zenject;

namespace Cores.Controllers
{
    public class InputActionController : MonoBehaviour
    {
        private IInputSystemModel inputSystemModel;

        [Inject]
        private void construct(IInputSystemModel inputSystemModel)
        {
            this.inputSystemModel = inputSystemModel;
        }

        // Invoke Event
        public void OnMove(InputAction.CallbackContext context)
        {
            inputSystemModel.SetMoveRP(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            inputSystemModel.SetJumpRP(context.ReadValueAsButton());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            inputSystemModel.SetLookRP(context.ReadValue<Vector2>());
        }

        public void OnLightAttack(InputAction.CallbackContext context)
        {
            inputSystemModel.SetLightAttackRP(context.ReadValueAsButton());
        }

        public void OnSneak(InputAction.CallbackContext context)
        {
            inputSystemModel.SetSneakRP(context.ReadValueAsButton());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            inputSystemModel.SetRunRP(context.ReadValueAsButton());
        }
    }
}
