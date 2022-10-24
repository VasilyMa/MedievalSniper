using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using DG.Tweening;

namespace Client 
{
    public class ResourcesMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        [SerializeField] private Text _coinAmount;
        [SerializeField] private Transform _holderResources;
        [SerializeField] private Transform _target;
        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            Scores.OnEventCalculateMoney += UpdateCoin;
        }
        public void UpdateCoin()
        {
            _coinAmount.text = SaveManager.GetData().Coins.ToString();
        }
        public void MovePanel()
        {
            _holderResources.transform.DOMove(_target.position, 0.5f, false);
        }
        private void OnDestroy()
        {
            Scores.OnEventCalculateMoney -= UpdateCoin;
        }
    }
}
