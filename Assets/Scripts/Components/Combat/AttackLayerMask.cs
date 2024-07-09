using System.Collections.Generic;
using UnityEngine;

namespace Components.Combat
{
    public class AttackLayerMask : MonoBehaviour
    {
        public List<ParticleSystem> particleSystems;

        public void SetLayer(LayerMask layerMask)
        {
            foreach (var particleSystem in particleSystems)
            {
                var collision = particleSystem.collision;
                collision.collidesWith = layerMask;
            }
        }
    }
}
