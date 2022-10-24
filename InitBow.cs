using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitBow : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<View> _viewPool = default;

        public void Init (IEcsSystems systems)
        {
            GameObject bowGameObject = null;

            bowGameObject = GameObject.FindObjectOfType<BowMB>()?.gameObject;
            

            if (bowGameObject == null)
            {
                return;
            }

            var bowEntity = _world.Value.NewEntity();

            ref var viewComponent = ref _viewPool.Value.Add(bowEntity);
            viewComponent.EntityNumber = bowEntity;
            viewComponent.GameObject = bowGameObject;
            viewComponent.Transform = bowGameObject.transform;
            viewComponent.LayerBeforeDeath = bowGameObject.layer;
            _gameState.Value.BowEntity = bowEntity;
        }
    }
}