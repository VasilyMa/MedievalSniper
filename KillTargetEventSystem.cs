using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class KillTargetEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<KillTargetEvent>> _killTargetFilter = default;

        readonly EcsPoolInject<KillTargetEvent> _killTargetEventPool = default;
        readonly EcsPoolInject<EnemyPointerComponent> _enemyPointerEventPool = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<Killable> _killablePool = default;
        readonly EcsPoolInject<Outlinable> _outlinablePool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;

        readonly EcsPoolInject<Killed> _killedPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;
        private int _killedEntity = GameState.NULL_ENTITY;

        private float _standartExplosionForce = 250;
        private float _spreadExplosionForce = 1000;

        private float _calculatedExplosionForce;

        private float _standartBodyExplosionForce = 50;

        private int _randomAudioScreamSelected;

        private bool _firstWork = true;

        public void Run (IEcsSystems systems)
        {
            if (_firstWork)
            {
                CalculateExplosionForce();

                _firstWork = false;
            }

            foreach (var killTargetEventEntity in _killTargetFilter.Value)
            {
                _eventEntity = killTargetEventEntity;

                ref var killTargetEvent = ref _killTargetEventPool.Value.Get(_eventEntity);
                _killedEntity = killTargetEvent.KilledEntity;

                if (_killedPool.Value.Has(_killedEntity))
                {
                    DeleteEvent();
                    continue;
                }

                if (_enemyPointerEventPool.Value.Has(_killedEntity))
                {
                    ref var enemyPointerEvent = ref _enemyPointerEventPool.Value.Get(_killedEntity);
                    enemyPointerEvent.TransformPrefabIndicator.gameObject.SetActive(false);
                    enemyPointerEvent.isKillable = false;
                }

                ref var viewComponent = ref _viewPool.Value.Get(_killedEntity);
                viewComponent.GameObject.layer = LayerMask.NameToLayer(nameof(Layers.Dead));
                viewComponent.Animator.enabled = false;

                if (viewComponent.NavMeshAgent != null)
                {
                    viewComponent.NavMeshAgent.enabled = false;
                }

                ref var arrowViewComponent = ref _viewPool.Value.Get(_gameState.Value.ArrowEntity);

                ref var arrowComponent = ref _arrowPool.Value.Get(_gameState.Value.ArrowEntity);

                ref var killableComponent = ref _killablePool.Value.Get(_killedEntity);

                foreach (var rigidbodysBones in killableComponent.RigidbodysBones)
                {

                    rigidbodysBones.isKinematic = false;

                    rigidbodysBones.gameObject.layer = LayerMask.NameToLayer(nameof(Layers.Dead));

                    if (rigidbodysBones == killTargetEvent.TochedRigidbody)
                    {
                        continue;
                    }

                    rigidbodysBones.AddExplosionForce(_standartBodyExplosionForce, arrowComponent.Tip.transform.position, 0, 0, mode: ForceMode.Impulse);

                    rigidbodysBones.maxAngularVelocity = Mathf.Infinity;
                    rigidbodysBones.AddTorque(arrowComponent.Tip.transform.forward * 1000);
                }

                ref var outlinableComponent = ref _outlinablePool.Value.Get(_killedEntity);
                outlinableComponent.Outline.enabled = false;

                killTargetEvent.TochedRigidbody.AddForce(arrowComponent.Tip.transform.forward * _calculatedExplosionForce, ForceMode.Impulse);

                PlayKilledScream();

                _gameState.Value.KillsOnLevel++;

                _killedPool.Value.Add(_killedEntity);

                foreach (var pools in _gameState.Value.ActivePools)
                {
                    if (pools is KillEffectPool)
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

        private void PlayKilledScream()
        {
            ref var audioComponent = ref _audioPool.Value.Get(_killedEntity);
            _randomAudioScreamSelected = Random.Range(0, _gameState.Value.AudioPack.AudioStorage.EnemysHittings.BodyScreams.Length - 1);
            audioComponent.AudioSource.pitch = Random.Range(0.8f, 1.2f);
            audioComponent.AudioSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.EnemysHittings.BodyScreams[_randomAudioScreamSelected]);
        }

        private void DeleteEvent()
        {
            _killTargetEventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
            _killedEntity = GameState.NULL_ENTITY;
        }
    }
}