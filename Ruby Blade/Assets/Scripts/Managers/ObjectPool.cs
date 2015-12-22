using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
    public GameObject pooledObject;
    public int pooledAmount = 30;
    public bool willGrow = true;

    private List<GameObject> pooledObjects;
    private MonoBehaviour beh;

    public ObjectPool(MonoBehaviour b)
    {
        beh = b;
    }

    public void Init(GameObject obj)
    {
        pooledObject = obj;
        pooledObjects = new List<GameObject>();
        for(int i = 0; i < pooledAmount; i++)
        {

            GameObject ob = Object.Instantiate(pooledObject) as GameObject;
            ob.SetActive(false);
            pooledObjects.Add(ob);
        }
    }

    public GameObject GetPooledObject()
    {
        for(int i = 0; i < pooledObjects.Count; i++)
        {
            if(!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        if(willGrow)
        {
            GameObject obj = Object.Instantiate(pooledObject) as GameObject;
            pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

    //public void Destroy()
    //{
    //    foreach (var item in pooledObjects)
    //    {
    //        MonoBehaviour.Destroy(item);
    //    }
    //}
}
