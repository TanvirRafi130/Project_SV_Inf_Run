using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{

    [Header("Land Management")]
    [SerializeField] GameObject lastLand;
    [SerializeField] float landMoveSpeed = 5f;
    public Action<GameObject> onLandNeed;
    List<GameObject> landAvailable = new List<GameObject>();
    [Header("Game State")]
    [SerializeField] GameState currentState = GameState.Playing;
    public GameState GameState => currentState;






    private static GameManager _instance;
    public static GameManager Instance => _instance;


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

    public void SetGameState(GameState newState)
    {
        currentState = newState;
    }

    public void AddLandToMove(GameObject land)
    {
        landAvailable.Add(land);
    }
    void RemoveLandFromMove(GameObject land)
    {
        landAvailable.Remove(land);
        PoolManager.Instance.ReturnLandToPool(land);
    }

    private void OnEnable()
    {
        onLandNeed += SpawnLand;
    }
    void OnDisable()
    {
        onLandNeed -= SpawnLand;
    }

    public void SpawnLand(GameObject calledfrom)
    {
        RemoveLandFromMove(calledfrom);
        Vector3 size = lastLand.GetComponent<Land>().groundMeshRenderer.bounds.size;
        Vector3 spawnPos = lastLand.transform.position + Vector3.Scale(lastLand.transform.forward, size);
        spawnPos.z -= 1f; // slight overlap to avoid gaps
        lastLand = PoolManager.Instance.GetPooledLand();
        landAvailable.Add(lastLand);
        lastLand.transform.position = spawnPos;
        lastLand.transform.rotation = lastLand.transform.rotation;
        //Debug.Break();

    }

    void FixedUpdate()
    {
        if (Player.Instance.IsGameStarted && currentState == GameState.Playing)
        {
            MoveLands();
        }
    }

    void MoveLands()
    {
        // Move each land piece backward along Z axis using Rigidbody
        var avail = landAvailable;
        for (int i = 0; i < avail.Count; i++)
        {
            GameObject land = avail[i];
            if (land == null)
            {
                avail.RemoveAt(i);
                continue;
            }
            Rigidbody rb = land.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.back * landMoveSpeed;
            }

        }
    }

}
public enum GameState
{
    Halt = 0,
    Playing = 1,
    GameOver = 2
}
