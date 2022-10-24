using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Client
{
    sealed class WinSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;

        readonly EcsFilterInject<Inc<WinEvent>> _winFilter = default;
        readonly EcsFilterInject<Inc<Pullable>> _pullFilter = default;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<SoundSystemComponent> _soundSystemPool = default;
        readonly EcsPoolInject<LevelTimer> _levelTimerPool = default;

        private string[] _namesTrigerAnimationArray = new string[7] { "Win0", "Win0", "Win01", "Win01", "Win1", "Win2", "Win3"};

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _winFilter.Value)
            {
                if (Time.timeScale != 1) Time.timeScale = 1;

                AppMetricaEvent();
                if (Tutorial.CurrentStage == Tutorial.Stage.ArrowControle)
                {
                    Tutorial.ArrowControle.SetIsPrematureEnd();
                }

                _gameState.Value.AfterLaunchingSystems = false;
                _gameState.Value.FinalPlayerScore = Scores.CalculateFinalScore(_gameState.Value);

                _gameState.Value.LevelReward = Scores.CalculateMoneyFromPlayerScore(in _gameState.Value, SceneManager.GetActiveScene().buildIndex);

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
                interfaceComponent.StrengthBarMB.HidePanel();
                //interfaceComponent.WinPanelMB.GetHolder().DOScale(1, 0.5f).SetEase(Ease.InCirc).OnComplete(() => StartAnimation());
                interfaceComponent.SlowMotionPanelMB.GetPaticles().gameObject.SetActive(false);
                interfaceComponent.ComboSystemMB.HolderCombo.gameObject.SetActive(false);

                var seq = DOTween.Sequence();
                seq
                    .AppendInterval(interfaceComponent.WinPanelMB.SecondsAfterCollisonEnemy)
                    .AppendCallback(() => MoveToHero())
                    .AppendInterval(interfaceComponent.WinPanelMB.SecondsAfterStartAnimationHero)
                    .Append(interfaceComponent.WinPanelMB.GetHolder().DOScale(1, 0.5f).SetEase(Ease.InCirc).OnKill(() => StartAnimation()));

                
                SaveManager.SaveLevelStage(SceneManager.GetActiveScene().buildIndex, _gameState.Value.FinalPlayerScore, GetLevelStage(), true);
                SaveManager.SavePlayerLevel(SaveManager.GetData().PlayerLevel++);

                if (GetLevelStage() >= LevelStage.fourthLevel)
                {
                    var openedLevel = SaveManager.GetData().Levels.Find(x => x.ID == SceneManager.GetActiveScene().buildIndex + 1);
                    if (openedLevel == null)
                    {
                        SaveManager.SaveSceneNumber(SceneManager.GetActiveScene().buildIndex);
                        _winFilter.Pools.Inc1.Del(entity);
                        return;
                    }
                    foreach (var level in SaveManager.GetData().Levels)
                    {
                        if (openedLevel.ID == level.ID)
                        {
                            if (!level.isOpen)
                                level.isOpen = true;
                            SaveManager.SaveSceneNumber(level.ID);
                            break;
                        }
                    }
                }
                else if(GetLevelStage() < LevelStage.fourthLevel)
                {
                    var openedLevel = SaveManager.GetData().Levels.Find(x => x.ID == SceneManager.GetActiveScene().buildIndex + 1);
                    if (openedLevel.isOpen)
                    {
                        SaveManager.SaveSceneNumber(openedLevel.ID);
                    }
                }

                PlayWinMusic();

                _winFilter.Pools.Inc1.Del(entity);
            }
        }
        private void AppMetricaEvent()
        {
            SaveManager.GetData().SceneCount++;
            var sceneName = SceneManager.GetActiveScene().name;
            var sceneNumber = SceneManager.GetActiveScene().buildIndex;
            var sceneCount = SaveManager.GetData().SceneCount;

            ref var levelTimerComponent = ref _levelTimerPool.Value.Get(_gameState.Value.LevelTimerEntity);
            int levelTimerInt = Mathf.FloorToInt(levelTimerComponent.CurrentValue);


            AzureMetrica.LevelFinish(sceneName, sceneNumber, sceneCount, 0, "Win", levelTimerInt, Mathf.FloorToInt(((float)_gameState.Value.FinalPlayerScore / _gameState.Value.TotalMaxScoreOnLevel)) * 100);
        }

        private void MoveToHero()
        {
            ref var cameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);
            cameraComponent.WinVirtualCamera.Priority = 100;

            ref var viewPoolSniper = ref _viewPool.Value.Get(_gameState.Value.SniperEntity);
            ref var viewPoolBow = ref _viewPool.Value.Get(_gameState.Value.BowEntity);

            viewPoolSniper.Animator.SetTrigger(_namesTrigerAnimationArray[(int)GetLevelStage()]);
            viewPoolBow.GameObject.SetActive(false);
        }

        private void StartAnimation()
        {
            foreach (var entity in _pullFilter.Value)
            {
                var ent = entity; 
                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);
                interfaceComponent.WinPanelMB.StartWinEvent(_gameState.Value.LevelReward, ent);
            }
            // start win event there we need point are reward amount
        }

        private LevelStage GetLevelStage()
        {
            var result = (float)_gameState.Value.FinalPlayerScore / _gameState.Value.TotalMaxScoreOnLevel;
            if (result == 0)
            {
                return LevelStage.zeroLevel;
            }
            if (result >= 0.2f && result < 0.4f)
            {
                return LevelStage.firstLevel;
            }
            if (result >= 0.4f && result < 0.6f)
            {
                return LevelStage.secondLevel;
            }
            if (result >= 0.6f && result < 0.7f)
            {
                return LevelStage.thirdLevel;
            }
            if (result >= 0.7f && result < 0.8f)
            {
                return LevelStage.fourthLevel;
            }
            if (result >= 0.8f && result < 0.9f)
            {
                return LevelStage.fifthLevel;
            }
            if (result >= 0.9f)
            {
                return LevelStage.sixthLevel;
            }
            return LevelStage.zeroLevel;
        }

        private void PlayWinMusic()
        {
            ref var soundSystemComponent = ref _soundSystemPool.Value.Get(_gameState.Value.SoundSystemEntity);
            _gameState.Value.AudioPack.AudioSnapshots.LevelEnd.TransitionTo(0.1f);
            soundSystemComponent.LevelEndSource.PlayOneShot(_gameState.Value.AudioPack.AudioStorage.LevelEnd.Win[3]);
        }
    }
}