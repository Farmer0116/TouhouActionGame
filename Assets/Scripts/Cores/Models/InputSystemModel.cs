using UnityEngine;
using UniRx;
using Cores.Models.Interfaces;

namespace Cores.Models
{
    public class InputSystemModel : IInputSystemModel
    {
        public IReadOnlyReactiveProperty<Vector2> Move => move;
        public IReadOnlyReactiveProperty<bool> Jump => jump;
        public IReadOnlyReactiveProperty<bool> Crouch => crouch;
        public IReadOnlyReactiveProperty<Vector2> Look => look;
        public IReadOnlyReactiveProperty<bool> LockOn => lockOn;
        public IReadOnlyReactiveProperty<bool> LightAttack => lightAttack;
        public IReadOnlyReactiveProperty<bool> Sneak => sneak;
        public IReadOnlyReactiveProperty<bool> Run => run;
        public IReadOnlyReactiveProperty<bool> NormalAttack => normalAttack;
        public IReadOnlyReactiveProperty<bool> MagicAttack => magicAttack;
        public IReadOnlyReactiveProperty<bool> Flight => flight;

        private ReactiveProperty<Vector2> move;
        private ReactiveProperty<bool> jump;
        private ReactiveProperty<bool> crouch;
        private ReactiveProperty<Vector2> look;
        private ReactiveProperty<bool> lockOn;
        private ReactiveProperty<bool> lightAttack;
        private ReactiveProperty<bool> sneak;
        private ReactiveProperty<bool> run;
        private ReactiveProperty<bool> normalAttack;
        private ReactiveProperty<bool> magicAttack;
        private ReactiveProperty<bool> flight;

        public InputSystemModel()
        {
            move = new ReactiveProperty<Vector2>();
            jump = new ReactiveProperty<bool>();
            crouch = new ReactiveProperty<bool>();
            look = new ReactiveProperty<Vector2>();
            lockOn = new ReactiveProperty<bool>();
            lightAttack = new ReactiveProperty<bool>();
            sneak = new ReactiveProperty<bool>();
            run = new ReactiveProperty<bool>();
            normalAttack = new ReactiveProperty<bool>();
            magicAttack = new ReactiveProperty<bool>();
            flight = new ReactiveProperty<bool>();
        }

        public void SetMove(Vector2 value)
        {
            move.Value = value;
        }
        public void SetJump(bool value)
        {
            jump.Value = value;
        }
        public void SetCrouch(bool value)
        {
            crouch.Value = value;
        }
        public void SetLook(Vector2 value)
        {
            look.Value = value;
        }
        public void SetLockOn(bool value)
        {
            lockOn.Value = value;
        }
        public void SetLightAttack(bool value)
        {
            lightAttack.Value = value;
        }
        public void SetSneak(bool value)
        {
            sneak.Value = value;
        }
        public void SetRun(bool value)
        {
            run.Value = value;
        }
        public void SetNormalAttack(bool value)
        {
            normalAttack.Value = value;
        }
        public void SetMagicAttack(bool value)
        {
            magicAttack.Value = value;
        }
        public void SetFlight(bool value)
        {
            flight.Value = value;
        }
    }
}
