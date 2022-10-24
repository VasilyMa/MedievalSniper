using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;

namespace Client
{
    sealed class ArrowControle : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<TutorialComponent>> _tutorialFilter = default;

        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private Sequence _sequence;

        private bool _isEnabledUI = false;
        private bool _isPrematureEnd = false;

        public void Run (IEcsSystems systems)
        {
            if (Tutorial.CurrentStage != Tutorial.Stage.ArrowControle)
            {
                return;
            }

            if (_isPrematureEnd)
            {
                return;
            }

            if (!Tutorial.ArrowControle.ArrowIsLaunched())
            {
                return;
            }

            if (!Tutorial.StageIsEnable)
            {
                Tutorial.StageIsEnable = true;
            }

            if (!_isEnabledUI)
            {
                EnableUI();
                DoAnimation();
                DoTutorialAudio();

                Time.timeScale = 0.1f;

                _isEnabledUI = true;
            }

            if (Tutorial.ArrowControle.IsPrematureEnd())
            {
                DoNormalAudio();
                PrematureEndStage();
                Tutorial.ArrowControle.ResetArrowIsLaunched();
                _isPrematureEnd = true;
            }

            if (Tutorial.ArrowControle.IsControlled())
            {
                DoNormalAudio();
                EndStage();
            }

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
                tutorialComponent.MessageText.text = "You can controle\nthe arrow!";
            }
        }

        private void DoAnimation()
        {
            _sequence = DOTween.Sequence();

            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.transform.localScale = Vector3.one;

            _sequence.Append(tutorialComponent.Hand.transform.DOScale(0.7f, 0.1f));
            _sequence.SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }

        private void DoTutorialAudio()
        {
            _gameState.Value.AudioPack.AudioSnapshots.TutorialPause.TransitionTo(1f);
        }

        private void DoNormalAudio()
        {
            _gameState.Value.AudioPack.AudioSnapshots.Normal.TransitionTo(1f);
        }

        private void PrematureEndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            Tutorial.StageIsEnable = false;

            Tutorial.ArrowControle.ResetStage();
            ResetStage();
        }

        private void ResetStage()
        {
            _isEnabledUI = false;
        }

        private void EndStage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_gameState.Value.InterfaceEntity);

            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.MessageText.gameObject.SetActive(false);

            _sequence.Kill();
            tutorialComponent.Hand.transform.localScale = Vector3.one;

            Tutorial.StageIsEnable = false;

            Tutorial.SetNextStage(_gameState);
        }
    }
}