using UnityEngine;
using UniRx;
using Cores.Models.Interfaces;

namespace Cores.Models
{
    public class InputSystemModel : IInputSystemModel
    {
        public IReadOnlyReactiveProperty<Vector2> MoveRP => moveRP;
        public IReadOnlyReactiveProperty<bool> JumpRP => jumpRP;
        public IReadOnlyReactiveProperty<Vector2> LookRP => lookRP;
        public IReadOnlyReactiveProperty<bool> LightAttackRP => lightAttackRP;
        public IReadOnlyReactiveProperty<bool> SneakRP => sneakRP;
        public IReadOnlyReactiveProperty<bool> RunRP => runRP;

        private ReactiveProperty<Vector2> moveRP;
        private ReactiveProperty<bool> jumpRP;
        private ReactiveProperty<Vector2> lookRP;
        private ReactiveProperty<bool> lightAttackRP;
        private ReactiveProperty<bool> sneakRP;
        private ReactiveProperty<bool> runRP;

        public InputSystemModel()
        {
            moveRP = new ReactiveProperty<Vector2>();
            jumpRP = new ReactiveProperty<bool>();
            lookRP = new ReactiveProperty<Vector2>();
            lightAttackRP = new ReactiveProperty<bool>();
            sneakRP = new ReactiveProperty<bool>();
            runRP = new ReactiveProperty<bool>();
        }

        public void SetMoveRP(Vector2 value)
        {
            moveRP.Value = value;
        }
        public void SetJumpRP(bool value)
        {
            jumpRP.Value = value;
        }
        public void SetLookRP(Vector2 value)
        {
            lookRP.Value = value;
        }
        public void SetLightAttackRP(bool value)
        {
            lightAttackRP.Value = value;
        }
        public void SetSneakRP(bool value)
        {
            sneakRP.Value = value;
        }
        public void SetRunRP(bool value)
        {
            runRP.Value = value;
        }
    }
}
