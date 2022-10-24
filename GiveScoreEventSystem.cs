using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class GiveScoreEventSystem : IEcsRunSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<GiveScoreEvent>> _giveScoreEventFilter = default;
        readonly EcsFilterInject<Inc<ComboComponent>> _comboComponentFilter = default;

        readonly EcsPoolInject<GiveScoreEvent> _giveScoreEventPool = default;
        readonly EcsPoolInject<ShowScoresEvent> _showScoreEventPool = default;

        readonly EcsPoolInject<ComboComponent> _comboComponent = default;
        readonly EcsPoolInject<InterfaceComponent> _interfaceComponent = default;

        private int _eventEntity = GameState.NULL_ENTITY;

        private int _comboCount = 0;
        private float _comboMultiply = 0;
        private int _scoreForObject = 0;
        private int _scoreForObjectAfterMultiply = 0;

        private bool _isHeadShot = false;

        public void Run (IEcsSystems systems)
        {
            foreach (var eventEntity in _giveScoreEventFilter.Value)
            {
                _eventEntity = eventEntity;

                ref var giveScoreEventComponent = ref _giveScoreEventPool.Value.Get(_eventEntity);
                _scoreForObject = giveScoreEventComponent.Value;
                _isHeadShot = giveScoreEventComponent.IsHeadShot;

                FindAndWriteComboCount();

                CalculateScore();

                AddPlayerScoreCount();

                InvokeShowScoreEvent();

                DeleteEvent();
            }
        }

        private void FindAndWriteComboCount()
        {
            foreach (var comboEntity in _comboComponentFilter.Value)
            {
                ref var comboComponent = ref _comboComponent.Value.Get(comboEntity);
                _comboCount = comboComponent.Count;
            }
        }

        private void CalculateScore()
        {
            _comboMultiply = Scores.Combo.GetMultiplyFromComboCount(_comboCount);

            if (_isHeadShot)
            {
                _scoreForObjectAfterMultiply = (int)(_scoreForObject * _comboMultiply * Scores.HEADSHOT_MULTIPLY);
            }
            else
            {
                _scoreForObjectAfterMultiply = (int)(_scoreForObject * _comboMultiply);
            }
        }

        private void AddPlayerScoreCount()
        {
            _gameState.Value.CurrentPlayerScore += _scoreForObjectAfterMultiply;
        }

        private void InvokeShowScoreEvent()
        {
            _showScoreEventPool.Value.Add(_world.Value.NewEntity()).Invoke(_scoreForObjectAfterMultiply, _isHeadShot);
        }

        private void DeleteEvent()
        {
            _giveScoreEventPool.Value.Del(_eventEntity);

            _eventEntity = GameState.NULL_ENTITY;
            _comboCount = 0;
            _comboMultiply = 0;
            _scoreForObject = 0;
            _scoreForObjectAfterMultiply = 0;

            _isHeadShot = false;
        }
    }
}