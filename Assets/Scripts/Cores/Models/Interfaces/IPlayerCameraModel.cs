using Cinemachine;
using UniRx;

namespace Cores.Models.Interfaces
{
    public interface IPlayerCameraModel
    {
        CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get; }

        Subject<CinemachineVirtualCamera> OnChangeVirtualCamera { get; }

        void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera);
    }
}