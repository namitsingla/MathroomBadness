using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            // Guard against unassigned prefabs
            if (pool.prefab == null)
            {
                Debug.LogError("Pool '" + pool.tag + "' has no prefab assigned!");
                continue;
            }

            Queue<GameObject> objectQueue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectQueue.Enqueue(obj);
            }

            poolDictionary[pool.tag] = objectQueue;
        }
    }

    public GameObject Get(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];

        // If pool is empty, expand it
        if (pool.Count == 0)
        {
            Pool poolConfig = pools.Find(p => p.tag == tag);
            GameObject newObj = Instantiate(poolConfig.prefab, transform);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
            Debug.Log("Pool '" + tag + "' expanded.");
        }

        GameObject obj = pool.Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[tag].Enqueue(obj);
    }

    public BaseEnemy GetEnemy(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Get(tag, position, rotation);
        if (obj == null) return null;

        // re-enable all scripts in case they were disabled
        foreach (MonoBehaviour mb in obj.GetComponentsInChildren<MonoBehaviour>(true))
            mb.enabled = true;

        return obj.GetComponent<BaseEnemy>();
    }

    public void ReturnEnemyToPool(string tag, BaseEnemy enemy)
    {
        enemy.ResetEnemy();
        ReturnToPool(tag, enemy.gameObject);
    }
}