using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using DG.Tweening;
using System.Text;

namespace Client
{
    sealed class ShowScoresEventSystem :  IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsFilterInject<Inc<ShowScoresEvent>> _scoreEventFilter = default;

        readonly EcsPoolInject<ShowScoresEvent> _showScoreEventPool = default;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private int _eventEntity = GameState.NULL_ENTITY;

        private float _effectRadius = 250;

        private float _effectRandomDirection;

        private Sequence _tweenSequence;

        //private DoTweenGo _doTweenGo;
        private StringBuilder _stringBuilder;

        private ScorePrefabMB _textPrefab;

        private bool _isFirstWork = true;

        public void Run (IEcsSystems systems)
        {
            if (_isFirstWork)
            {
                //_tweenSequence = DOTween.Sequence();
                //_doTweenGo = new DoTweenGo();
                _stringBuilder = new StringBuilder(4);

                _isFirstWork = false;
            }

            foreach (int entity in _scoreEventFilter.Value)
            {
                _eventEntity = entity;
                
                ref var showScoreEvent = ref _showScoreEventPool.Value.Get(_eventEntity);

                ref var interfaceComponent = ref _interfacePool.Value.Get(_gameState.Value.InterfaceEntity);

                foreach (var pools in _gameState.Value.ActivePools)
                {
                    if (pools is ScoresEffectPool)
                    {
                        if (pools.GetFromPool().TryGetComponent<ScorePrefabMB>(out var textPrefab))
                        {
                            _textPrefab = textPrefab;
                        }

                        if (_textPrefab.transform.position != interfaceComponent.MainCanvas.transform.position)
                        {
                            _textPrefab.transform.position = interfaceComponent.MainCanvas.transform.position;
                        }
                    }
                }

                ResetTextAfterPool();

                if (showScoreEvent.IsHeadShot) _stringBuilder.Append("HEADSHOT\n");

                if (showScoreEvent.ScoreValue > 0)
                {
                    _stringBuilder.Append($"+{showScoreEvent.ScoreValue}");
                }
                else
                {
                    _stringBuilder.Append($"{showScoreEvent.ScoreValue}");
                }

                _textPrefab.Text.SetText(_stringBuilder.ToString());
                //_textPrefab.SimpleText.text = _stringBuilder.ToString();
                //_textPrefab.Shadow.text = _stringBuilder.ToString();
                _textPrefab.ParticleEffect.gameObject.SetActive(false);

                //_doTweenGo.TextPrefab = _textPrefab;

                ref var CameraComponent = ref _cameraPool.Value.Get(_gameState.Value.CameraEntity);

                DoEffect(ref CameraComponent.Camera);

                DeleteEvent();
            }
        }

        private void ResetTextAfterPool()
        {
            if (_textPrefab.transform.localScale != Vector3.one)
            {
                _textPrefab.transform.localScale = Vector3.one;
            }
            
            if (_textPrefab.Text.alpha != 1)
            {
                _textPrefab.Text.alpha = 1;
            }
        }

        private void DoEffect(ref Camera camera)
        {
            Vector3 cam = camera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            _effectRandomDirection = Random.Range(0, -180);

            GameObject Particleeffect = _textPrefab.ParticleEffect;

            float x = Mathf.Cos(_effectRandomDirection) * _effectRadius;
            float y = Mathf.Sin(_effectRandomDirection) * _effectRadius;

            Vector3 randomPoint = cam + new Vector3(x, y);
            var _tweenSequence = DOTween.Sequence();

            _tweenSequence
                .Append(_textPrefab.transform.DOLocalMove(randomPoint, _textPrefab.TimeDuration * 0.1f))
                .Join(_textPrefab.transform.DOScale(8f, _textPrefab.TimeDuration * 0.1f))
                .SetEase(Ease.InCubic)
                .InsertCallback(_textPrefab.TimeDuration * 0.1f, () => PlayParticle())
                .Append(_textPrefab.transform.DOScale(10f, _textPrefab.TimeDuration * 0.9f))
                .Join(_textPrefab.transform.DOPunchScale(new Vector3(12f, 12f, 1), _textPrefab.TimeDuration * 0.9f, 3, 2))
                .Join(_textPrefab.Text.DOFade(0, _textPrefab.TimeDuration * 0.9f))
                //.Join(_textPrefab.Shadow.DOFade(0, _textPrefab.TimeDuration * 0.9f))
                .SetEase(Ease.InCubic)
            .OnKill(()=>DisableText());
        }

        private void DisableText()
        {
            _textPrefab.gameObject.SetActive(false);
            _textPrefab.ParticleEffect.gameObject.SetActive(false);
        }

        private void PlayParticle()
        {
            _textPrefab.ParticleEffect.gameObject.SetActive(true); 
            if (_textPrefab.ParticleEffect.transform.GetChild(0).TryGetComponent<ParticleSystem>(out var particleSystem))
            {
                particleSystem.Play();
            }
        }

        private void DeleteEvent()
        {
            _showScoreEventPool.Value.Del(_eventEntity);

            _stringBuilder.Clear();

            _eventEntity = GameState.NULL_ENTITY;
        }
    }
}