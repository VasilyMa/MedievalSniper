using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Client 
{
    public class ProductPrefabMB : MonoBehaviour
    {
        public Text textName;
        public Image sprite;
        public Text textCost;
        public Text textPowerStrike;
        public Text textSkillTimeBonus;
        public SkinData SkinData { get; set; }
        public bool isCurrentSkin { get; set; }
        public bool isEquiped { get; set; }
        public bool isOpen { get; set; }
        
        [HideInInspector]
        public bool isBought { get; set; }
        public Text textButton { get; set; }

        private bool isNextProduct1 = true;
        private bool isNextProduct2;

        float width;

        private void Awake()
        {
            width = transform.GetComponent<RectTransform>().rect.width / 3;
        }


        public void FixedUpdate()
        {
            if (!isOpen)
                return;

            float dis = Mathf.Abs(textButton.gameObject.transform.position.x - transform.position.x);

            if (isNextProduct1 && dis < width)
            {
                isNextProduct1 = false;
                isNextProduct2 = true;

                print("Hello");
                if (!isBought)
                {
                    textButton.text = "Buy";
                    isCurrentSkin = true;
                    FindObjectOfType<StorePanelMB>().currentSkinData = SkinData;
                }
                else if (!isEquiped)
                {
                    textButton.text = "Equip";
                    isCurrentSkin = true;
                    FindObjectOfType<StorePanelMB>().currentSkinData = SkinData;
                }
                else if (isEquiped)
                {
                    textButton.text = "Equipped";
                    isCurrentSkin = true;
                    FindObjectOfType<StorePanelMB>().currentSkinData = SkinData;
                }
            }
            else if (isNextProduct2 && dis > width)
            {
                isNextProduct1 = true;
                isNextProduct2 = false;
                isCurrentSkin = false;
            }
        }
    }
}

