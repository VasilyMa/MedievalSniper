using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ArrowSpinning : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Arrow>, Exc<Unmovable>> _movableArrowFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private float _standartArrowSpinning = 5;
        private float _spreadArrowSpinning = 15;

        private float _calculatedArrowSpinning;

        private bool _isCalculated = false;

        public void Run (IEcsSystems systems)
        {
            if (!_isCalculated)
            {
                CalculateSpinningSpead();
                _isCalculated = true;
            }

            foreach (var arrowEntity in _movableArrowFilter.Value)
            {
                ref var viewComponent = ref _viewPool.Value.Get(arrowEntity);
                ref var arrowComponent = ref _arrowPool.Value.Get(arrowEntity);
                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                viewComponent.Model.transform.Rotate(Vector3.forward * _calculatedArrowSpinning);
            }
        }

        private void CalculateSpinningSpead()
        {
            _calculatedArrowSpinning = _standartArrowSpinning + (_spreadArrowSpinning * _gameState.Value.PullingStrength);
        }
    }
}