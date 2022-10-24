using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ProductConfig", menuName = "Data/ProductConfig")]
public class ProductConfig : ScriptableObject
{
    public SkinConfig[] SkinConfigs;
}
