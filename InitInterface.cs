using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    sealed class InitInterface : MonoBehaviour, IEcsInitSystem
    {
        readonly EcsWorldInject _world;

        readonly EcsSharedInject<GameState> _gameState;

        readonly EcsPoolInject<InterfaceComponent> _interfacePool = default;
        readonly EcsPoolInject<InterfaceComboComponent> _interfaceComboPool = default;
        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;

        int _interfaceEntity;

        public void Init (IEcsSystems systems)
        {
            _interfaceEntity = _world.Value.NewEntity();

            _gameState.Value.InterfaceEntity = _interfaceEntity;

            ref var interfaceComponent = ref _interfacePool.Value.Add(_interfaceEntity);

            interfaceComponent.MainCanvas = FindObjectOfType<MainCanvasMB>().gameObject;
            interfaceComponent.CanvasTMPro = FindObjectOfType<CanvasTMPro>();

            interfaceComponent.StrengthBarMB = FindObjectOfType<StrengthBarMB>();
            interfaceComponent.TargetStrengthBar = FindObjectOfType<TargetStrengthMB>().transform;
            interfaceComponent.StrengthBarSlider = interfaceComponent.StrengthBarMB.Slider;

            interfaceComponent.SlowMotionPanelMB = FindObjectOfType<SlowMotionPanelMB>();
            interfaceComponent.FloatingJoystick = FindObjectOfType<FloatingJoystick>();
            interfaceComponent.WinPanelMB = FindObjectOfType<WinPanelMB>();
            interfaceComponent.StorePanelMB = FindObjectOfType<StorePanelMB>();
            interfaceComponent.StagePanelMB = FindObjectOfType<StagePanelMB>();
            interfaceComponent.MainMenuMB = FindObjectOfType<MainMenuMB>();
            interfaceComponent.LosePanelMB = FindObjectOfType<LosePanelMB>();
            interfaceComponent.EventSystem = FindObjectOfType<EventSystem>();
            interfaceComponent.GraphicRaycaster = FindObjectOfType<GraphicRaycaster>();
            interfaceComponent.ComboSystemMB = FindObjectOfType<ComboSystemMB>();
            interfaceComponent.ResourcesMB = FindObjectOfType<ResourcesMB>();
            interfaceComponent.SettingsPanelMB = FindObjectOfType<SettingsPanelMB>();
            interfaceComponent.PlayPanelMB = FindObjectOfType<PlayPanelMB>();

            interfaceComponent.PlayPanel = interfaceComponent.MainCanvas.transform.GetChild(0);

            FindComboSystem();
            InitSlowMo(ref interfaceComponent);
            InitWinPanel(ref interfaceComponent);
            InitStorePanel(ref interfaceComponent);
            InitLosePanel(ref interfaceComponent);
            InitStagePanel(ref interfaceComponent);
            InitMainMenu(ref interfaceComponent);
            InitStrengthBar(ref interfaceComponent);
            InitComboSystemMB(ref interfaceComponent);
            InitResourcesMB(ref interfaceComponent);
            InitSettingsMB(ref interfaceComponent);
            InitPlayPanelMB(ref interfaceComponent);
            InitTutorialsElements();

            interfaceComponent.FloatingJoystick.gameObject.SetActive(false);

            if (Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
            {
                TutorialStart();
            }
            else
            {
                UntutorialStart();
            }
        }

        

        private void FindComboSystem()
        {
            ref var interfaceComboComponent = ref _interfaceComboPool.Value.Add(_interfaceEntity);
            interfaceComboComponent.TimerText = FindObjectOfType<ComboSystemMB>().TimerText;
            interfaceComboComponent.TimerBarImage = FindObjectOfType<ComboSystemMB>().TimerBarImage;
            //interfaceComboComponent.TimerBarTransform = FindObjectOfType<ComboSystemMB>().RectTransform;
            //interfaceComboComponent.TimerBarTransform.gameObject.SetActive(false);
        }
        void InitComboSystemMB(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.ComboSystemMB.Init(_world.Value, _gameState.Value);
            interfaceComponent.ComboSystemMB.HolderCombo.gameObject.SetActive(false);
        }

        void InitSlowMo(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.SlowMotionPanelMB.Init(_world.Value, _gameState.Value);
            interfaceComponent.SlowMotionPanelMB.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        
        void InitStorePanel(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.StorePanelMB.Init(_world.Value, _gameState.Value);
            if(Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
                interfaceComponent.MainMenuMB.GetHodlerSkin().gameObject.SetActive(false);
            //interfaceComponent.StorePanelMB.transform.GetChild(0).gameObject.SetActive(false);
        }

        private void InitWinPanel(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.WinPanelMB.Init(_world.Value, _gameState.Value);
        }
        private void InitLosePanel(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.LosePanelMB.Init(_world.Value, _gameState.Value);
        }
        private void InitStagePanel(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.StagePanelMB.Init(_world.Value, _gameState.Value);
            if (Tutorial.CurrentStage == Tutorial.Stage.FirstLaunching)
                interfaceComponent.MainMenuMB.GetHolderLevel().gameObject.SetActive(false);
        }
        private void InitMainMenu(ref InterfaceComponent interfaceComponent)
        {

            interfaceComponent.MainMenuMB.Init(_world.Value, _gameState.Value);
        }
        private void InitStrengthBar(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.StrengthBarMB.Init(_world.Value, _gameState.Value);
        }

        private void InitResourcesMB(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.ResourcesMB.Init(_world.Value, _gameState.Value);
            interfaceComponent.ResourcesMB.UpdateCoin();
        }
        private void InitSettingsMB(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.SettingsPanelMB.Init(_world.Value, _gameState.Value);
        }
        private void InitPlayPanelMB(ref InterfaceComponent interfaceComponent)
        {
            interfaceComponent.PlayPanelMB.Init(_world.Value, _gameState.Value);
            interfaceComponent.PlayPanel.gameObject.SetActive(false); 
        }

        private void InitTutorialsElements()
        {
            ref var interfaceComp = ref _interfacePool.Value.Get(_interfaceEntity);
            ref var tutorialComponent = ref _tutorialPool.Value.Add(_interfaceEntity);

            FindPanel();
            FindHand();
            FindFocus();
            FindMessage();
            FindMessageRectTransform();
            FindMessageText();

            DisableElements();
        }

        private void FindPanel()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.Panel = FindObjectOfType<TutorialPanelMB>().transform;
        }

        private void FindHand()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.Hand = tutorialComponent.Panel.GetComponentInChildren<TutorialHandMB>().transform;
        }

        private void FindFocus()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.Focus = tutorialComponent.Panel.GetComponentInChildren<TutorialFocusMB>().transform;
        }

        private void FindMessage()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.Message = tutorialComponent.Panel.GetComponentInChildren<TutorialMessageMB>().transform;
        }

        private void FindMessageRectTransform()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.MessageRectTransform = tutorialComponent.Message.GetComponent<RectTransform>();
        }

        private void FindMessageText()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.MessageText = tutorialComponent.Message.GetComponent<Text>();
        }

        private void DisableElements()
        {
            ref var tutorialComponent = ref _tutorialPool.Value.Get(_interfaceEntity);
            tutorialComponent.Hand.gameObject.SetActive(false);
            tutorialComponent.Focus.gameObject.SetActive(false);
            tutorialComponent.Message.gameObject.SetActive(false);
        }

        private void TutorialStart()
        {
            /*ref var interfaceComp = ref _interfacePool.Value.Get(_interfaceEntity);
            _playDeckPool.Value.Add(_world.Value.NewEntity());
            interfaceComp.Hide.gameObject.SetActive(false);
            interfaceComp.HolderCards.gameObject.SetActive(true);
            interfaceComp.MenuHolder.gameObject.SetActive(false);
            interfaceComp.Resources.gameObject.SetActive(true);
            interfaceComp.DeckHolder.gameObject.SetActive(false);
            interfaceComp.BiomHolder.gameObject.SetActive(false);
            interfaceComp.Resources.UpdatePlayerCoinAmount();
            interfaceComp.HolderCards.transform.DOMove(GameObject.Find("TargetCardPanel").transform.position, 1f, false);
            interfaceComp.Progress.transform.GetChild(0).transform.DOMove(GameObject.Find("TargetProgress").transform.position, 1f, false);*/
        }

        private void UntutorialStart()
        {
            /*ref var interfaceComp = ref _interfacePool.Value.Get(_interfaceEntity);
            interfaceComp.MenuHolder.gameObject.SetActive(true);
            _state.Value.HubSystems = true;
            _state.Value.PreparedSystems = false;
            _state.Value.FightSystems = false;
            interfaceComp.Resources.UpdatePlayerCoinAmount();*/
        }
    }
}