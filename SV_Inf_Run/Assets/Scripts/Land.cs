using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Land : MonoBehaviour
{
    public MeshRenderer groundMeshRenderer;
    [SerializeField] bool isActiveAtStart = false;
    [SerializeField] Transform gemParentHolder;
    List<GameObject> gemsOnLand = new List<GameObject>();
    int totalGemsOnLand = 0;
    public Action OnGemsSpawm;

    private void Start()
    {
        if (isActiveAtStart)
        {
            GameManager.Instance.AddLandToMove(this.gameObject);

        }
        totalGemsOnLand = gemParentHolder.childCount;
        OnGemsSpawm += SpawnGems;
    }

    private void OnDestroy()
    {
        OnGemsSpawm -= SpawnGems;
    }
    private void SpawnGems()
    {
        for (int i = 0; i < totalGemsOnLand; i++)
        {
            var gem = PoolManager.Instance.GetPooledGem();
            if (gem == null) continue;
            gem.transform.position = gemParentHolder.GetChild(i).position;
            gem.transform.parent = gemParentHolder;
            gem.GetComponent<Gem>().myLand = this;
            gemsOnLand.Add(gem.gameObject);
        }
        // Debug.LogError("Total gems on land: " + gemsOnLand.Count + " for land: " + this.gameObject.name);
    }
    private void OnDisable()
    {
        for (int i = 0; i < gemsOnLand.Count; i++)
        {
            if(gemsOnLand[i] == null) continue;
            PoolManager.Instance.ReturnGemToPool(gemsOnLand[i]);
        }
        gemsOnLand.Clear();
    }

    public void RemoveGem(GameObject gem)
    {
        gemsOnLand.Remove(gem);
    }

    void OnTriggerExit(Collider other)
    {
        if (GameManager.Instance.GameState != GameState.Playing) return;
        if (other.TryGetComponent<Player>(out Player player))
        {

            GameManager.Instance.onLandNeed?.Invoke(this.gameObject);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {

        if (gemParentHolder.childCount != 0)
        {
            Gizmos.color = Color.yellow;
            foreach (Transform gem in gemParentHolder)
            {
                Gizmos.DrawSphere(gem.position, 0.1f);
            }
        }
    }


#endif
}
