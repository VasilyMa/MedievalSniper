using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ArrowFalling : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<Controlled> _controlledPool = default;
        readonly EcsPoolInject<Penetrated> _penetratedPool = default;
        readonly EcsPoolInject<View> _viewPool = default;

        private int _arrowEntity = GameState.NULL_ENTITY;

        private float _standartFallinSpeed = 5;
        private float _spreadFallinSpeed = 40;

        private float _calculatedFallinSpeed;

        private float _rotationDeadZone = 0.5f;

        private bool _firstWork = true;

        public void Run(IEcsSystems systems)
        {
            if (_firstWork)
            {
                _arrowEntity = _gameState.Value.ArrowEntity;
                CalculateFallinSpeed();
            }

            if (ArrowIsControlledOrPenetrated())
            {
                return;
            }

            ref var arrowViewComponent = ref _viewPool.Value.Get(_arrowEntity);

            if (arrowViewComponent.Transform.rotation.normalized.x >= _rotationDeadZone)
            {
                return;
            }

            arrowViewComponent.Transform.Rotate(Vector3.right, (_calculatedFallinSpeed * Time.deltaTime));
        }

        private void CalculateFallinSpeed()
        {
            _calculatedFallinSpeed = _standartFallinSpeed + (_spreadFallinSpeed * (1 - _gameState.Value.PullingStrength));
        }

        private bool ArrowIsControlledOrPenetrated()
        {
            return _controlledPool.Value.Has(_arrowEntity) || _penetratedPool.Value.Has(_arrowEntity);
        }
    }
}