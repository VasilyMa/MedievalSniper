using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client {
    public class WinPanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        private EcsPool<Pullable> _pullablePool = default;
        private EcsPool<VibrationEvent> _vibrationEventPool = default;
        private EcsPool<SoundSystemComponent> _soundSystemPool = default;
        private int _reward;

        private float _ratingAmount;
        private float _killCount;
        private float _environmentCount;
        private float _multiplyForUpdateScores = 0;
        private float _multiplyStep = 10;
        private bool _isRating = false;
        private bool _isKillEnvironment = false;

        [SerializeField] private Text _powerMultiply;
        [SerializeField] private Transform targetPositionWinInfo;
        [SerializeField] private Text _buttonText;
        [SerializeField] private Transform _winPanelHolder;
        [SerializeField] private Transform _hornLeft;
        [SerializeField] private Transform _hornRight;
        [SerializeField] private Transform _header;

        [SerializeField] private Transform _winInfoHolder;
        [SerializeField] private Text _killAmount;
        [SerializeField] private Text _environmentAmount;

        [SerializeField] private Transform _rewardHolder;
        [SerializeField] private Transform _boxReward;
        [SerializeField] private Transform _unboxReward;
        [SerializeField] private Image _rewardImage;
        [SerializeField] private Text _rewardAmount;

        private WinStage winStage = WinStage.TotalScore;
        private LevelReward _rewardLevel;

        [SerializeField] private Transform _starsHolder;
        [SerializeField] private Transform _firstStar;
        [SerializeField] private Transform _secondStar;
        [SerializeField] private Transform _lastStar;
        [SerializeField] private Text _ratingCount;

        [SerializeField] private Transform _particleStar_01;
        [SerializeField] private Transform _particleStar_02;
        [SerializeField] private Transform _particleStar_03;
        [SerializeField] private Transform _particleUnboxing;
        private int _pullEntity;

        [Space]
        public float SecondsAfterCollisonEnemy;
        public float SecondsAfterStartAnimationHero;

        private Ease ease = Ease.OutElastic;
        private float time = 1f;
        private enum LevelReward { empty = 0, first = 1, second = 2, third = 3}
        private enum WinStage { TotalScore, KillsAmount, Reward, NextLevel} 
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _pullablePool = _world.GetPool<Pullable>();
            _vibrationEventPool = _world.GetPool<VibrationEvent>();
            _soundSystemPool = _world.GetPool<SoundSystemComponent>();
            _firstStar.GetComponent<Image>().sprite = _state.InterfaceConfig.EmptyStar;
            _secondStar.GetComponent<Image>().sprite = _state.InterfaceConfig.EmptyStar;
            _lastStar.GetComponent<Image>().sprite = _state.InterfaceConfig.EmptyStar;

            _rewardAmount.gameObject.SetActive(false);

        }
        public Transform GetHolder()
        {
            return _winPanelHolder;
        }
        /// <summary>
        /// Start the win event and throw are reward coin amount
        /// </summary>
        /// <param reward amount="value"></param>
        public void StartWinEvent(int rewardAmount, int pullEntity)
        {
            _pullEntity = pullEntity;
            _reward = rewardAmount;
            _hornLeft.DOScale(0.5f, 0.15f).OnComplete(() => PlayConfettiLeft());
            _hornRight.DOScale(0.5f, 0.15f).OnComplete(() => PlayConfettiRight());
            _hornLeft.DOScale(1f, 0.5f).SetEase(ease);
            _hornRight.DOScale(1f, 0.5f).SetEase(ease);
            _header.GetChild(0).transform.DOScale(1, time).SetEase(ease).OnComplete(() => OpenStarsComplete());
            _hornLeft.DORotate(new Vector3(0, 0, 0), time, RotateMode.Fast).SetEase(ease);
            _hornRight.DORotate(new Vector3(0, 0, 0), time, RotateMode.Fast).SetEase(ease);
        }
        void PlayConfettiLeft()
        {
            _hornLeft.GetChild(0).gameObject.SetActive(true);
        }
        void PlayConfettiRight()
        {
            _hornRight.GetChild(0).gameObject.SetActive(true);
        }
        void OpenStarsComplete()
        {
            ref var pullComp = ref _pullablePool.Get(_pullEntity);
            if (_starsHolder == null)
            {
                Debug.LogError($"Holder {_starsHolder} is missing");
                return;
            }
            _starsHolder.DOScale(1f, 0.5f).SetEase(Ease.InCubic);
            _powerMultiply.text = $"Skill check multiply\n{Scores.GetMultiplyFromPullingStrength(pullComp.CurrentStrength) * 100}%";
            _isRating = true;
        }
        void StarsUpdate()
        {
            var result = _ratingAmount / _state.TotalMaxScoreOnLevel;
            if (result == 0)
            {
                _rewardLevel = LevelReward.empty;
                return;
            }
            if (result >= 0.2f && result < 0.4f)
            {
                _firstStar.GetComponent<Image>().sprite = _state.InterfaceConfig.HalfStarLeft;
                return;
            }
            if (result >= 0.4f && result < 0.6f)
            {
                _particleStar_01.gameObject.SetActive(true);
                _rewardLevel = LevelReward.first;
                StartCoroutine(FirstStarActive());
                return;
            }
            if (result >= 0.6f && result < 0.7f)
            {
                _secondStar.GetComponent<Image>().sprite = _state.InterfaceConfig.HalfStarLeft;
                return;
            }
            if (result >= 0.7f && result < 0.8f)
            {
                _particleStar_02.gameObject.SetActive(true);
                _rewardLevel = LevelReward.second;
                StartCoroutine(SecondStarActive());
                return;
            }
            if (result >= 0.8f && result < 0.9f)
            {
                _lastStar.GetComponent<Image>().sprite = _state.InterfaceConfig.HalfStarLeft;
                return;
            }
            if (result >= 0.9f)
            {
                _particleStar_03.gameObject.SetActive(true);
                _rewardLevel = LevelReward.third;
                StartCoroutine(LastStarActive());
                return;
            }
        }
        private IEnumerator FirstStarActive()
        {
            yield return new WaitForSeconds(1f);
            _firstStar.GetComponent<Image>().sprite = _state.InterfaceConfig.FullStar;
        }
        private IEnumerator SecondStarActive()
        {
            yield return new WaitForSeconds(1f);
            _secondStar.GetComponent<Image>().sprite = _state.InterfaceConfig.FullStar;
        }
        private IEnumerator LastStarActive()
        {
            yield return new WaitForSeconds(1f);
            _lastStar.GetComponent<Image>().sprite = _state.InterfaceConfig.FullStar;
        }
        void RatingUpdate()
        {
            if (!_isRating) return;
            _ratingAmount += Time.deltaTime * _multiplyForUpdateScores;
            if (_ratingAmount < _state.FinalPlayerScore)
            {
                StarsUpdate();
                _multiplyForUpdateScores += _multiplyStep;
                float rating = Mathf.FloorToInt(_ratingAmount);
                _ratingCount.text = string.Format("{00}", rating);
            }
            else
            {
                _ratingCount.transform.DOPunchScale(new Vector3(1.15f, 1.15f, 1.15f), 0.25f, 3, 2).SetEase(Ease.InCirc).OnComplete(() => StartCoroutine(WaitToOpenInfo()));
                _ratingCount.text = _state.FinalPlayerScore.ToString();
                _ratingAmount = _state.FinalPlayerScore;
                StarsUpdate();
                _isRating = false;
            }
        }
        private IEnumerator WaitToOpenInfo()
        {
            yield return new WaitForSeconds(1);
            _starsHolder.gameObject.SetActive(false);
            OpenInfo();
        }
        void OpenInfo()
        {
            if (winStage == WinStage.Reward || winStage == WinStage.NextLevel)
                return;
            winStage = WinStage.KillsAmount;
            _winInfoHolder.GetComponent<RectTransform>().DOMove(targetPositionWinInfo.position, 1f, false);
            for (int i = 0; i < _winInfoHolder.childCount; i++)
            {
                _winInfoHolder.GetChild(i).GetComponent<Text>().DOFade(1, 0.25f);
            }
            _isKillEnvironment = true;
            _multiplyForUpdateScores = 0;
        }
        void KillEnvironmentUpdate()
        {
            if (!_isKillEnvironment) return;
            _killCount += Time.deltaTime * _multiplyForUpdateScores;
            _environmentCount += Time.deltaTime * _multiplyForUpdateScores;
            _multiplyForUpdateScores += 0.15f;
            if (_killCount < _state.KillsOnLevel)
            {
                float kills = Mathf.FloorToInt(_killCount);
                _killAmount.text = string.Format("{00}", kills);
            }
            if (_environmentCount < _state.EnvironmentOnLevel)
            {
                float environment = Mathf.FloorToInt(_environmentCount);
                _environmentAmount.text = string.Format("{00}", environment);
            }
            if (_killCount > _state.KillsOnLevel && _environmentCount > _state.EnvironmentOnLevel)
            {
                _killAmount.text = _state.KillsOnLevel.ToString();
                _killCount = _state.KillsOnLevel;
                _environmentAmount.text = _state.EnvironmentOnLevel.ToString();
                _environmentCount = _state.EnvironmentOnLevel;
                _isKillEnvironment = false;
                _killAmount.transform.DOPunchScale(new Vector3(1.15f, 1.15f, 1.15f), 0.25f, 3, 2).SetEase(Ease.InCirc);
                _environmentAmount.transform.DOPunchScale(new Vector3(1.15f, 1.15f, 1.15f), 0.25f, 3, 2).SetEase(Ease.InCirc).OnComplete(()=>StartCoroutine(WaitToReward()));
            }
        }
        private IEnumerator WaitToReward()
        {
            yield return new WaitForSeconds(1);
            _winInfoHolder.gameObject.SetActive(false);
            OpenReward();
        }

        void OpenReward()
        {
            if(_reward <= 0)
                SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
            var result = GetRewardInfo((float)_state.FinalPlayerScore / _state.TotalMaxScoreOnLevel);
            winStage = WinStage.Reward;
            switch (result)
            {
                case LevelReward.empty:
                    _rewardImage.sprite = _state.InterfaceConfig.EmptyLevel;
                    break;
                case LevelReward.first:
                    _rewardImage.sprite = _state.InterfaceConfig.FirstLevel;
                    break;
                case LevelReward.second:
                    _rewardImage.sprite = _state.InterfaceConfig.SecondLevel;
                    break;
                case LevelReward.third:
                    _rewardImage.sprite = _state.InterfaceConfig.ThirdLevel;
                    break;
                default:
                    break;
            }
            _rewardHolder.DOScale(1, 0.5f).SetEase(Ease.OutCirc).OnComplete(()=>RewardIsOpen());
        }
        void RewardIsOpen()
        {
            _buttonText.text = "Open";
        }
        public void GetReward()
        {
            winStage = WinStage.NextLevel;
            ref var vibrationComp = ref _vibrationEventPool.Add(_world.NewEntity());
            vibrationComp.Vibration = VibrationEvent.VibrationType.HeavyImpact;
            _buttonText.text = "Next";
            _particleUnboxing.gameObject.SetActive(true);
            _boxReward.gameObject.SetActive(false);
            _unboxReward.gameObject.SetActive(true);
            _rewardAmount.gameObject.SetActive(true);
            _rewardAmount.text = _reward.ToString(); //to do show reward amount
            _soundSystemPool.Get(_state.SoundSystemEntity).LevelEndSource.PlayOneShot(_state.AudioPack.AudioStorage.LevelEnd.GoldReward);
            StartCoroutine(WaitToNext());
        }
        private IEnumerator WaitToNext()
        {
            yield return new WaitForSeconds(2.5f);
            SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
        }

        public void ButtonNextEvent()
        {
            var result = _state.FinalPlayerScore / _state.TotalMaxScoreOnLevel;
            _rewardLevel = GetRewardInfo(result);

            StopAllCoroutines();
            switch (winStage) 
            {
                case WinStage.TotalScore:
                    winStage = WinStage.Reward;
                    _starsHolder.gameObject.SetActive(false);
                    OpenReward();
                    break;
                case WinStage.KillsAmount:
                    winStage = WinStage.Reward;
                    _winInfoHolder.gameObject.SetActive(false);
                    OpenReward();
                    break;
                case WinStage.Reward:
                    winStage = WinStage.NextLevel;
                    GetReward();
                    break;
                case WinStage.NextLevel:
                    SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
                    break;
                default:
                    break;
            }
        }
        private void Update()
        {
            RatingUpdate();
            KillEnvironmentUpdate();
        }
        LevelReward GetRewardInfo(float result)
        {
            if (result == 0)
            {
                return LevelReward.empty;
            }
            if (result >= 0.4f && result < 0.7f)
            {
                return LevelReward.first;
            }
            if (result >= 0.6f && result < 0.9f)
            {
                return LevelReward.second;
            }
            if (result >= 0.9f)
            {
                return LevelReward.third;
            }
            return LevelReward.empty;
        }
    }
}

