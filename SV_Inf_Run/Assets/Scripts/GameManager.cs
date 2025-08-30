using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.Collections;
using DG.Tweening;
public class GameManager : MonoBehaviour
{

    [System.Serializable]
    public class LevelDifficulty
    {
        public int scoreThreshold;
        public int landMoveSpeed;
        public float scoreIncreaseTimeInterval;
    }

    [Header("Land Management")]
    [SerializeField] GameObject lastLand;
    [SerializeField] float landMoveSpeed = 5f;
    public Action<GameObject> onLandNeed;
    List<GameObject> landAvailable = new List<GameObject>();
    [Header("Game State")]
    [SerializeField] List<LevelDifficulty> levelDifficulties;
    [SerializeField] GameState currentState = GameState.Playing;
    public GameState GameState => currentState;

    public Action<int> onGemCollect;
    public Action<GameState> onGameStateChange;
    public Action tweenPause;
    public Action tweenResume;


    int currentScore = 0;
    int currentGemCount = 0;
    float scoreIncreaseTimer;

    [SerializeField] int currentDifficultyIndex = 0;

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

    private void Start()
    {
        Application.targetFrameRate = 60;
        ApplyDifficulty(levelDifficulties[currentDifficultyIndex]); // Initial difficulty set
        StartCoroutine(DifficultyRoutine());
        StartCoroutine(ScoreIncreaseRoutine());
        onGemCollect += OnGemCollected;
        onGameStateChange += SetGameState;

        currentGemCount = SaveManager.Instance.GetGemCount();
        UiManager.Instance.onGemUpdate?.Invoke(currentGemCount);
    }
    private void OnDestroy()
    {
        onGemCollect -= OnGemCollected;
        onGameStateChange -= SetGameState;
    }

    void OnGemCollected(int scoreAdd)
    {
        currentGemCount++;
        currentScore += scoreAdd;
        UiManager.Instance.onScoreUpdate?.Invoke(currentScore);
        UiManager.Instance.onGemUpdate?.Invoke(currentGemCount);
    }

    IEnumerator DifficultyRoutine()
    {
        while (true)
        {
            if (currentState != GameState.Playing)
            {
                yield return null;
                continue;
            }
            if (currentDifficultyIndex < levelDifficulties.Count - 1)
            {
                var next = levelDifficulties[currentDifficultyIndex + 1];
                if (currentScore >= next.scoreThreshold)
                {
                    currentDifficultyIndex++;
                    ApplyDifficulty(levelDifficulties[currentDifficultyIndex]);
                }
            }
            yield return new WaitForSeconds(0.01f);
        }
    }

    void ApplyDifficulty(LevelDifficulty difficulty)
    {
        DOTween.To(() => landMoveSpeed, x => landMoveSpeed = x, difficulty.landMoveSpeed, 1.5f);
        scoreIncreaseTimer = difficulty.scoreIncreaseTimeInterval;
        // Score increase interval or onno property o ekhane set korte paren
    }

    IEnumerator ScoreIncreaseRoutine()
    {
        while (true)
        {
            if (currentState != GameState.Playing || !Player.Instance.IsGameStarted)
            {
                yield return null;
                continue;
            }
            currentScore++;
            UiManager.Instance.onScoreUpdate?.Invoke(currentScore);
            yield return new WaitForSeconds(scoreIncreaseTimer);
        }
    }

    void SetGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Halt:

                Player.Instance.onGameHalt?.Invoke();
                tweenPause?.Invoke();
                break;
            case GameState.Playing:
                Player.Instance.onGameResume?.Invoke();
                tweenResume?.Invoke();
                break;
            case GameState.GameOver:
                tweenPause?.Invoke();
                bool isHighScore = currentScore > SaveManager.Instance.GetHighScore();
                UiManager.Instance.onGameOverPanelOpen?.Invoke(isHighScore);
                SaveManager.Instance.SetGemCount(currentGemCount);
                if (isHighScore)
                {
                    SaveManager.Instance.SetHighScore(currentScore);
                }
                break;

        }
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
        int rand = UnityEngine.Random.Range(0, 2);
        if (rand > 0)
        {
            lastLand.GetComponent<Land>().OnGemsSpawm?.Invoke();
        }
        landAvailable.Add(lastLand);
        lastLand.transform.position = spawnPos;
        lastLand.transform.rotation = lastLand.transform.rotation;
        //Debug.Break();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UiManager.Instance.sceneRestart?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && currentState != GameState.GameOver)
        {
            UiManager.Instance.onPausePanelOpen?.Invoke();
        }
    }

    void FixedUpdate()
    {
        if (Player.Instance.IsGameStarted && currentState == GameState.Playing)
        {
            MoveLands();
        }
        else
        {
            if (!isLandMoveOff)
            {
                // Stop all lands
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
                        rb.linearVelocity = Vector3.zero;
                    }

                }
                isLandMoveOff = true;
            }
        }
    }

    bool isLandMoveOff = false;

    void MoveLands()
    {
        isLandMoveOff = false;
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
