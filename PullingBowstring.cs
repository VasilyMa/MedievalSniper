using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class PullingBowstring : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Pullable>> _pullableFilter = default;

        readonly EcsPoolInject<Pullable> _pullablePool = default;
        readonly EcsPoolInject<View> _viewPool = default;

        private Vector3 _arrowStarPosition = new Vector3(0f, 0f, -4.5f);
        private Vector3 _arrowEndPosition = new Vector3(0f, 0f, -5f);

        private float _timeToMove = 0;
        private float _totalUpTime = 0;
        private float _totalDownTime = 0;

        private bool _firstWork = true;

        public void Run (IEcsSystems systems)
        {
            if (_gameState.Value.AfterLaunchingSystems)
            {
                return;
            }

            foreach (var pullableEntity in _pullableFilter.Value)
            {
                ref var pullableComponent = ref _pullablePool.Value.Get(pullableEntity);

                ref var arrowViewComponent = ref _viewPool.Value.Get(_gameState.Value.ArrowEntity);

                if (_firstWork)
                {
                    _totalUpTime = pullableComponent.PullongUpCurve.keys[pullableComponent.PullongUpCurve.keys.Length - 1].time;
                    _totalDownTime = pullableComponent.PullongDownCurve.keys[pullableComponent.PullongDownCurve.keys.Length - 1].time;
                    _arrowStarPosition.y += arrowViewComponent.Transform.position.y; // kostili
                    _arrowEndPosition.y += arrowViewComponent.Transform.position.y; // kostili

                    _firstWork = false;
                }

                if (pullableComponent.isPullingUp)
                {
                    pullableComponent.CurrentStrength = pullableComponent.PullongUpCurve.Evaluate(_timeToMove);
                }
                else
                {
                    pullableComponent.CurrentStrength = pullableComponent.PullongDownCurve.Evaluate(_timeToMove);
                }

                //arrowViewComponent.Transform.position = Vector3.Lerp(_arrowStarPosition, _arrowEndPosition, pullableComponent.CurrentStrength);
                ChangeIKPositions(ref pullableComponent);


                _timeToMove += Time.deltaTime;

                if (pullableComponent.isPullingUp && _timeToMove >= _totalUpTime)
                {
                    ResetTimeAndSwitchPullingDirection(ref pullableComponent);
                }
                else if (!pullableComponent.isPullingUp && _timeToMove >= _totalDownTime)
                {
                    ResetTimeAndSwitchPullingDirection(ref pullableComponent);
                }
            }
        }

        private void ChangeIKPositions(ref Pullable pullableComponent)
        {
            var currentStrength = pullableComponent.CurrentStrength;

            pullableComponent.SniperChangeIKStates.HandSecondPositionConstraint.weight = currentStrength;

            var bowstringSourceObjects = pullableComponent.SniperChangeIKStates.BowstringMultiParentConstraint.data.sourceObjects;

            bowstringSourceObjects.SetWeight(0, 1 - currentStrength);
            bowstringSourceObjects.SetWeight(1, currentStrength);

            pullableComponent.SniperChangeIKStates.BowstringMultiParentConstraint.data.sourceObjects = bowstringSourceObjects;

            var arrowSourceObjects = pullableComponent.SniperChangeIKStates.ArrowMultiParentConstraint.data.sourceObjects;

            arrowSourceObjects.SetWeight(2, 1 - currentStrength);
            arrowSourceObjects.SetWeight(3, currentStrength);

            pullableComponent.SniperChangeIKStates.ArrowMultiParentConstraint.data.sourceObjects = arrowSourceObjects;

        }

        private void ResetTimeAndSwitchPullingDirection(ref Pullable pullableComponent)
        {
            _timeToMove = 0;

            pullableComponent.isPullingUp = !pullableComponent.isPullingUp;
        }
    }
}