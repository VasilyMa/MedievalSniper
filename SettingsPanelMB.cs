using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using DG.Tweening;
using UnityEngine.UI;

namespace Client
{
    public class SettingsPanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        [SerializeField] private Transform _settingHolder;
        [SerializeField] private Transform _soundButton;
        [SerializeField] private Transform _vibroButton;
        [SerializeField] private Transform _target;
        private Vector3 defaultPos;
        private bool isOpen;

        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            defaultPos = _settingHolder.position;
            isOpen = false;
            if (SaveManager.GetData().isSound)
            {
                _soundButton.GetComponent<Image>().sprite = _state.InterfaceConfig.SoundOn;
                AudioListener.volume = 1;
                _state.AudioPack.AudioMixer.audioMixer.SetFloat("MasterVolume", 1);
            }
            else
            {
                _soundButton.GetComponent<Image>().sprite = _state.InterfaceConfig.SoundOff;
                AudioListener.volume = 0;
                _state.AudioPack.AudioMixer.audioMixer.SetFloat("MasterVolume", 0);
            }
            if (SaveManager.GetData().isVibro)
            {
                _vibroButton.GetComponent<Image>().sprite = _state.InterfaceConfig.VibroOn;
                _state.EnabledVibration = true;
            }
            else
            {
                _vibroButton.GetComponent<Image>().sprite = _state.InterfaceConfig.VibroOff;
                _state.EnabledVibration = false;
            }
        }
        public void OpenPanel()
        {
            if(isOpen)
                _settingHolder.DOMove(defaultPos, 0.5f, false).SetEase(Ease.OutCirc);
            else
                _settingHolder.DOMove(_target.position, 0.5f, false).SetEase(Ease.OutCirc);
            isOpen = !isOpen;
        }
        public void ToggleSound()
        {
            SaveManager.GetData().isSound = !SaveManager.GetData().isSound;
            SaveManager.SaveSoundSettings(SaveManager.GetData().isSound);
            if (SaveManager.GetData().isSound)
            {
                _soundButton.GetComponent<Image>().sprite = _state.InterfaceConfig.SoundOn;
                AudioListener.volume = 1;
                _state.AudioPack.AudioMixer.audioMixer.SetFloat("MasterVolume", 1);
            }
            else
            {
                _soundButton.GetComponent<Image>().sprite = _state.InterfaceConfig.SoundOff;
                AudioListener.volume = 0;
                _state.AudioPack.AudioMixer.audioMixer.SetFloat("MasterVolume", 0);
            }
        }
        public void ToggleVibro()
        {
            SaveManager.GetData().isVibro = !SaveManager.GetData().isVibro;
            SaveManager.SaveVibroSettings(SaveManager.GetData().isVibro);
            if (SaveManager.GetData().isVibro)
            {
                _vibroButton.GetComponent<Image>().sprite = _state.InterfaceConfig.VibroOn;
                _state.EnabledVibration = true;
            }
            else
            {
                _vibroButton.GetComponent<Image>().sprite = _state.InterfaceConfig.VibroOff;
                _state.EnabledVibration = false;
            }
        }
        public void ClosePanel()
        {
            _settingHolder.gameObject.SetActive(false);
        }
    }
}
