using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using System.Collections;

namespace Client 
{
    public class ComboSystemMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        public Transform HolderCombo;

        public Slider LeftSlider;
        public Slider RightSlider;

        public Image LeftSliderImage;
        public Image RightSliderImage;

        public Text TopText;
        public Text TimerText;
        public RawImage TimerBarImage;
        public RectTransform RectTransform;

        [SerializeField] private Image _star_01;
        [SerializeField] private Image _star_02;
        [SerializeField] private Image _star_03;

        [SerializeField] private GameObject _particle_01;
        [SerializeField] private GameObject _particle_02;
        [SerializeField] private GameObject _particle_03;


        public Gradient Gradient;

        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
        }
        public void UpdateSliders(float value)
        {
            LeftSlider.value = value; RightSlider.value = value;
            LeftSliderImage.color = Gradient.Evaluate(LeftSlider.normalizedValue);
            RightSliderImage.color = Gradient.Evaluate(RightSlider.normalizedValue);
        }
        public void UpdateRating(LevelStage value)
        {
            switch (value)
            {
                case LevelStage.zeroLevel:
                    break;
                case LevelStage.firstLevel:
                    _star_01.gameObject.SetActive(true);
                    _star_01.sprite = _state.InterfaceConfig.HalfStarLeft;
                    break;
                case LevelStage.secondLevel:
                    _particle_01.SetActive(true);
                    StartCoroutine(WaitForFirstStar());
                    break;
                case LevelStage.thirdLevel:
                    _star_02.gameObject.SetActive(true);
                    _star_02.sprite = _state.InterfaceConfig.HalfStarLeft;
                    break;
                case LevelStage.fourthLevel:
                    _particle_02.SetActive(true);
                    StartCoroutine(WaitForSecondStar());
                    break;
                case LevelStage.fifthLevel:
                    _star_03.gameObject.SetActive(true);
                    _star_03.sprite = _state.InterfaceConfig.HalfStarLeft;
                    break;
                case LevelStage.sixthLevel:
                    _particle_03.SetActive(true);
                    StartCoroutine(WaitForThirdStar());
                    break;
                default:
                    break;
            }
        }
        private IEnumerator WaitForFirstStar()
        {
            yield return new WaitForSeconds(0.1f);
            _star_01.sprite = _state.InterfaceConfig.FullStar;
        }
        private IEnumerator WaitForSecondStar()
        {
            yield return new WaitForSeconds(0.1f);
            _star_02.sprite = _state.InterfaceConfig.FullStar;
        }
        private IEnumerator WaitForThirdStar()
        {
            yield return new WaitForSeconds(0.1f);
            _star_03.sprite = _state.InterfaceConfig.FullStar;
        }
        public void ResetComboSystem()
        {
            _star_01.sprite = _state.InterfaceConfig.EmptyStar;
            _star_02.sprite = _state.InterfaceConfig.EmptyStar;
            _star_03.sprite = _state.InterfaceConfig.EmptyStar;
            _particle_01.gameObject.SetActive(false);
            _particle_02.gameObject.SetActive(false);
            _particle_03.gameObject.SetActive(false);
        }
    }
}
