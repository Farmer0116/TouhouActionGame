using UnityEngine;
using UniRx;
using Cores.Models.Interfaces;

namespace Cores.Models
{
    public class InputSystemModel : IInputSystemModel
    {
        public IReadOnlyReactiveProperty<Vector2> Move => move;
        public IReadOnlyReactiveProperty<bool> Jump => jump;
        public IReadOnlyReactiveProperty<Vector2> Look => look;
        public IReadOnlyReactiveProperty<bool> LightAttack => lightAttack;
        public IReadOnlyReactiveProperty<bool> Sneak => sneak;
        public IReadOnlyReactiveProperty<bool> Run => run;

        private ReactiveProperty<Vector2> move;
        private ReactiveProperty<bool> jump;
        private ReactiveProperty<Vector2> look;
        private ReactiveProperty<bool> lightAttack;
        private ReactiveProperty<bool> sneak;
        private ReactiveProperty<bool> run;

        public InputSystemModel()
        {
            move = new ReactiveProperty<Vector2>();
            jump = new ReactiveProperty<bool>();
            look = new ReactiveProperty<Vector2>();
            lightAttack = new ReactiveProperty<bool>();
            sneak = new ReactiveProperty<bool>();
            run = new ReactiveProperty<bool>();
        }

        public void SetMove(Vector2 value)
        {
            move.Value = value;
        }
        public void SetJump(bool value)
        {
            jump.Value = value;
        }
        public void SetLook(Vector2 value)
        {
            look.Value = value;
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
    }
}