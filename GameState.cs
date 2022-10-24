using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Client
{
    public class GameState
    {
        public AnimationCurve PullingBowstringUpCurve;
        public AnimationCurve PullingBowstringDownCurve;

        public Pools[] AllPools;
        public List<Pools> ActivePools;

        public int LevelReward { get; set; }
        public int LevelTimerEntity;

        private EcsWorld _ecsWorld;

        public int TotalMaxScoreOnLevel;
        public int CurrentPlayerScore;
        public int FinalPlayerScore;
        public int KillsOnLevel { get; set; }
        public int EnvironmentOnLevel { get; set; }

        public int TutorialStage;

        public float PullingStrength { get; set; }

        public int ScoreEffectCollision;
        public SkinData SkinData { get; set; }

        #region Entitys
        public const int NULL_ENTITY = -1;
        public int SoundSystemEntity { get; set; }
        public int CameraEntity { get; set; }
        public int InterfaceEntity { get; set; }
        public int SniperEntity { get; set; }
        public int BowEntity { get; set; }
        public int ArrowEntity { get; set; }
        #endregion

        public float RotateSpeed { get; set; }

        #region EcsSystemsToggles
        public bool PullingBowstringSystems { get; set; } = false;
        public bool LaunchingSystems { get; set; } = false;
        public bool AfterLaunchingSystems { get; set; } = false;
        public bool IsEndCameraFly { get; set; } = false;
        #endregion

        public InterfaceConfig InterfaceConfig;
        public ProductConfig ProductConfig;

        #region Services
        public bool EnabledVibration = true;
        #endregion

        #region Audio
        public AudioPack AudioPack { get; set; }
        public AmbientType AmbientType { get; set; }

        public bool IsNeedChangeEmbient = false;
        #endregion

        public GameState(EcsWorld EcsWorld, float Rotate, InterfaceConfig interfaceConfig, ProductConfig productConfig, AnimationCurve pullingBowstringUpCurve, AnimationCurve pullingBowstringDownCurve, int levelReward, Pools[] pools, AudioPack audioPack, AmbientType ambientType)
        {
            _ecsWorld = EcsWorld;
            RotateSpeed = Rotate;

            InterfaceConfig = interfaceConfig;
            ProductConfig = productConfig;

            PullingBowstringUpCurve = pullingBowstringUpCurve;
            PullingBowstringDownCurve = pullingBowstringDownCurve;

            LevelReward = levelReward;

            AllPools = pools;

            AudioPack = audioPack;
            AmbientType = ambientType;
        }

    }
}
