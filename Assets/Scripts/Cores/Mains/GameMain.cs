using System;
using UnityEngine;
using Zenject;

namespace Cores.Mains
{
    [DefaultExecutionOrder(999)]
    public class GameMain : MonoBehaviour
    {
        [Inject]
        private void construct
        (
        )
        {
        }

        private async void Awake()
        {
            try
            {
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


