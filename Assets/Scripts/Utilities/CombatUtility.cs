using Components.Combat;
using UnityEngine;
using UnityEngine.Playables;

namespace Utilities
{
    public static class CombatUtility
    {
        public static LayerMask PlayerToEnemyMask { get { return LayerMask.GetMask(_playerToEnemyMask); } }
        private static string[] _playerToEnemyMask = { "Default" };

        public static PlayableDirector SpawnAttack(PlayableDirector root, Vector3 position, Quaternion rotation, LayerMask mask, bool autoPlay)
        {
            var o = GameObject.Instantiate(root, position, rotation);

            // レイヤーマスク
            var attackLayerMask = o.GetComponent<AttackLayerMask>();
            if (attackLayerMask != null) attackLayerMask.SetLayer(mask);
            else Debug.LogWarning("AttackLayerMaskがアタッチされていませんでした。想定外の衝突を検知する可能性があります。");

            if (autoPlay) o.Play();
            return o;
        }
    }
}
