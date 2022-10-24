using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using DG.Tweening;

namespace Client 
{
    public class StrengthBarMB : MonoBehaviour
    {
        [SerializeField] private Transform _targetStrength;
        [SerializeField] private Transform _sliderHolder;
        private Vector3 defaultPosStrength;

        [SerializeField] public Slider Slider;
        [SerializeField] private Gradient _gradient;
        [SerializeField] private Image _image;
        private EcsWorld _world;
        private GameState _state; 


        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            defaultPosStrength = _sliderHolder.transform.position;
        }

        public void UpdateSlider(float value)
        {
            Slider.value = value;
            _image.color = _gradient.Evaluate(Slider.normalizedValue);
        }

        public void StartPlay()
        {
            this.transform.GetChild(0).transform.DOMove(_targetStrength.position, 1.5f, false).SetEase(Ease.OutCubic);
        }

        public void HidePanel()
        {
            this.transform.GetChild(0).transform.DOMove(defaultPosStrength, 1.5f, false).SetEase(Ease.OutCubic);
        }
    }
}
