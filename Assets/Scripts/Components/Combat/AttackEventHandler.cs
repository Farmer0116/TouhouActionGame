using UnityEngine;
using UnityEngine.Playables;

namespace Components.Combat
{
    public class AttackEventHandler : MonoBehaviour
    {
        public PlayableDirector PlayableDirector;

        private void Start()
        {
            PlayableDirector.stopped += Finish;
        }

        private void Finish(PlayableDirector aDirector)
        {
            if (PlayableDirector != aDirector) return;
            Destroy(this.gameObject);
        }
    }
}
