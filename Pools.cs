using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Pools : ScriptableObject
{
    public int Size { get; set; }
    public string Name { get; set; }
    public GameObject Prefab;
    public Queue<GameObject> Pool;

    private GameObject _parent;

    public Pools (GameObject prefab, int size, string name = null)
    {
        if (size < 1)
        {
            return;
        }

        Pool = new Queue<GameObject>();
        Size = size;
        Name = name ?? null;

        if (Name != null)
        {
            _parent = new GameObject();
            _parent.name = Name;
        }

        if (prefab != null)
        {
            Prefab = prefab;
        }

        CreateObjects();
    }

    public virtual void CreateObjects()
    {
        if (Prefab == null)
        {
            Debug.LogError("The pool did not have a prefab!");
            return;
        }

        for (int i = 0; i < Size; i++)
        {
            var poolObject = GameObject.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            poolObject.SetActive(false);
            Pool.Enqueue(poolObject);

            if (_parent != null)
            {
                poolObject.transform.SetParent(_parent.transform);
            }
        }
    }

    public virtual GameObject GetFromPool()
    {
        var poolObject = Pool.Dequeue();
        Pool.Enqueue(poolObject);
        poolObject.SetActive(true);
        return poolObject;
    }
}
