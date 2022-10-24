using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CollisionEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<CollisionEvent>> _collisionEventFilter = default;

        readonly EcsFilterInject<Inc<Killable>, Exc<Killed, Pullable>> _notKilledFilter = default;

        readonly EcsPoolInject<CollisionEvent> _collisionEventPool = default;
        readonly EcsPoolInject<KillTargetEvent> _killTargetEventPool = default;
        readonly EcsPoolInject<DestroyEvent> _destroyEventPool = default;

        readonly EcsPoolInject<VibrationEvent> _vibrationEventPool = default;

        readonly EcsPoolInject<PenetrationEvent> _penetrationEventPool = default;
        readonly EcsPoolInject<LoseEvent> _losePool = default;
        readonly EcsPoolInject<WinEvent> _winPool = default;
        readonly EcsPoolInject<StartComboEvent> _startComboEventPool = default;
        readonly EcsPoolInject<GiveScoreEvent> _giveScoreEventPool = default;

        readonly EcsPoolInject<Killable> _killablePool = default;
        readonly EcsPoolInject<Destructible> _destructiblePool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;
        readonly EcsPoolInject<GivableScores> _givableScoresPool = default;
        readonly EcsPoolInject<SniperComponent> _sniperPool = default;

        readonly EcsPoolInject<Killed> _killedPool = default;
        readonly EcsPoolInject<Destroyed> _destroyedPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;
        private int _targetEntity = GameState.NULL_ENTITY;

        private bool _isHeadShot = false;

        public void Run (IEcsSystems systems)
        {
            foreach (var collisionEventEntity in _collisionEventFilter.Value)
            {
                _eventEntity = collisionEventEntity;

                ref var collisionEvent = ref _collisionEventPool.Value.Get(_eventEntity);
                _targetEntity = collisionEvent.TargetEntity;

                if (ObjectWasCollisioned())
                {
                    DeleteEvent();

                    continue;
                }

                if (CollisionWithUndestructibleObject(ref collisionEvent))
                {
                    InvokeLoseEvent();
                    InvokePenetrationEvent(ref collisionEvent);

                    _vibrationEventPool.Value.Add(_world.Value.NewEntity()).Invoke(VibrationEvent.VibrationType.Warning);

                    DeleteEvent();

                    return;
                }

                if (TargetEntityIsKillable())
                {
                    _isHeadShot = collisionEvent.TargetRigidbody.TryGetComponent<HeadMB>(out var component);

                    InvokeStartComboEvent();
                    InvokeKillTargetEvent();
                    GiveScoreEvent();
                    PlayHumanHitAudioEffect();
                    
                    _vibrationEventPool.Value.Add(_world.Value.NewEntity()).Invoke(VibrationEvent.VibrationType.SoftImpact);

                    if (IsSniper())
                    {
                        InvokePenetrationEvent(ref collisionEvent);
                        InvokeLoseEvent();
                    }
                    else if (IsLast())
                    {
                        InvokePenetrationEvent(ref collisionEvent);
                        InvokeWinEvent();
                    }

                    DeleteEvent();

                    return;
                }

                if (TargetEntityIsDestructible())
                {
                    InvokeStartComboEvent();
                    InvokeDestroyEvent();
                    GiveScoreEvent();
                    PlayObjectHitsAudioEffects();

                    _vibrationEventPool.Value.Add(_world.Value.NewEntity()).Invoke(VibrationEvent.VibrationType.LightImpack);

                    DeleteEvent();

                    return;
                }

                DeleteEvent();
            }
        }
        
        private bool ObjectWasCollisioned()
        {
            return _killedPool.Value.Has(_targetEntity) || _destroyedPool.Value.Has(_targetEntity);
        }

        private bool CollisionWithUndestructibleObject(ref CollisionEvent collisionEvent)
        {
            return collisionEvent.TargetRigidbody == null;
        }

        private void InvokePenetrationEvent(ref CollisionEvent collisionEvent)
        {
            _penetrationEventPool.Value.Add(_world.Value.NewEntity()).Invoke(collisionEvent.TargetObject);
        }

        private void InvokeLoseEvent()
        {
            _losePool.Value.Add(_world.Value.NewEntity());
        }

        private void InvokeWinEvent()
        {
            _winPool.Value.Add(_world.Value.NewEntity());
        }

        private bool TargetEntityIsKillable()
        {
            return _killablePool.Value.Has(_targetEntity);
        }

        private void InvokeStartComboEvent()
        {
            _startComboEventPool.Value.Add(_world.Value.NewEntity());
        }

        private void InvokeKillTargetEvent()
        {
            ref var collisionEvent = ref _collisionEventPool.Value.Get(_eventEntity);

            _killTargetEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_targetEntity, collisionEvent.TargetRigidbody);
        }

        private void GiveScoreEvent()
        {
            ref var givableScoresComponent = ref _givableScoresPool.Value.Get(_targetEntity);

            _giveScoreEventPool.Value.Add(_world.Value.NewEntity()).Invoke(givableScoresComponent.Value, _isHeadShot);
        }

        private void PlayHumanHitAudioEffect()
        {
            ref var collisionEvent = ref _collisionEventPool.Value.Get(_eventEntity);

            ref var audioComponent = ref _audioPool.Value.Get(_gameState.Value.ArrowEntity);

            audioComponent.AudioSource.pitch = Random.Range(0.9f, 1.1f);

            if (_isHeadShot)
            {
                audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.EnemysHittings.HeadHitting);
            }
            else
            {
                audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.EnemysHittings.BodyHittings[Random.Range(0, _gameState.Value.AudioPack.AudioStorage.EnemysHittings.BodyHittings.Length - 1)]);
            }
        }

        private void PlayObjectHitsAudioEffects()
        {
            ref var destructibleComponent = ref _destructiblePool.Value.Get(_targetEntity);

            ref var audioComponent = ref _audioPool.Value.Get(_gameState.Value.ArrowEntity);

            audioComponent.AudioSource.pitch = Random.Range(0.8f, 1.2f);

            foreach (var objectMaterial in destructibleComponent.ObjectMaterials)
            {
                switch (objectMaterial)
                {
                    case ObjectMaterial.Wood:
                        audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.WoodsHittings[Random.Range(0, _gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.WoodsHittings.Length - 1)]);
                        break;
                    case ObjectMaterial.Rock:
                        audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.RockHittings[Random.Range(0, _gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.RockHittings.Length - 1)]);
                        break;
                    case ObjectMaterial.Metall:
                        audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.MetallHittings[Random.Range(0, _gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.MetallHittings.Length - 1)]);
                        break;
                    case ObjectMaterial.Glass:
                        audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.GlassHittings[Random.Range(0, _gameState.Value.AudioPack.AudioStorage.ObjectsHitiings.GlassHittings.Length - 1)]);
                        break;
                    default:
                        audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.UIAudio.StoreBuying);
                        break;
                }
            }
        }

        private bool IsSniper()
        {
            return _sniperPool.Value.Has(_targetEntity);
        }

        private bool IsLast()
        {
            return _notKilledFilter.Value.GetEntitiesCount() == 1;
        }

        private bool TargetEntityIsDestructible()
        {
            return _destructiblePool.Value.Has(_targetEntity);
        }

        private void InvokeDestroyEvent()
        {
            _destroyEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_targetEntity);
        }

        private void DeleteEvent()
        {
            _collisionEventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
            _targetEntity = GameState.NULL_ENTITY;
            
            _isHeadShot = false;
        }
    }
}