using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Cores.Models.Interfaces;
using UniRx;
using Components.Character;

namespace Components.Gimmick
{
    public class Teleporter : MonoBehaviour
    {
        public Teleporter TeleportTo;
        public UnityAction<CharacterMovementController> OnCharacterTeleport;

        private IPlayerCharacterModel _playerCharacterModel;
        private bool _isBeingTeleportedTo { get; set; }

        private CompositeDisposable _disposables = new CompositeDisposable();

        [Inject]
        public void Construct(IPlayerCharacterModel playerCharacterModel)
        {
            _playerCharacterModel = playerCharacterModel;
        }

        void Start()
        {
            _playerCharacterModel.CharacterModel.Subscribe(character =>
            {

            }).AddTo(_disposables);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isBeingTeleportedTo)
            {
                CharacterMovementController cc = other.GetComponent<CharacterMovementController>();
                if (cc)
                {
                    cc.Motor.SetPositionAndRotation(TeleportTo.transform.position, TeleportTo.transform.rotation);

                    if (OnCharacterTeleport != null)
                    {
                        OnCharacterTeleport(cc);
                    }
                    TeleportTo._isBeingTeleportedTo = true;
                }
            }

            _isBeingTeleportedTo = false;
        }
    }
}