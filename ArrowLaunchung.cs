using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Client
{
    sealed class ArrowLaunchung : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<PlayEvent>> _playFilter = default;

        readonly EcsPoolInject<Pullable> _pullablePool = default;
        readonly EcsPoolInject<Arrow> _arrowPool = default;
        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<FlightController> _flightPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;

        private int _sniperEntity;
        private int _arrowEntity;
        private const float DURATION_BULLET_TIME = 20;

        private float _standartArrowSpeed = 3;
        private float _spreadArrowSpeed = 7;

        private bool _firstWork = true;
        private bool _buttonIsDown = false;

        private float _rotateSpeedBonusForSkillcheck = 50;

        public void Run (IEcsSystems systems)
        {
            foreach (var entity in _playFilter.Value)
            {

                if (_firstWork)
                {
                    _sniperEntity = _gameState.Value.SniperEntity;
                    _arrowEntity = _gameState.Value.ArrowEntity;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    TutorialSetIsLaunchedFirstTime();

                    LaunchArrowAndSwitchSystems();
                    AddRotateSpeedBonus();
                    SwitchCamera();
                    ResetAllSniperIK();

                    AppMetricaEvent();

                    _playFilter.Pools.Inc1.Del(entity);
                    break;
                }
            }
        }
        private void AppMetricaEvent()
        {
            SaveManager.GetData().SceneCount++;
            var sceneName = SceneManager.GetActiveScene().name;
            var sceneNumber = SceneManager.GetActiveScene().buildIndex;
            var sceneCount = SaveManager.GetData().SceneCount;

            AzureMetrica.LevelStart(sceneName, sceneNumber, sceneCount, 0);
        }


        private void TutorialSetIsLaunchedFirstTime()
        {
            if (Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
            {
                Tutorial.FirstLaunching.SetIsLaunched();
            }
        }

        private void LaunchArrowAndSwitchSystems()
        {
            Debug.Log("Launch the Arrow");

            ref var pullableComponent = ref _pullablePool.Value.Get(_sniperEntity);
            pullableComponent.SniperChangeIKStates.ArrowMultiParentConstraint.weight = 0;

            _gameState.Value.PullingBowstringSystems = false;
            _gameState.Value.LaunchingSystems = false;
            _gameState.Value.AfterLaunchingSystems = true;
            _gameState.Value.PullingStrength = pullableComponent.CurrentStrength;

            ref var arrowViewComponent = ref _viewPool.Value.Get(_arrowEntity);

            var currentArrowModelPosition = new Vector3(arrowViewComponent.Transform.position.x, arrowViewComponent.Transform.position.y, arrowViewComponent.Transform.position.z);
            
            arrowViewComponent.Transform.SetParent(null);

            arrowViewComponent.Transform.position = currentArrowModelPosition;

            ref var arrowComponent = ref _arrowPool.Value.Get(_arrowEntity);
            arrowComponent.StrengthLaunching = _standartArrowSpeed + (_spreadArrowSpeed * pullableComponent.CurrentStrength);
            arrowComponent.UsualTrail.gameObject.SetActive(true);
            arrowComponent.CollisionDetectorMB.ArrowIsLaunched = true;
            arrowComponent.BulletTimeChargeMax = DURATION_BULLET_TIME + _gameState.Value.SkinData.SkillTimeBonus;
            arrowComponent.BulletTimeChargeCurrent = pullableComponent.CurrentStrength * DURATION_BULLET_TIME;

            ref var flightComponent = ref _flightPool.Value.Add(_arrowEntity);
            ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
            interfaceComponent.SlowMotionPanelMB.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            interfaceComponent.SlowMotionPanelMB.SetParticle(arrowComponent.StrengthLaunching);
            //interfaceComponent.StrengthBarMB.StartFlight();
            interfaceComponent.FloatingJoystick.gameObject.SetActive(true);

            ref var audioComponent = ref _audioPool.Value.Get(_arrowEntity);
            audioComponent.AudioSource.Play();

            _gameState.Value.AudioPack.AudioSnapshots.Normal.TransitionTo(1f);
        }

        private void AddRotateSpeedBonus()
        {
            _gameState.Value.RotateSpeed += _rotateSpeedBonusForSkillcheck * _gameState.Value.PullingStrength;
        }

        private void SwitchCamera()
        {
            ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);
            cameraComponent.ArrowVirtualCamera.Priority = 5;
            cameraComponent.SniperVirtualCamera.Priority = 0;
        }

        private void ResetAllSniperIK()
        {
            ref var pullableComponent = ref _pullablePool.Value.Get(_sniperEntity);
            pullableComponent.SniperChangeIKStates.HandFirstPositionConstraint.weight = 0;
            pullableComponent.SniperChangeIKStates.HandSecondPositionConstraint.weight = 0;
            pullableComponent.SniperChangeIKStates.BowstringMultiParentConstraint.weight = 0;

            ref var viewComponent = ref _viewPool.Value.Get(_sniperEntity);
            viewComponent.Animator.SetTrigger("Launchung");
        }
    }
}