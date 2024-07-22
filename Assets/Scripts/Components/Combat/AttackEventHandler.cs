using System;
using UnityEngine;
using UnityEngine.Playables;

namespace Components.Combat
{
    public class AttackEventHandler : MonoBehaviour
    {
        public PlayableDirector PlayableDirector;

        public Action OnEnableComb = null;
        public Action OnCheckComb = null;
        public Action OnFinishAttack = null;

        private void Start()
        {
            PlayableDirector.stopped += Finish;
        }

        public void EnableComb()
        {
            OnEnableComb?.Invoke();
        }

        public void CheckComb()
        {
            OnCheckComb?.Invoke();
        }

        public void FinishAttack()
        {
            OnFinishAttack?.Invoke();
        }

        private void Finish(PlayableDirector aDirector)
        {
            if (PlayableDirector != aDirector) return;
            PlayableDirector.stopped -= Finish;
            OnEnableComb = null;
            OnCheckComb = null;
            OnFinishAttack = null;
            Destroy(this.gameObject);
        }
    }
}
