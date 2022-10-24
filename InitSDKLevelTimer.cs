using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client 
{
    sealed class InitSDKLevelTimer : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsPoolInject<LevelTimer> _levelTimerPool = default;
        public void Init (IEcsSystems systems) 
        {
            var timerEntity = _world.Value.NewEntity();

            ref var levelTimerComponent = ref _levelTimerPool.Value.Add(timerEntity);
            levelTimerComponent.CurrentValue = 0;

            _gameState.Value.LevelTimerEntity = timerEntity;

        }
    }
}