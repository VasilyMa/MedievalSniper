using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class FirstLaunching : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private Sequence _sequence;

        private bool _isEnabledUI = false;
        private bool _isChangedUI = false;

        private float _timerForEndStage = 0;
        private const float _timerMaxValue = 1f;

        private bool _timerForEndStageIsStart = false;
        private bool _timerForEndStageIsEnd = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.FirstLaunching)
            {
                return;
            }

            if (_timerForEndStageIsStart && !_timerForEndStageIsEnd)
            {
                _timerForEndStage += Time.deltaTime;

                if (_timerForEndStage >= _timerMaxValue)
                {
                    _timerForEndStageIsEnd = true;
                    Debug.Log("DONE");
                }
            }

            if (FingerDontHoldLaunchingSystemsNotWorkAndCameraIsNotEndFly())
            {
                return;
            }

            if (!Tutorial.StageIsEnable)
            {
                Tutorial.StageIsEnable = true;
            }

            ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

            if (!_isEnabledUI)
            {
                EnableUI();
                DoAnimation();
                DoTutorialAudio();

                _isEnabledUI = true;
            }

            if (!_isChangedUI && Tutorial.FirstLaunching.SkillCheckIsArriving())
            {
                ChangeUI();
                DisableAnimation();

                _timerForEndStageIsStart = true;

                _isChangedUI = true;
            }

            if (Tutorial.FirstLaunching.isLaunched())
            {
                DoNormalAudio();
                EndStage();
            }
        }

        private bool FingerDontHoldLaunchingSystemsNotWorkAndCameraIsNotEndFly()
        {
            return !Tutorial.FirstLaunching.isHolded() && (_gameState.Value.LaunchingSystems == false || _gameState.Value.IsEndCameraFly == false);
        }

        private void EnableUI()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.Hand.gameObject.SetActive(true);
                tutorialComponent.Focus.gameObject.SetActive(true);
                tutorialComponent.Message.gameObject.SetActive(true);

                var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

                tutorialComponent.Hand.position = screenCenter;
                tutorialComponent.Focus.position = screenCenter;

                tutorialComponent.MessageRectTransform.pivot = new Vector2(0.5f, 1f);
                tutorialComponent.Message.position = screenCenter;
                tutorialComponent.MessageText.text = "Hold!";
            }
        }

        private void DoAnimation()
        {
            _sequence = DOTween.Sequence();

            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.transform.localScale = Vector3.one;

            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.7f, 2f));
            _sequence.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void DoTutorialAudio()
        {
            _gameState.Value.AudioPack.AudioSnapshots.TutorialPause.TransitionTo(1f);
        }

        private void ChangeUI()
        {
            foreach (var interfaceEntity in _tutorialFilter.Value)
            {
                ref var tutorialComponent = ref _tutorialPool.Value.Get(interfaceEntity);

                tutorialComponent.MessageText.alignment = TextAnchor.MiddleLeft;
                tutorialComponent.MessageText.text = "Skill Check\nis affect on\narrow speed\nand final scores!";

                ref var interfaceComponent = ref _interfacePool.Value.Get(interfaceEntity);

                tutorialComponent.Focus.position = interfaceComponent.TargetStrengthBar.position;
                tutorialComponent.Message.position = interfaceComponent.TargetStrengthBar.position;
                tutorialComponent.MessageRectTransform.pivot = new Vector2(1f, 0.5f);

                tutorialComponent.Hand.gameObject.SetActive(false);
            }
        }

        private void DisableAnimation()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            _sequence.Kill();

            tutorialComponent.Hand.transform.localScale = Vector3.one;
        }

        private void DoNormalAudio()
        {
            _gameState.Value.AudioPack.AudioSnapshots.Normal.TransitionTo(1f);
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);

            tutorialComponent.MessageText.alignment = TextAnchor.MiddleCenter;
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            Tutorial.StageIsEnable = false;

            if (_timerForEndStageIsEnd)
            {
                Tutorial.SetNextStage(_gameState);
            }
            else
            {
                Tutorial.FirstLaunching.ResetStage();
                ResetStage();
            }
        }

        private void ResetStage()
        {
            _isEnabledUI = false;
            _isChangedUI = false;

            _timerForEndStage = 0;

            _timerForEndStageIsStart = false;
            _timerForEndStageIsEnd = false;
        }
    }
}