using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.EcsLite;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Client 
{
    public class LosePanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;

        [SerializeField] private Transform targetPositionLosePanel;
        [SerializeField] private Text _textResult;
        [SerializeField] private Transform _holderLosePanel;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
        }
        public void StartLoseEvent(int value)
        {
            _holderLosePanel.gameObject.SetActive(true);
            _holderLosePanel.DOMove(targetPositionLosePanel.position, 1f, false);
            for (int i = 0; i < _holderLosePanel.childCount; i++)
            {
                if (_holderLosePanel.GetChild(i).childCount > 0)
                {
                    for (int y = 0; y < _holderLosePanel.GetChild(i).childCount; y++)
                    {
                        if(_holderLosePanel.GetChild(i).GetChild(y).GetComponent<Image>()) _holderLosePanel.GetChild(i).GetChild(y).GetComponent<Image>().DOFade(1, 0.5f);
                        if (_holderLosePanel.GetChild(i).GetChild(y).GetComponent<Text>()) _holderLosePanel.GetChild(i).GetChild(y).GetComponent<Text>().DOFade(1, 0.5f);
                    }
                }
                if (_holderLosePanel.GetChild(i).GetComponent<Image>()) _holderLosePanel.GetChild(i).GetComponent<Image>().DOFade(1, 0.5f);
                if (_holderLosePanel.GetChild(i).GetComponent<Text>()) _holderLosePanel.GetChild(i).GetComponent<Text>().DOFade(1, 0.5f);
            }
            if (value >= 0) _textResult.text = value.ToString();
            else _textResult.text = 0.ToString();
        }
        public void TryAgain()
        {
            SceneManager.LoadScene(SaveManager.GetData().CurrentSceneNumber);
        }
    }
}

