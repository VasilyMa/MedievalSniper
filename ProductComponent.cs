using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    struct ProductComponent 
    {
        public string Name;
        public int ID;
        public Sprite Sprite;
        public int Cost;
        public float SkillTimeBonus;
        public float RotateSpeedBouns;
        public GameObject prefab;
        public bool isBought;
    }
}