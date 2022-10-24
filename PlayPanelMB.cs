using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;

namespace Client 
{
    public class PlayPanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<PlayEvent> _playPool = default;
        private EcsPool<InterfaceComponent> _interfacePool = default;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _playPool = _world.GetPool<PlayEvent>();
            _interfacePool = _world.GetPool<InterfaceComponent>();
        }
        public void StartGame()
        {
            ref var playComp = ref _playPool.Add(_world.NewEntity());
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.StrengthBarMB.StartPlay();
            interfaceComp.MainMenuMB.StartPlay();

            TutorialSetIsHoldedFirstTime();

            TutorialSetSkillCheckIsArrived();
        }
        private void TutorialSetIsHoldedFirstTime()
        {
            if (Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
            {
                Tutorial.FirstLaunching.SetIsHolded();
            }
        }

        private void TutorialSetSkillCheckIsArrived()
        {
            if (Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
            {
                Tutorial.FirstLaunching.SetSkillCheckIsArrived();
            }
        }
    }
}
