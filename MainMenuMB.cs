using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using DG.Tweening;

namespace Client 
{
    public class MainMenuMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        [SerializeField] private Transform targetButtonSkin;
        [SerializeField] private Transform targetButtonLevel;

        [SerializeField] private Transform holderLevel;
        [SerializeField] private Transform holderSkin;

        private Vector3 defaultPosSkin;
        private Vector3 defaultPosLevel;

        private EcsPool<InterfaceComponent> _interfacePool;
        InterfaceComponent interfaceComponent;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            interfaceComponent = _interfacePool.Get(_state.InterfaceEntity);
            defaultPosSkin = holderSkin.position;
            defaultPosLevel = holderLevel.position;

        }
        public Transform GetHolderLevel()
        {
            return holderLevel;
        }
        public Transform GetHodlerSkin()
        {
            return holderSkin;
        }
        public void OpenSkinStore()
        {
            interfaceComponent.PlayPanel.gameObject.SetActive(false);
            interfaceComponent.StorePanelMB.OpenPanel();
            interfaceComponent.StorePanelMB.transform.GetChild(0).DOScale(1, 0.5f);
            this.transform.GetChild(0).transform.GetChild(0).transform.DOMove(targetButtonLevel.position, 0.5f, false);
            this.transform.GetChild(0).transform.GetChild(1).transform.DOMove(targetButtonSkin.position, 0.5f, false);
        }
        public void OpenLevelSelection()
        {
            interfaceComponent.PlayPanel.gameObject.SetActive(false);
            interfaceComponent.StagePanelMB.OpenLevelMenu();
            interfaceComponent.StagePanelMB.transform.GetChild(0).DOScale(1, 0.5f);
            this.transform.GetChild(0).transform.GetChild(0).transform.DOMove(targetButtonLevel.position, 0.5f, false);
            this.transform.GetChild(0).transform.GetChild(1).transform.DOMove(targetButtonSkin.position, 0.5f, false);
        }
        public void StartPlay()
        {
            this.transform.GetChild(0).transform.GetChild(0).transform.DOMove(targetButtonLevel.position, 0.5f, false);
            this.transform.GetChild(0).transform.GetChild(1).transform.DOMove(targetButtonSkin.position, 0.5f, false);
            interfaceComponent.StorePanelMB.ClosePanel();
            interfaceComponent.StagePanelMB.ClosePanel();
            interfaceComponent.SettingsPanelMB.ClosePanel();
            interfaceComponent.ResourcesMB.MovePanel();
        }
        public void CloseThePanel()
        {
            interfaceComponent.PlayPanel.gameObject.SetActive(true);
            this.transform.GetChild(0).transform.GetChild(0).transform.DOMove(defaultPosLevel, 0.5f, false);
            this.transform.GetChild(0).transform.GetChild(1).transform.DOMove(defaultPosSkin, 0.5f, false);
            interfaceComponent.StagePanelMB.ClosePanel();
            interfaceComponent.StorePanelMB.ClosePanel();
        }
    }
}
