using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class ProductInitSystem : MonoBehaviour, IEcsInitSystem
    {
        public void Init (IEcsSystems systems)
        {
            /*var storePanelMB = FindObjectOfType<StorePanelMB>();
            var scrollPanel = storePanelMB.GetScrollPanel();

            storePanelMB.OpenPanel();*/
        }
    }
}