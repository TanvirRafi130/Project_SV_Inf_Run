using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    [Header("SO Data")]
    [SerializeField] LandDataSo landDataSo;
    [SerializeField] GemDataSo gemDataSo;

    // Pooling system for objects
    // private Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();  // unnecessary complexity for now
    List<GameObject> lands = new List<GameObject>();
    Queue<GameObject> gems = new Queue<GameObject>();
    // Singleton instance
    private static PoolManager _instance;
    public static PoolManager Instance => _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        CreateLandObject();
        CreateGemObject();
    }


    void CreateLandObject()
    {
        foreach (var land in landDataSo.landDataArray)
        {
            for (int i = 0; i < land.maxSpawnCount; i++)
            {
                GameObject obj = Instantiate(land.landPrefab);
                obj.SetActive(false);
                lands.Add(obj);
            }
        }
    }

    public GameObject GetPooledLand()
    {
        if (lands.Count == 0)
        {
            Debug.LogError("No land available in pool!");
            return null;
        }
        var randomIndex = Random.Range(0, lands.Count);
        var land = lands[randomIndex];
        land.SetActive(true);
        lands.RemoveAt(randomIndex);
        return land;
    }

    public void ReturnLandToPool(GameObject land)
    {
        land.SetActive(false);
        lands.Add(land);
    }
    void CreateGemObject()
    {
        foreach (var gem in gemDataSo.gemEntries)
        {
            for (int i = 0; i < gem.spawnAmountInPool; i++)
            {
                GameObject obj = Instantiate(gem.gemPrefab);
                obj.SetActive(false);
                gems.Enqueue(obj);
            }
        }
    }

    public GameObject GetPooledGem()
    {
        if (gems.Count == 0)
        {
            Debug.LogError("No gem available in pool!");
            return null;
        }
        var gem = gems.Dequeue();
        gem.SetActive(true);
        return gem;
    }

    public void ReturnGemToPool(GameObject gem)
    {
        if (gem == null) return;
        gem.SetActive(false);
        gem.transform.localScale = Vector3.one;
        gems.Enqueue(gem);
    }

}
