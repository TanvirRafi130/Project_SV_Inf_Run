using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    [Header("SO Data")]
    [SerializeField] LandDataSo landDataSo;

    // Pooling system for objects
    // private Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();  // unnecessary complexity for now
    List<GameObject> lands = new List<GameObject>();
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
    }

    private void Start()
    {
        CreateLandObject();
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




}
