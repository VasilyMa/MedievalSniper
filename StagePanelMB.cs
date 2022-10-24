using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Client 
{
    public class StagePanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        [SerializeField] private Transform _holderLevels;
        [SerializeField] private Transform _currentLevel;

        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
        }
        public void OpenLevelMenu()
        {
            foreach (var item in SaveManager.GetData().Levels)
            {
                var level = Instantiate(_state.InterfaceConfig.levelPrefab, _holderLevels);
                if (SceneManager.GetActiveScene().buildIndex == item.ID)
                {
                    _currentLevel = level.transform;
                }
                var Info = level.GetComponent<LevelMB>();
                if (item.isOpen) 
                    Info.LoadLevel(item.LevelRating, item.ID, _state, _holderLevels);
                else 
                    Info.UpdateInfo(_state.InterfaceConfig.ClosedLevel);
            }
            var posY = _holderLevels.transform.GetComponent<RectTransform>().position.y;
            
        }
        public void ClosePanel()
        {
            SaveManager.SaveSceneNumber(SceneManager.GetActiveScene().buildIndex);
            this.transform.GetChild(0).transform.DOScale(0, 0.5f).OnComplete(()=>RemovePanels());
        }
        void RemovePanels()
        {
            for (int i = 0; i < _holderLevels.childCount; i++)
            {
                Destroy(_holderLevels.GetChild(i).gameObject);
            }
        }
        public void LoadLevel()
        {
            SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
        }
    }
}
public enum LevelStage { zeroLevel = 0, firstLevel = 1, secondLevel = 2, thirdLevel = 3, fourthLevel = 4, fifthLevel = 5, sixthLevel = 6 }

