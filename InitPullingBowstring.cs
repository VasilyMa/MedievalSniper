using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitPullingBowstring : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<Pullable> _pullingBowstringPool = default;

        public void Init (IEcsSystems systems)
        {
            ref var pullingBowstringComponent = ref _pullingBowstringPool.Value.Add(_world.Value.NewEntity());

            pullingBowstringComponent.PullongUpCurve = _gameState.Value.PullingBowstringUpCurve;
            pullingBowstringComponent.PullongDownCurve = _gameState.Value.PullingBowstringDownCurve ?? _gameState.Value.PullingBowstringUpCurve;

            pullingBowstringComponent.CurrentStrength = 0;
            pullingBowstringComponent.isPullingUp = true;
        }
    }
}