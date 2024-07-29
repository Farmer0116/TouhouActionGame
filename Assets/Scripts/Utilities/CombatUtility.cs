using System;
using Components.Combat;
using UnityEngine;
using UnityEngine.Playables;

namespace Utilities
{
    public static class CombatUtility
    {
        public static LayerMask PlayerToEnemyMask { get { return LayerMask.GetMask(_playerToEnemyMask); } }
        private static string[] _playerToEnemyMask = { "Default", "Enemy" };

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

        /// <summary>
        /// コンボ有効時イベントのセット
        /// /// </summary>
        /// <param name="root"></param>
        /// <param name="enableCombAction"></param>
        public static void SetEnableComboEvent(GameObject root, Action enableCombAction)
        {
            var attackEventHandler = root.GetComponent<AttackEventHandler>();
            if (attackEventHandler == null)
            {
                Debug.LogWarning("AttackEventHandlerがアタッチされていません。");
                return;
            }
            attackEventHandler.OnEnableComb += enableCombAction;
        }

        /// <summary>
        /// コンボチェック時イベントのセット
        /// </summary>
        /// <param name="root"></param>
        /// <param name="checkCombAction"></param>
        public static void SetCheckComboEvent(GameObject root, Action checkCombAction)
        {
            var attackEventHandler = root.GetComponent<AttackEventHandler>();
            if (attackEventHandler == null)
            {
                Debug.LogWarning("AttackEventHandlerがアタッチされていません。");
                return;
            }
            attackEventHandler.OnCheckComb += checkCombAction;
        }

        /// <summary>
        /// 攻撃終了時イベントのセット
        /// </summary>
        /// <param name="root"></param>
        /// <param name="finishAttackAction"></param>
        public static void SetFinishAttackEvent(GameObject root, Action finishAttackAction)
        {
            var attackEventHandler = root.GetComponent<AttackEventHandler>();
            if (attackEventHandler == null)
            {
                Debug.LogWarning("AttackEventHandlerがアタッチされていません。");
                return;
            }
            attackEventHandler.OnFinishAttack += finishAttackAction;
        }

        /// <summary>
        /// コンボチェック時イベントの削除
        /// </summary>
        /// <param name="root"></param>
        /// <param name="checkCombAction"></param>
        public static void RemoveCheckComboEvent(GameObject root, Action checkCombAction)
        {
            var attackEventHandler = root.GetComponent<AttackEventHandler>();
            if (attackEventHandler == null)
            {
                Debug.LogWarning("AttackEventHandlerがアタッチされていません。");
                return;
            }
            attackEventHandler.OnCheckComb -= checkCombAction;
        }

        /// <summary>
        /// 攻撃終了時イベントの削除
        /// </summary>
        /// <param name="root"></param>
        /// <param name="finishAttackAction"></param>
        public static void RemoveFinishAttackEvent(GameObject root, Action finishAttackAction)
        {
            var attackEventHandler = root.GetComponent<AttackEventHandler>();
            if (attackEventHandler == null)
            {
                Debug.LogWarning("AttackEventHandlerがアタッチされていません。");
                return;
            }
            attackEventHandler.OnFinishAttack -= finishAttackAction;
        }
    }
}
