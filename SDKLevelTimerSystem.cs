using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class SDKLevelTimerSystem : IEcsRunSystem {
        readonly EcsFilterInject<Inc<LevelTimer>> _levelTimerFilter = default;

        readonly EcsPoolInject<LevelTimer> _levelTimerPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var levelTimerEntity in _levelTimerFilter.Value)
            {
                ref var levelTimerComponent = ref _levelTimerPool.Value.Get(levelTimerEntity);
                levelTimerComponent.CurrentValue += Time.unscaledDeltaTime;
            }
        }
    }
}