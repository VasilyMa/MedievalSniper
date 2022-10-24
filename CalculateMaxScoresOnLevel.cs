using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class CalculateMaxScoresOnLevel : IEcsInitSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<GivableScores>> _givableScoresFilter = default;

        readonly EcsPoolInject<GivableScores> _givableScoresPool = default;

        private int _objectsCount = 0;

        private int _maxScoreOnLevel = 0;

        public void Init(IEcsSystems systems)
        {
            foreach (var givableScoresEntity in _givableScoresFilter.Value)
            {
                ref var givableScoresComponent = ref _givableScoresPool.Value.Get(givableScoresEntity);

                if (givableScoresComponent.Value < 0)
                {
                    continue;
                }

                _objectsCount++;

                _maxScoreOnLevel += (int)(givableScoresComponent.Value * Scores.Combo.GetMultiplyFromComboCount(_objectsCount));
            }

            _gameState.Value.TotalMaxScoreOnLevel = _maxScoreOnLevel;
        }
    }
}