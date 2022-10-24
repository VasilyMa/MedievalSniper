using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Collections.Generic;

namespace Client {
    sealed class InitPools : IEcsInitSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Destructible>> _destructibleFilter = default;
        readonly EcsFilterInject<Inc<Killable>> _killableFilter = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        private int _destroyEffectsCount = 3;
        private int _killEffectsCount = 3;
        private int _scoresEffectsCount = 2;

        public void Init (IEcsSystems systems)
        {
            _gameState.Value.ActivePools = new List<Pools>();

            _destroyEffectsCount = Mathf.CeilToInt((float)_destructibleFilter.Value.GetEntitiesCount() / 2);
            _killEffectsCount = Mathf.CeilToInt((float)_killableFilter.Value.GetEntitiesCount() / 2);
            _scoresEffectsCount = _destroyEffectsCount + _killEffectsCount;

            foreach (var pool in _gameState.Value.AllPools)
            {
                switch (pool)
                {
                    case DestroyEffectPool:
                        _gameState.Value.ActivePools.Add(new DestroyEffectPool(pool.Prefab, _destroyEffectsCount, "DestroyEffects"));

                        continue;

                    case KillEffectPool:
                        _gameState.Value.ActivePools.Add(new KillEffectPool(pool.Prefab, _killEffectsCount, "KillEffects"));

                        continue;

                    case ScoresEffectPool:
                        _gameState.Value.ActivePools.Add(new ScoresEffectPool(pool.Prefab, _scoresEffectsCount, "ScoresEffects"));

                        continue;
                }
            }

            ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

            foreach (var pools in _gameState.Value.ActivePools)
            {
                if (pools is ScoresEffectPool)
                {
                    for (int i = 0; i < _scoresEffectsCount; i++)
                    {
                        var textPrefab = pools.GetFromPool()?.GetComponent<ScorePrefabMB>();

                        textPrefab.transform.position = interfaceComponent.MainCanvas.transform.position;
                        textPrefab.transform.SetParent(interfaceComponent.MainCanvas.transform);
                        textPrefab.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}