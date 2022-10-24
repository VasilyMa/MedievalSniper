using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ResetArrowDirection : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<Controlled> _controlledPool = default;
        readonly EcsPoolInject<Penetrated> _penetratedPool = default;
        readonly EcsPoolInject<View> _viewPool = default;

        private int _arrowEntity = GameState.NULL_ENTITY;

        private float _timeToResetMaxValue = 3f;
        private float _timeToResetCurrentValue = 0f;

        private bool _firstWork = true;
        private bool _workIsDone = true;

        public void Run (IEcsSystems systems)
        {
            if (_firstWork)
            {
                _arrowEntity = _gameState.Value.ArrowEntity;
            }

            if (ArrowIsControlledOrPenetrated())
            {
                _timeToResetCurrentValue = 0;
                _workIsDone = false;

                return;
            }

            if (_workIsDone)
            {
                return;
            }

            ref var arrowViewComponent = ref _viewPool.Value.Get(_arrowEntity);

            var neededDirection = new Quaternion(Quaternion.identity.x, arrowViewComponent.Model.transform.rotation.y, arrowViewComponent.Model.transform.rotation.z, arrowViewComponent.Model.transform.rotation.w);

            arrowViewComponent.Model.transform.rotation = Quaternion.Slerp(arrowViewComponent.Model.transform.rotation, neededDirection, _timeToResetCurrentValue / _timeToResetMaxValue);

            _timeToResetCurrentValue += Time.unscaledDeltaTime;

            if (_timeToResetCurrentValue >= _timeToResetMaxValue)
            {
                _timeToResetCurrentValue = 0;
                _workIsDone = true;
            }
        }

        private bool ArrowIsControlledOrPenetrated()
        {
            return _controlledPool.Value.Has(_arrowEntity) || _penetratedPool.Value.Has(_arrowEntity);
        }
    }
}