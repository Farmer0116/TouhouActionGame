using Cinemachine;
using Cores.Models.Interfaces;
using UniRx;

namespace Cores.Models
{
    public class PlayerCameraModel : IPlayerCameraModel
    {
        public CinemachineVirtualCamera CurrentCinemachineVirtualCamera { get { return _currentCinemachineVirtualCamera; } }
        private CinemachineVirtualCamera _currentCinemachineVirtualCamera;

        public Subject<CinemachineVirtualCamera> OnChangeVirtualCamera => _onChangeVirtualCamera;
        private Subject<CinemachineVirtualCamera> _onChangeVirtualCamera = new Subject<CinemachineVirtualCamera>();

        public PlayerCameraModel() { }

        public void SetCurrentCamera(CinemachineVirtualCamera cinemachineVirtualCamera)
        {
            _currentCinemachineVirtualCamera = cinemachineVirtualCamera;
        }
    }
}