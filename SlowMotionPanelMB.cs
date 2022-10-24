using Leopotam.EcsLite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client 
{
    public class SlowMotionPanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        [SerializeField] private ParticleSystem ParticleSystem;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
        }
        public void SetParticle(float value)
        {
            var emission = ParticleSystem.emission;
            emission.rateOverTime = value * 50;
        }
        public GameObject GetPaticles()
        {
            return ParticleSystem.gameObject;
        }
    }
}
