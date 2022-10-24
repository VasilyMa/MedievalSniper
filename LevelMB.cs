using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client 
{
    public class LevelMB : MonoBehaviour
    {
        private GameState gameState;
        private Transform _scenesHolder;
        [SerializeField] private Image _mainImage;
        [SerializeField] private GameObject _firstStar;
        [SerializeField] private GameObject _secondStar;
        [SerializeField] private GameObject _thirdStar;
        [SerializeField] private GameObject _levelNumber;
        [SerializeField] private GameObject _levelLocked;
        [SerializeField] private GameObject _currentLevel;
        public LevelStage LevelStage { get; set; }
        public int SceneNumber { get; set; }
        public bool isCurrentScene { get; set;} = false;
        public bool isLevelLocked { get; set; }
        public void UpdateInfo(Sprite MainSprite)
        {
            _mainImage.sprite = MainSprite;
            _levelLocked.SetActive(true);
            _firstStar.SetActive(false);
            _secondStar.SetActive(false);
            _thirdStar.SetActive(false);
            _levelNumber.SetActive(false);
            _currentLevel.SetActive(false);
            isLevelLocked = true;
        }
        public void SelectCurrentLevel()
        {
            if (isLevelLocked || SceneNumber == SaveManager.GetData().CurrentSceneNumber)
                return;
            SaveManager.SaveSceneNumber(SceneNumber);
            _mainImage.sprite = gameState.InterfaceConfig.CurrentLevel;
            _levelNumber.transform.GetComponent<Text>().text = SceneNumber.ToString();
            _firstStar.SetActive(false);
            _secondStar.SetActive(false);
            _thirdStar.SetActive(false);
            _levelLocked.SetActive(false);
            _currentLevel.SetActive(true);
            FindOldScene();
            isCurrentScene = true;
        }
        void FindOldScene()
        {
            foreach (Transform Scene in _scenesHolder)
            {
                var SceneInfo = Scene.GetComponent<LevelMB>();
                if (SceneInfo.isCurrentScene)
                {
                    SceneInfo.isCurrentScene = false;
                    SceneInfo.UpdateLevel(SceneInfo.LevelStage, SceneInfo.SceneNumber, gameState, _scenesHolder);
                }
            }
        }
        public void UpdateLevel(LevelStage levelStage, int levelNumber, GameState state, Transform scenesHolder)
        {
            isLevelLocked = false;
            _scenesHolder = scenesHolder;
            gameState = state;
            SceneNumber = levelNumber;
            LevelStage = levelStage;
            _levelNumber.transform.GetComponent<Text>().text = levelNumber.ToString();
            _mainImage.sprite = state.InterfaceConfig.CompleteLevel;
            _levelLocked.SetActive(false);
            _currentLevel.SetActive(false);
            switch (levelStage)
            {
                case LevelStage.zeroLevel:
                    _firstStar.SetActive(false);
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.firstLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.secondLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.thirdLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.fourthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.fifthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(true);
                    _thirdStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    break;
                case LevelStage.sixthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(true);
                    _thirdStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    break;
                default:
                    break;
            }
        }
        public void LoadLevel(LevelStage levelStage, int levelNumber, GameState state, Transform scenesHolder)
        {
            isLevelLocked = false;
            _scenesHolder = scenesHolder;
            gameState = state;
            SceneNumber = levelNumber;
            LevelStage = levelStage;
            if (levelNumber == SceneManager.GetActiveScene().buildIndex)
            {
                isCurrentScene = true;
                _mainImage.sprite = state.InterfaceConfig.CurrentLevel;
                _levelNumber.transform.GetComponent<Text>().text = levelNumber.ToString();
                _firstStar.SetActive(false);
                _secondStar.SetActive(false);
                _thirdStar.SetActive(false);
                _levelLocked.SetActive(false);
                _currentLevel.SetActive(true);
                return;
            }
            _levelNumber.transform.GetComponent<Text>().text = levelNumber.ToString();
            _mainImage.sprite = state.InterfaceConfig.CompleteLevel;
            _levelLocked.SetActive(false); 
            _currentLevel.SetActive(false);
            switch (levelStage)
            {
                case LevelStage.zeroLevel:
                    _firstStar.SetActive(false);
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.firstLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.secondLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(false);
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.thirdLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.fourthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(false);
                    break;
                case LevelStage.fifthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(true);
                    _thirdStar.GetComponent<Image>().sprite = state.InterfaceConfig.HalfStarLeft;
                    break;
                case LevelStage.sixthLevel:
                    _firstStar.SetActive(true);
                    _firstStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _secondStar.SetActive(true);
                    _secondStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    _thirdStar.SetActive(true);
                    _thirdStar.GetComponent<Image>().sprite = state.InterfaceConfig.FullStar;
                    break;
                default:
                    break;
            }
        }
    }
}