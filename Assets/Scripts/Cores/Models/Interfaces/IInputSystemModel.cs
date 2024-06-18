using UnityEngine;
using UniRx;

namespace Cores.Models.Interfaces
{
    public interface IInputSystemModel
    {
        IReadOnlyReactiveProperty<Vector2> MoveRP { get; }
        IReadOnlyReactiveProperty<bool> JumpRP { get; }
        IReadOnlyReactiveProperty<Vector2> LookRP { get; }
        IReadOnlyReactiveProperty<bool> LightAttackRP { get; }
        IReadOnlyReactiveProperty<bool> SneakRP { get; }
        IReadOnlyReactiveProperty<bool> RunRP { get; }

        void SetMoveRP(Vector2 value);
        void SetJumpRP(bool value);
        void SetLookRP(Vector2 value);
        void SetLightAttackRP(bool value);
        void SetSneakRP(bool value);
        void SetRunRP(bool value);
    }
}
