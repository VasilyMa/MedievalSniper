using Leopotam.EcsLite;
using UnityEngine;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class ScoreInitCollison : IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsPoolInject<View> _viewPool = default;

        readonly EcsPoolInject<ScoreComponent> _scoreComponentPool = default;

        private int _currentEntity = GameState.NULL_ENTITY;

        public void Init (IEcsSystems systems) 
        {
            var gameObjects = GameObject.FindObjectsOfType<ScoreEffectMB>();

            foreach (var scoresGo in gameObjects)
            {
                _currentEntity = _world.Value.NewEntity();

                ref var viewComponent = ref _viewPool.Value.Add(_currentEntity);
                viewComponent.GameObject = scoresGo.gameObject;
                viewComponent.EcsInfoMB = scoresGo.GetComponent<EcsInfoMB>();
                viewComponent.EcsInfoMB.Init(_world, _currentEntity);

                ref var scoreComponentPool = ref _scoreComponentPool.Value.Add(_currentEntity);
                scoreComponentPool.Score = scoresGo.Score;
                scoreComponentPool.Canvas = scoresGo.canvas;
            }
        }
    }
}