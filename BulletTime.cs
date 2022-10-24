using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class BulletTime : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<BulletTimeComponent>> _bulletFilter = default;

        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private float _bulletTimeValue = 0.25f;
        private float _fixedDeltaTimeValue = 0.02f;

        private bool isBulletTime = false;

        private int _bulletTimeEntity = GameState.NULL_ENTITY;

        public void Run (IEcsSystems systems)
        {
            if (isBulletTime && _bulletFilter.Value.GetEntitiesCount() <= 0)
            {
                ref var arrowComponent = ref _arrowPool.Value.Get(_gameState.Value.ArrowEntity);
                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                DisableBulletTime(ref arrowComponent, ref interfaceComponent);
            }

            foreach (var bulletTimeEntity in _bulletFilter.Value)
            {
                ref var arrowComponent = ref _arrowPool.Value.Get(_gameState.Value.ArrowEntity);
                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
                _bulletTimeEntity = bulletTimeEntity;

                if (arrowComponent.BulletTimeChargeCurrent == 0)
                {
                    DisableBulletTime(ref arrowComponent, ref interfaceComponent);
                    return;
                }    

                if (!isBulletTime) EnableBulletTime(); //check and set bullet time

                if (Input.GetMouseButtonUp(0)) DisableBulletTime(ref arrowComponent, ref interfaceComponent); //stop bullet time if we stop input

                RemainingTime(ref arrowComponent, ref interfaceComponent);
            }
        }
        private void RemainingTime(ref Arrow arrowComponent, ref InterfaceComponent interfaceComponent)
        {
            if (arrowComponent.BulletTimeChargeCurrent > 0) 
            {
                arrowComponent.BulletTimeChargeCurrent -= Time.unscaledDeltaTime;
                interfaceComponent.StrengthBarMB.UpdateSlider(arrowComponent.BulletTimeChargeCurrent / arrowComponent.BulletTimeChargeMax);
            }
            else
            {
                interfaceComponent.StrengthBarMB.UpdateSlider(0);
                arrowComponent.BulletTimeChargeCurrent = 0;
            }
        }

        void EnableBulletTime()
        {
            isBulletTime = true;    
            Time.timeScale = _bulletTimeValue;
            Time.fixedDeltaTime = Time.timeScale * _fixedDeltaTimeValue;

            _gameState.Value.AudioPack.AudioSnapshots.BulletTime.TransitionTo(0.1f);
        }

        void DisableBulletTime(ref Arrow arrowComponent, ref InterfaceComponent interfaceComponent)
        {
            isBulletTime = false;
            Time.timeScale = 1;
            //_bulletFilter.Pools.Inc1.Del(_bulletTimeEntity);

            _gameState.Value.AudioPack.AudioSnapshots.Normal.TransitionTo(0.1f);

            //_bulletTimeEntity = GameState.NULL_ENTITY;
        }
    }
}