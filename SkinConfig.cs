using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewSkin", menuName = "Data/NewSkin")]
public class SkinConfig : ScriptableObject
{
    public SkinData skinData = new SkinData();
}
