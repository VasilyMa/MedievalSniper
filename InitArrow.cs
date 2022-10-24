using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitArrow : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;

        public void Init (IEcsSystems systems)
        {
            var arrowEntity = _world.Value.NewEntity();

            _gameState.Value.ArrowEntity = arrowEntity;

            var arrowGameObject = GameObject.FindObjectOfType<ArrowMB>().gameObject;

            ref var viewComponent = ref _viewPool.Value.Add(arrowEntity);
            viewComponent.EntityNumber = arrowEntity;
            viewComponent.GameObject = arrowGameObject;
            viewComponent.EcsInfoMB = arrowGameObject.GetComponent<EcsInfoMB>();
            viewComponent.EcsInfoMB.Init(_world, arrowEntity);

            viewComponent.Transform = arrowGameObject.transform;
            viewComponent.LayerBeforeDeath = arrowGameObject.layer;

            viewComponent.Model = arrowGameObject.GetComponentInChildren<ArrowModelMB>().gameObject;
            viewComponent.Rigidbody = viewComponent.Model.GetComponent<Rigidbody>();
            viewComponent.Rigidbody.maxAngularVelocity = Mathf.Infinity;

            ref var arrowComponent = ref _arrowPool.Value.Add(arrowEntity);
            arrowComponent.StrengthLaunching = 0;
            arrowComponent.CollisionDetectorMB = viewComponent.Model.GetComponent<CollisionDetectorMB>();

            arrowComponent.Tip = arrowGameObject.GetComponentInChildren<TipMB>().gameObject;

            arrowComponent.UsualTrail = viewComponent.Model.GetComponentInChildren<UsualTrailMB>().GetComponent<ParticleSystem>();
            arrowComponent.EpicTrail = viewComponent.Model.GetComponentInChildren<EpicTrailMB>().GetComponent<ParticleSystem>();

            arrowComponent.UsualTrail.gameObject.SetActive(false);
            arrowComponent.EpicTrail.gameObject.SetActive(false);

            ref var audioComponent = ref _audioPool.Value.Add(arrowEntity);
            audioComponent.AudioSource = arrowGameObject.GetComponent<AudioSource>();
        }
    }
}