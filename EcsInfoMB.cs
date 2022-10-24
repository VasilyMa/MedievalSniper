using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    public class EcsInfoMB : MonoBehaviour
    {
        private EcsWorldInject _world;

        [SerializeField] private int _objectEntity;

        public void Init(EcsWorldInject world, int objectEntity)
        {
            _world = world;
            _objectEntity = objectEntity;
        }

        public int GetEntity()
        {
            return _objectEntity;
        }

        public EcsWorldInject GetWorld()
        {
            return _world;
        }
    }
}
