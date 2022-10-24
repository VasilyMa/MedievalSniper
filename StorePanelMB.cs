using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using DG.Tweening;

namespace Client
{
    public class StorePanelMB : MonoBehaviour
    {
        private EcsWorld _world;
        private GameState _state;
        private EcsPool<InterfaceComponent> _interfacePool = default;
        private EcsPool<ProductComponent> _productComponent = default;
        private EcsPool<SniperComponent> _sniperPool = default;
        private EcsPool<SoundSystemComponent> _soundSystemPool = default;

        public SkinData currentSkinData;


        [SerializeField] private Transform ScrollPanel;
        [SerializeField] private Text ButtonBuy;

        public void Init(EcsWorld world, GameState state)
        {
            _world = world;
            _state = state;
            _interfacePool = _world.GetPool<InterfaceComponent>();
            _productComponent = _world.GetPool<ProductComponent>();
            _sniperPool = _world.GetPool<SniperComponent>();
            _soundSystemPool = _world.GetPool<SoundSystemComponent>();
        }

        public void OpenPanel()
        {
            ref var interfaceComp = ref _interfacePool.Get(_state.InterfaceEntity);
            interfaceComp.StorePanelMB.transform.GetChild(0).gameObject.SetActive(true);
            ScrollPanel.transform.GetComponent<RectTransform>().localPosition = new Vector2(float.MaxValue, ScrollPanel.transform.GetComponent<RectTransform>().localPosition.y);
            DestroyAllChild();
            FillComponents();
        }
        private void FillComponents()
        {
            foreach (var skin in SaveManager.GetData().Skins) // тест :  _state.ProductConfig.SkinConfigs || SaveManager.GetData().Skins
            {
                var productEntity = _world.NewEntity();
                ref var productComponent = ref _productComponent.Add(productEntity);

                GameObject prefabProduct = GameObject.Instantiate(_state.InterfaceConfig.prefabProduct, ScrollPanel);
                var textsProduct = prefabProduct.GetComponent<ProductPrefabMB>();

                productComponent.Name = skin.Name;
                productComponent.ID = skin.ID;
                productComponent.Sprite = skin.Sprite;
                productComponent.Cost = skin.Cost;
                productComponent.RotateSpeedBouns = skin.RotateSpeedBonus;
                productComponent.SkillTimeBonus = skin.SkillTimeBonus;
                productComponent.isBought = skin.isBought;

                textsProduct.textName.text = skin.Name;
                textsProduct.sprite.sprite = skin.Sprite;
                textsProduct.textCost.text = skin.Cost.ToString();
                textsProduct.textPowerStrike.text = skin.RotateSpeedBonus.ToString();
                textsProduct.textSkillTimeBonus.text = skin.SkillTimeBonus.ToString();
                textsProduct.isBought = skin.isBought;
                textsProduct.textButton = ButtonBuy;
                textsProduct.isEquiped = skin.isEquip;
                CheckItem(ref textsProduct);
                textsProduct.SkinData = skin;
                textsProduct.isOpen = true;
            }
        }
        void CheckItem(ref ProductPrefabMB textsProduct)
        {
            if (textsProduct.isBought)
            {
                if (textsProduct.isEquiped)
                    textsProduct.textButton.text = "Equipped";
                else
                    textsProduct.textButton.text = "Equip";
            }
            else
                textsProduct.textButton.text = "Buy";
        }

        public void ClosePanel()
        {
            this.transform.GetChild(0).transform.DOScale(0, 0.5f);
            for (int i = 0; i < ScrollPanel.childCount; i++)
            {
                var prefab = ScrollPanel.GetChild(i);
                prefab.GetComponent<ProductPrefabMB>().isOpen = false;
                DestroyAllChild();
            }
        }
        public void BuyAndEquip()
        {
            if (!currentSkinData.isBought)
            {
                if (SaveManager.GetData().Coins > currentSkinData.Cost)
                {
                    SaveManager.SaveNewSkin(currentSkinData.Cost, currentSkinData.Name);
                    _interfacePool.Get(_state.InterfaceEntity).ResourcesMB.UpdateCoin();

                    _soundSystemPool.Get(_state.SoundSystemEntity).UIAudioSource.PlayOneShot(_state.AudioPack.AudioStorage.UIAudio.StoreBuying);

                    foreach (Transform item in ScrollPanel)
                    {
                        if (item.GetComponent<ProductPrefabMB>().isCurrentSkin)
                        {
                            item.GetComponent<ProductPrefabMB>().isBought = true;
                            item.GetComponent<ProductPrefabMB>().textButton.text = "Equip";
                            
                            break;
                        }
                    }
                    return;
                }
                return;
            }
            else if (currentSkinData.isBought)
            {
                foreach (Transform item in ScrollPanel)
                {
                    item.GetComponent<ProductPrefabMB>().SkinData.isEquip = false;
                    item.GetComponent<ProductPrefabMB>().isEquiped = false;

                    _soundSystemPool.Get(_state.SoundSystemEntity).UIAudioSource.PlayOneShot(_state.AudioPack.AudioStorage.UIAudio.StoreEquiping);

                    if (item.GetComponent<ProductPrefabMB>().isCurrentSkin)
                    {
                        item.GetComponent<ProductPrefabMB>().isEquiped = true;
                        item.GetComponent<ProductPrefabMB>().SkinData.isEquip = true;
                        item.GetComponent<ProductPrefabMB>().textButton.text = "Equipped";
                        foreach (var skin in
                        SaveManager.GetData().Skins)
                        {
                            if (skin.Name == item.GetComponent<ProductPrefabMB>().SkinData.Name)
                            {
                                skin.isEquip = true;
                                break;
                            }
                        }

                    }
                }
            }
            _state.SkinData = currentSkinData;
            ref var sniperComp = ref _sniperPool.Get(_state.SniperEntity);
            sniperComp.SkinnedMeshRenderer.sharedMesh = _state.SkinData.Mesh;
        }

        private void DestroyAllChild()
        {
            foreach (Transform child in ScrollPanel) Destroy(child.gameObject);
        }

        public Transform GetScrollPanel()
        {
            return ScrollPanel;
        }
    }
}


