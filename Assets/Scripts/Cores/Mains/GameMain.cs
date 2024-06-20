using System;
using Cores.UseCases.Interfaces;
using UnityEngine;
using Zenject;

namespace Cores.Mains
{
    [DefaultExecutionOrder(999)]
    public class GameMain : MonoBehaviour
    {
        private IPlayerCharacterControlUseCase _playerCharacterControlUseCase;

        [Inject]
        private void construct
        (
            IPlayerCharacterControlUseCase playerCharacterControlUseCase
        )
        {
            _playerCharacterControlUseCase = playerCharacterControlUseCase;
        }

        private async void Awake()
        {
            try
            {
                _playerCharacterControlUseCase.Begin();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private void Start()
        {
            // ========== 初期設定 ==========
        }

        private void OnDestroy()
        {
        }
    }
}


