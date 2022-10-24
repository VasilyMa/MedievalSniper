using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    sealed class PullingStrengthBar : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Pullable>> _pullableFilter = default;

        readonly EcsPoolInject<Pullable> _pullablePool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var pullableEntity in _pullableFilter.Value)
            {
                ref var pullableComponent = ref _pullablePool.Value.Get(pullableEntity);

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                //interfaceComponent.StrengthBar.value = pullableComponent.CurrentStrength;
                interfaceComponent.StrengthBarMB.UpdateSlider(pullableComponent.CurrentStrength);
            }
        }
    }
}