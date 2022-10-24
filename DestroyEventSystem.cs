using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DestroyEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<DestroyEvent>> _destroyEventFilter = default;

        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<Destructible> _destructiblePool = default;
        readonly EcsPoolInject<Outlinable> _outlinablePool = default;

        readonly EcsPoolInject<Destroyed> _destroyedPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;
        private int _destroyedEntity = GameState.NULL_ENTITY;

        private float _standartExplosionForce = 1500;
        private float _spreadExplosionForce = 13500;

        private float _calculatedExplosionForce;

        private bool _firstWork = true;

        public void Run (IEcsSystems systems)
        {
            if (_firstWork)
            {
                CalculateExplosionForce();

                _firstWork = false;
            }

            foreach (var destroyEventEntity in _destroyEventFilter.Value)
            {
                _eventEntity = destroyEventEntity;

                ref var destroyEvent = ref _destroyEventPool.Value.Get(_eventEntity);
                _destroyedEntity = destroyEvent.DestroyedEntity;

                if (_destroyedPool.Value.Has(_destroyedEntity))
                {
                    DeleteEvent();
                    continue;
                }

                ref var viewComponent = ref _viewPool.Value.Get(_destroyedEntity);
                viewComponent.GameObject.layer = LayerMask.NameToLayer(nameof(Layers.Destroyed));

                ref var arrowViewComponent = ref _viewPool.Value.Get(_gameState.Value.ArrowEntity);

                ref var arrowComponent = ref _arrowPool.Value.Get(_gameState.Value.ArrowEntity);

                ref var destructibleComponent = ref _destructiblePool.Value.Get(_destroyedEntity);

                foreach (var rigidbodyPart in destructibleComponent.RigidbodyParts)
                {
                    rigidbodyPart.isKinematic = false;

                    rigidbodyPart.AddExplosionForce(_calculatedExplosionForce, arrowViewComponent.Model.transform.position, 0);

                    rigidbodyPart.gameObject.layer = LayerMask.NameToLayer(nameof(Layers.Destroyed));
                }

                ref var outlinableComponent = ref _outlinablePool.Value.Get(_destroyedEntity);
                outlinableComponent.Outline.enabled = false;

                _gameState.Value.EnvironmentOnLevel++;

                _destroyedPool.Value.Add(_destroyedEntity);

                foreach (var pools in _gameState.Value.ActivePools)
                {
                    if (pools is DestroyEffectPool)
                    {
                        var effect = pools.GetFromPool();

                        effect.transform.position = arrowComponent.Tip.transform.position;
                        effect.GetComponent<ParticleSystem>()?.Play();
                    }
                }

                DeleteEvent();
            }
        }

        private void CalculateExplosionForce()
        {
            _calculatedExplosionForce = _standartExplosionForce + Mathf.CeilToInt(_spreadExplosionForce * _gameState.Value.PullingStrength);
        }

        private void DeleteEvent()
        {
            _destroyEventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
            _destroyedEntity = GameState.NULL_ENTITY;
        }
    }
}