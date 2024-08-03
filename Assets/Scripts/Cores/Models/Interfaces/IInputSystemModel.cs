using UnityEngine;
using UniRx;

namespace Cores.Models.Interfaces
{
    public interface IInputSystemModel
    {
        IReadOnlyReactiveProperty<Vector2> Move { get; }
        IReadOnlyReactiveProperty<bool> Jump { get; }
        IReadOnlyReactiveProperty<bool> Crouch { get; }
        IReadOnlyReactiveProperty<Vector2> Look { get; }
        IReadOnlyReactiveProperty<bool> LockOn { get; }
        IReadOnlyReactiveProperty<bool> Sneak { get; }
        IReadOnlyReactiveProperty<bool> Run { get; }
        IReadOnlyReactiveProperty<bool> NormalAttack { get; }
        IReadOnlyReactiveProperty<bool> MagicAttack { get; }
        IReadOnlyReactiveProperty<bool> Flight { get; }

        void SetMove(Vector2 value);
        void SetJump(bool value);
        void SetCrouch(bool value);
        void SetLook(Vector2 value);
        void SetLockOn(bool value);
        void SetSneak(bool value);
        void SetRun(bool value);
        void SetNormalAttack(bool value);
        void SetMagicAttack(bool value);
        void SetFlight(bool value);
    }
}
