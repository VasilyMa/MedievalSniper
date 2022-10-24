using UnityEngine;

[CreateAssetMenu(fileName = "KillEffectPool", menuName = "Pools/KillEffectPool")]
[System.Serializable]
public class KillEffectPool : Pools
{
    public KillEffectPool(GameObject prefab, int size, string name = null) : base(prefab, size, name)
    {

    }
}