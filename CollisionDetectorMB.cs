using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Burst;
using UnityEngine;

namespace Client
{
    
    public class CollisionDetectorMB : MonoBehaviour
    {
        [SerializeField] private GameObject _mainGameObject;
        [SerializeField] private EcsInfoMB _ecsInfoMB;

        private EcsWorldInject _world;

        private EcsPool<CollisionEvent> _collisionEventPool;

        private int _targetEntity = GameState.NULL_ENTITY;

        private GameObject _lastCollisionedObject;

        private float _firstWorkDelay = 1f;
        private bool _isFirstWork = true;

        private float _enableDelay = 0.5f;
        private bool _isDisable = true;

        public bool ArrowIsLaunched { get; set; } = false;

        void Start()
        {
            if (_mainGameObject == null) _mainGameObject = transform.parent.gameObject;
            if (_ecsInfoMB == null) _ecsInfoMB = GetComponentInParent<EcsInfoMB>();
        }

        private void Update()
        {
            if (_isFirstWork)
            {
                _firstWorkDelay -= Time.unscaledDeltaTime;

                if (_firstWorkDelay < 0)
                {
                    _world = _ecsInfoMB.GetWorld();
                    _collisionEventPool = _world.Value.GetPool<CollisionEvent>();

                    _isFirstWork = false;
                }
            }

            if (ArrowIsLaunched && _isDisable)
            {
                _enableDelay -= Time.deltaTime;

                if (_enableDelay < 0)
                {
                    _isDisable = false;
                }
            }
        }

        [BurstCompile]
        private void OnTriggerEnter(Collider other)
        {
            if (!ArrowIsLaunched)
            {
                return;
            }

            if (_isDisable)
            {
                return;
            }

            if (other.gameObject == _lastCollisionedObject)
            {
                return;
            }

            if (other.isTrigger)
            {
                return;
            }

            _lastCollisionedObject = other.gameObject;

            if (other.gameObject.layer == LayerMask.NameToLayer(nameof(Layers.Undestructible)))
            {
                _collisionEventPool.Add(_world.Value.NewEntity()).Invoke(other.gameObject);

                return;
            }

            InvokeCollisionEvent(in _lastCollisionedObject);
        }

        private void InvokeCollisionEvent(in GameObject gameObject)
        {
            _targetEntity = gameObject.GetComponentInParent<EcsInfoMB>().GetEntity();

            if (gameObject.TryGetComponent<Rigidbody>(out var targetRigidbody))
            {
                _collisionEventPool.Add(_world.Value.NewEntity()).Invoke(_targetEntity, targetRigidbody, gameObject);

                _targetEntity = GameState.NULL_ENTITY;
            }

        }
    }
}
