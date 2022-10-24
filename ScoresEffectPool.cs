using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoresEffectPool", menuName = "Pools/ScoresEffectPool")]
[System.Serializable]
public class ScoresEffectPool : Pools
{
    public ScoresEffectPool(GameObject prefab, int size, string name = null) : base(prefab, size, name)
    {

    }
}
