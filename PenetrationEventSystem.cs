using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PenetrationEventSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<PenetrationEvent>> _penetrationEventFilter = default;

        readonly EcsPoolInject<PenetrationEvent> _penetrationEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<Unmovable> _unmovablePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _penetrationEventFilter.Value)
            {
                _eventEntity = eventEntity;

                if (_unmovablePool.Value.Has(_gameState.Value.ArrowEntity))
                {
                    DeleteEvent();

                    continue;
                }

                _unmovablePool.Value.Add(_gameState.Value.ArrowEntity);

                ref var penetrationEvent = ref _penetrationEventPool.Value.Get(_eventEntity);

                ref var arrowViewComponent = ref _viewPool.Value.Get(_gameState.Value.ArrowEntity);
                arrowViewComponent.Model.transform.SetParent(penetrationEvent.PenetrationObject.transform);

                ref var arrowComponent = ref _arrowPool.Value.Get(_gameState.Value.ArrowEntity);
                arrowComponent.UsualTrail.Stop();
                arrowComponent.EpicTrail.Stop();

                ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);
                cameraComponent.ArrowVirtualCamera.Follow = null;

                PlayUndestructibleHitAudioEffect();

                DeleteEvent();
            }
        }

        private void PlayUndestructibleHitAudioEffect()
        {
            ref var audioComponent = ref _audioPool.Value.Get(_gameState.Value.ArrowEntity);

            audioComponent.AudioSource.pitch = Random.Range(0.9f, 1.1f);
            audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.Sniper.ArrowIsFailed);
        }

        private void DeleteEvent()
        {
            _penetrationEventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
        }
    }
}