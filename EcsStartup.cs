using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.IO;

namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        public Pools[] Pools;

        public AnimationCurve PullingBowstringUpCurve;
        public AnimationCurve PullingBowstringDownCurve;
        [Space]
        public int LevelReward = 100;

        public DataSave saves = new DataSave();
        private EcsWorld _world;
        private GameState _gameState;
        private EcsSystems _tutorialSystems, _interfaceSystems, _initBeforePlayingSystems, _systemSystems, _playbleSystems, _pullingSystems, _launchingSystems, _afterLaunchingSystems, _endSystems;

        [Space]
        public float RotateSpeed;

        public InterfaceConfig interfaceConfig;
        public ProductConfig productConfig;

        [Space]
        public AmbientType AmbientType;
        public AudioPack AudioPack;

        private void Awake()
        {
            
        }

        void Start ()
        {
            _world = new EcsWorld ();

            _gameState = new GameState(_world, RotateSpeed, interfaceConfig, productConfig, PullingBowstringUpCurve, PullingBowstringDownCurve, LevelReward, Pools, AudioPack, AmbientType);

            LoadGame();
            _interfaceSystems = new EcsSystems(_world, _gameState);
            _initBeforePlayingSystems = new EcsSystems(_world, _gameState);
            _systemSystems = new EcsSystems(_world, _gameState);
            _playbleSystems = new EcsSystems(_world, _gameState);
            _pullingSystems = new EcsSystems(_world, _gameState);
            _launchingSystems = new EcsSystems(_world, _gameState);
            _afterLaunchingSystems = new EcsSystems(_world, _gameState);
            _endSystems = new EcsSystems(_world, _gameState);
            _tutorialSystems = new EcsSystems(_world, _gameState);

            _tutorialSystems
                .Add(new FirstLaunching())
                .Add(new ArrowControle())
                ;


            _initBeforePlayingSystems
                .Add(new InitSDKLevelTimer())
                .Add(new InitCamera())
                .Add(new InitInterface())
                .Add(new InitFirstVibrations())

                .Add(new InitBow())
                .Add(new InitArrow())
                .Add(new InitSniper())

                .Add(new InitKillableTargets())
                .Add(new InitDestructibleObjects())

                .Add(new InitPools())

                .Add(new CalculateMaxScoresOnLevel())

                .Add(new InitSoundSystem())

                //.Add(new InitEnemyPointer())
                ;

            _systemSystems
                .Add(new ChangeAmbientEventSystem())
                .Add(new VibrationEventSystem())
                .Add(new SDKLevelTimerSystem())
                ;

            _playbleSystems
                .Add(new EnemyPatrolling())

                .Add(new CameraFlyingAroundLevel())
                ;

            _pullingSystems
                .Add(new PullingBowstring())
                .Add(new PullingStrengthBar())
                ;

            _launchingSystems
                .Add(new ArrowLaunchung())
                ;

            _afterLaunchingSystems
                .Add(new ArrowMoving())
                .Add(new ArrowController())

                //.Add(new ResetArrowDirection())
                .Add(new ArrowFalling())
                .Add(new ArrowSpinning())

                //.Add(new CameraMoveToArrow())
                .Add(new BulletTime())

                .Add(new CollisionEventSystem())
                .Add(new DestroyEventSystem())
                .Add(new KillTargetEventSystem())

                .Add(new PenetrationEventSystem())

                .Add(new ChangeDestructibleOutlineWidth())

                .Add(new StartComboEventSystem())
                .Add(new ComboTimerSystem())

                .Add(new GiveScoreEventSystem())
                .Add(new ShowScoresEventSystem())

                .Add(new EnemyPointerRun())
                ;

            _endSystems
                .Add(new WinSystem())
                .Add(new LoseSystem())
                ;
#if UNITY_EDITOR
            _initBeforePlayingSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
                ;
#endif
            
            InjectAllSystems(_interfaceSystems, _initBeforePlayingSystems, _systemSystems, _playbleSystems, _pullingSystems, _launchingSystems, _afterLaunchingSystems, _endSystems, _tutorialSystems);
            InitAllSystems(_interfaceSystems, _initBeforePlayingSystems, _systemSystems, _playbleSystems, _pullingSystems, _launchingSystems, _afterLaunchingSystems, _endSystems, _tutorialSystems);
        }

        void Update ()
        {
            _interfaceSystems?.Run();
            _initBeforePlayingSystems?.Run();

            if (!Tutorial.isOver())
            {
                _tutorialSystems?.Run();
            }

            _systemSystems?.Run();

            _playbleSystems?.Run();

            if (_gameState.PullingBowstringSystems) _pullingSystems?.Run();
            if (_gameState.LaunchingSystems) _launchingSystems?.Run();
            if (_gameState.AfterLaunchingSystems) _afterLaunchingSystems?.Run();

            _endSystems?.Run();
        }

        void OnDestroy ()
        {
            OnDestroyAllSystems(_interfaceSystems, _initBeforePlayingSystems, _systemSystems, _playbleSystems, _pullingSystems, _launchingSystems, _afterLaunchingSystems, _endSystems, _tutorialSystems);

            if (_world != null)
            {
                _world.Destroy ();
                _world = null;
            }
        }
        private void OnApplicationQuit()
        {
            SaveManager.SaveData();
        }
        private void InjectAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Inject();
            }
        }

        private void InitAllSystems(params EcsSystems[] systems)
        {
            foreach (var system in systems)
            {
                system.Init();
            }
        }

        private void OnDestroyAllSystems(params EcsSystems[] systems)
        {
            for (int i = 0; i < systems.Length; i++)
            {
                if (systems[i] != null)
                {
                    systems[i].Destroy();
                    systems[i] = null;
                }
            }
        }
        void LoadGame()
        {
            if (File.Exists(SaveManager.savePath)) SaveManager.LoadData(_gameState);
            else
            {
                SaveManager.NewSave();
                for (int i = 1; i <= SceneManager.sceneCountInBuildSettings - 1; i++)
                {
                    var newLevel = new LevelData();
                    newLevel.ID = i;
                    if (newLevel.ID == 1)
                    {
                        SaveManager.SaveSceneNumber(1);
                        newLevel.isOpen = true;
                    }
                    else
                        newLevel.isOpen = false;
                    
                    SaveManager.GetData().Levels.Add(newLevel);
                }
                for (int i = 0; i < _gameState.ProductConfig.SkinConfigs.Length; i++)
                {
                    SaveManager.GetData().Skins.Add(_gameState.ProductConfig.SkinConfigs[i].skinData);
                }
            }
            foreach (var item in SaveManager.GetData().Skins)
            {
                if (item.isEquip)
                {
                    _gameState.SkinData = item;
                    break;
                }
            }
            Tutorial.LoadCurrentStage(SaveManager.GetData().TutorialStage);
            saves = SaveManager.GetData();
            if(SceneManager.GetActiveScene().buildIndex != SaveManager.GetData().CurrentSceneNumber) 
                SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
        }
    }
}