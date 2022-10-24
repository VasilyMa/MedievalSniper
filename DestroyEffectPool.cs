using UnityEngine;

[CreateAssetMenu(fileName = "DestroyEffectPool", menuName = "Pools/DestroyEffectPool")]
[System.Serializable]
public class DestroyEffectPool : Pools
{
    public DestroyEffectPool(GameObject prefab, int size, string name = null) : base(prefab, size, name)
    {

    }
}
