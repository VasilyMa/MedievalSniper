using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections;
using UnityEngine;


namespace Client
{
    sealed class ArrowController : IEcsRunSystem
    {
        readonly EcsWorldInject _world = default;

        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsFilterInject<Inc<FlightController>> _filter = default;
        readonly EcsFilterInject<Inc<Arrow, Unmovable>> _arrowUnmovableFilter = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Controlled> _controlledPool = default;
        readonly EcsPoolInject<BulletTimeComponent> _bulletPool = default;

        readonly EcsPoolInject<WinEvent> _winPool = default;


        private Arrow ArrowComponent;
        private View ViewComponent;
        private FloatingJoystick FloatingJoystick;

        private float _horizontalRotateMultiply = 0.6f;


        private float _doubleTapTimer = 0;
        private int _countTap = 0;

        private int _bulletTimeEntity = GameState.NULL_ENTITY;

        Vector2 tapPos;
        Vector2 swipeDelta;

        Vector3 Forward = new Vector3(-90, 0, 0);
        Vector3 Up = new Vector3(-115, 0, 0);
        Vector3 Down = new Vector3(-65, 0, 0);
        Vector3 Right = new Vector3(0, 20, 0);
        Vector3 Left = new Vector3(0, -20, 0);

        private const float DEAD_ZONE = 0f;

        private bool isSwiping;

        private float rotationSpeed;
        public void Run(IEcsSystems systems)
        {
            foreach (var penetratedEntity in _arrowUnmovableFilter.Value)
            {
                return;
            }

            foreach (var entity in _filter.Value)
            {
                var arrowEntity = _state.Value.ArrowEntity;
                ArrowComponent = _arrowPool.Value.Get(arrowEntity);
                ViewComponent = _viewPool.Value.Get(arrowEntity);
                FloatingJoystick = _interfacePool.Value.Get(_state.Value.InterfaceEntity).FloatingJoystick;
                rotationSpeed = _state.Value.RotateSpeed + _state.Value.SkinData.RotateSpeedBonus;
                if (Input.GetKeyDown(KeyCode.Space))
                    _winPool.Value.Add(_world.Value.NewEntity());
                if (Input.GetMouseButtonDown(0))
                {
                    if (Tutorial.CurrentStage == Tutorial.Stage.ArrowControle)
                    {
                        Tutorial.ArrowControle.SetIsControlled();
                    }

                    _bulletTimeEntity = _world.Value.NewEntity();

                    ref var bulletComp = ref _bulletPool.Value.Add(_bulletTimeEntity);

                    if (!_controlledPool.Value.Has(_state.Value.ArrowEntity))
                    {
                        _controlledPool.Value.Add(_state.Value.ArrowEntity);
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    MoveByDirection(); //there we detection are direction and move or rotate 
                }
                if (_doubleTapTimer > 0) //just timer for double tap check, if timer equal to 0 count of touch reset to 0
                    _doubleTapTimer -= Time.deltaTime; 
                else
                {
                    _countTap = 0;
                    _doubleTapTimer = 0;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_bulletTimeEntity != GameState.NULL_ENTITY && _bulletPool.Value.Has(_bulletTimeEntity))
                    {
                        _bulletPool.Value.Del(_bulletTimeEntity);
                        _bulletTimeEntity = GameState.NULL_ENTITY;
                    }

                    _controlledPool.Value.Del(_state.Value.ArrowEntity);
                }
            }
        }

        void MoveByDirection()
        {
            if (FloatingJoystick.Horizontal < -DEAD_ZONE)
            {
                ViewComponent.Transform.RotateAround(ViewComponent.Transform.position, Vector3.down, rotationSpeed * Time.unscaledDeltaTime * -FloatingJoystick.Horizontal * _horizontalRotateMultiply);
            }
            if (FloatingJoystick.Horizontal > DEAD_ZONE)
            {
                ViewComponent.Transform.RotateAround(ViewComponent.Transform.position, Vector3.up, rotationSpeed * Time.unscaledDeltaTime * FloatingJoystick.Horizontal * _horizontalRotateMultiply);
            }
            if (FloatingJoystick.Vertical < -DEAD_ZONE)
            {
                ViewComponent.Transform.Translate(Vector3.down * -FloatingJoystick.Vertical * Time.deltaTime, Space.World);

                if (ViewComponent.Transform.rotation.eulerAngles.x < 40 || ViewComponent.Transform.rotation.eulerAngles.x > 360 - 100)  
                {
                    ViewComponent.Transform.Rotate(new Vector3(-FloatingJoystick.Vertical, 0, 0) * rotationSpeed * Time.deltaTime);
                }// 40 degrees max angle to rotate
            }
            if (FloatingJoystick.Vertical > DEAD_ZONE)
            {
                ViewComponent.Transform.Translate(Vector3.up * FloatingJoystick.Vertical * Time.deltaTime, Space.World);

                if (ViewComponent.Transform.rotation.eulerAngles.x > 360 - 40 || ViewComponent.Transform.rotation.eulerAngles.x == 0 || ViewComponent.Transform.rotation.eulerAngles.x < 100)
                {
                    ViewComponent.Transform.Rotate(new Vector3(FloatingJoystick.Vertical * -1, 0, 0) * rotationSpeed * Time.deltaTime);
                }// 40 degrees max angle to rotate
            }
        }

        void StartBullet()
        {
            ref var bulletComp = ref _bulletPool.Value.Add(_world.Value.NewEntity());
        }
    }
}