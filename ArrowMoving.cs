using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ArrowMoving : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<Arrow>, Exc<Unmovable>> _movableArrowFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;

        public void Run (IEcsSystems systems)
        {
            foreach (var arrowEntity in _movableArrowFilter.Value)
            {
                if (Tutorial.CurrentStage == Tutorial.Stage.ArrowControle && !Tutorial.ArrowControle.ArrowIsLaunched())
                {
                    Tutorial.ArrowControle.SetArrowIsLaunched();
                }

                ref var viewComponent = ref _viewPool.Value.Get(arrowEntity);
                ref var arrowComponent = ref _arrowPool.Value.Get(arrowEntity);
                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                viewComponent.Transform.Translate((Vector3.forward * Time.deltaTime * arrowComponent.StrengthLaunching));
            }
        }
       
    }
}