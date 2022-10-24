using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class InitSniper : MonoBehaviour, IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<View> _viewPool = default;
        readonly EcsPoolInject<Pullable> _pullingBowstringPool = default;
        readonly EcsPoolInject<Killable> _killablePool = default;
        readonly EcsPoolInject<GivableScores> _givableScoresPool = default;
        readonly EcsPoolInject<ScoreComponent> _scoreComponentPool = default;
        readonly EcsPoolInject<Outlinable> _outlinablePool = default;
        readonly EcsPoolInject<SniperComponent> _sniperPool = default;
        readonly EcsPoolInject<AudioComponent> _audioPool = default;

        private int _sniperEntity;
        private SniperMB _sniperMB;

        public void Init(IEcsSystems systems)
        {
            _sniperEntity = _world.Value.NewEntity();

            _gameState.Value.SniperEntity = _sniperEntity;

            _sniperMB = GameObject.FindObjectOfType<SniperMB>();

            InitPullingBowstring();

            InitView();

            InitKillableGivableOutlinable();

            InitSniperData();

            InitAudioComponent();
        }

        private void InitPullingBowstring()
        {
            ref var pullingBowstringComponent = ref _pullingBowstringPool.Value.Add(_sniperEntity);

            pullingBowstringComponent.PullongUpCurve = _gameState.Value.PullingBowstringUpCurve;
            pullingBowstringComponent.PullongDownCurve = _gameState.Value.PullingBowstringDownCurve ?? _gameState.Value.PullingBowstringUpCurve;

            pullingBowstringComponent.CurrentStrength = 0;
            pullingBowstringComponent.isPullingUp = true;

            pullingBowstringComponent.SniperChangeIKStates = _sniperMB.GetComponentInChildren<SniperChangeIKStates>();
            pullingBowstringComponent.SniperChangeIKStates.GameState = _gameState.Value;
        }

        private void InitView()
        {
            ref var viewComponent = ref _viewPool.Value.Add(_sniperEntity);
            viewComponent.EntityNumber = _sniperEntity;
            viewComponent.GameObject = _sniperMB.gameObject;
            viewComponent.Transform = _sniperMB.gameObject.transform;
            viewComponent.LayerBeforeDeath = _sniperMB.gameObject.layer;
            viewComponent.Animator = _sniperMB.GetComponentInChildren<Animator>();
            viewComponent.EcsInfoMB = _sniperMB.gameObject.GetComponent<EcsInfoMB>();
            viewComponent.EcsInfoMB.Init(_world, _sniperEntity);
        }

        private void InitKillableGivableOutlinable()
        {
            var killableTargetMB = _sniperMB.GetComponent<KillableTargetMB>();

            ref var killableComponent = ref _killablePool.Value.Add(_sniperEntity);
            killableComponent.MainBody = _sniperMB.gameObject;
            killableComponent.RigidbodysBones = killableTargetMB.RigidbodysBones;
            killableComponent.Type = killableTargetMB.ObjectType;

            ref var givableScoresComponent = ref _givableScoresPool.Value.Add(_sniperEntity);
            givableScoresComponent.Value = Scores.Objects.GetValueFromObjectType(killableTargetMB.ObjectType);

            ref var scoreComponentPool = ref _scoreComponentPool.Value.Add(_sniperEntity);
            scoreComponentPool.Score = givableScoresComponent.Value;

            ref var outlinableComponent = ref _outlinablePool.Value.Add(_sniperEntity);
            outlinableComponent.Outline = killableTargetMB.GetComponent<Outline>();
        }
        
        private void InitSniperData()
        {
            ref var sniperComponent = ref _sniperPool.Value.Add(_sniperEntity);
            sniperComponent.SkinnedMeshRenderer = FindObjectOfType<SkinTag>().GetComponent<SkinnedMeshRenderer>();
            sniperComponent.SkinnedMeshRenderer.sharedMesh = _gameState.Value.SkinData.Mesh;
        }

        private void InitAudioComponent()
        {
            ref var audioComponent = ref _audioPool.Value.Add(_sniperEntity);

            if (_sniperMB.TryGetComponent<AudioSource>(out var audioSource))
            {
                audioComponent.AudioSource = audioSource;
            }
            else
            {
                Debug.LogError($"On {_sniperMB} missed AudioSource component!");
            }
        }
    }
}