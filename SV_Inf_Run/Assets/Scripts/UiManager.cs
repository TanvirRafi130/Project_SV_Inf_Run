using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{

    [Header("Panels")]
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject gameOverPanel;
    [Header("Buttons")]
    [SerializeField] Button pauseButton;
    [SerializeField] Button resumeButtonPausePanel;
    [SerializeField] Button restartButtonPausePanel;
    [SerializeField] Button restartButtonGameOverPanel;

    [Header("UI Elements")]
    [SerializeField] Transform gemCanvasPosition;
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gemText;

    [Header("Final Score Animation")]
    [SerializeField] Vector3 finalPosition;
    [SerializeField] float finalFontSize;
    [SerializeField] float time;

    public Action<int> onScoreUpdate;
    public Action<int> onGemUpdate;
    public Action sceneRestart;
    public Action onPausePanelOpen;
    public Action<bool> onGameOverPanelOpen;

    Vector3 gemUiWorldPosition;
    public Vector3 GemCanvasPosition => gemUiWorldPosition;


    private static UiManager _instance;
    public static UiManager Instance => _instance;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRectTransform,
            gemCanvasPosition.position,
            Camera.main,
            out worldPos);

        gemUiWorldPosition = worldPos;
        ButtonSetup();
        onScoreUpdate += ScoreUpdate;
        onGemUpdate += GemUpdate;
        sceneRestart += RestartScene;
        onPausePanelOpen += OpenPausePanel;
        onGameOverPanelOpen += OpenEndPanel;
        pausePanel.GetComponent<CanvasGroup>().alpha = 0;
        pausePanel.gameObject.SetActive(false);
        gameOverPanel.GetComponent<CanvasGroup>().alpha = 0;
        gameOverPanel.gameObject.SetActive(false);


    }

    void ButtonSetup()
    {
        pauseButton.onClick.AddListener(() =>
        {
            onPausePanelOpen?.Invoke();
        });
        resumeButtonPausePanel.onClick.AddListener(() =>
        {
            pausePanel.GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
            {
                pausePanel.SetActive(false);
                GameManager.Instance.onGameStateChange?.Invoke(GameState.Playing);

            });
        });
        restartButtonPausePanel.onClick.AddListener(() =>
        {
            sceneRestart?.Invoke();
        });
        restartButtonGameOverPanel.onClick.AddListener(() =>
        {
            sceneRestart?.Invoke();
        });
    }
    void OpenPausePanel()
    {
        GameManager.Instance.onGameStateChange?.Invoke(GameState.Halt);
        pausePanel.SetActive(true);
        pausePanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);

    }
    void OpenEndPanel(bool isNewHighScore)
    {
        //GameManager.Instance.onGameStateChange?.Invoke(GameState.GameOver);
        pauseButton.interactable = false;
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<CanvasGroup>().DOFade(1, 0.5f);
        scoreText.transform.DOLocalMove(finalPosition, time)
        .OnStart(() =>
        {
            DOTween.To(() => scoreText.fontSize, x => scoreText.fontSize = x, finalFontSize, time);
        })
        .OnComplete(() =>
        {
            if (isNewHighScore)
            {
                scoreText.DOColor(Color.green, 0.5f);
                DOTween.To(() => scoreText.fontSize, x => scoreText.fontSize = x, 100f, time);
                // scoreText.text = "New High Score!\n" + scoreText.text;
                StartCoroutine(TypeHighScoreMessage("New High Score!", scoreText.text));
            }
            else
            {
                scoreText.DOColor(Color.red, 0.5f);
            }
        })
        .SetEase(Ease.OutBounce)
        ;

    }

    private void OnDestroy()
    {
        onScoreUpdate -= ScoreUpdate;
        onGemUpdate -= GemUpdate;
        sceneRestart -= RestartScene;
        onPausePanelOpen -= OpenPausePanel;
        onGameOverPanelOpen -= OpenEndPanel;
    }
    void ScoreUpdate(int score)
    {
        scoreText.text = score.ToString();
    }
    void GemUpdate(int gemCount)
    {
        gemText.text = gemCount.ToString();
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator TypeHighScoreMessage(string message, string originalScore)
    {
        scoreText.text = "";
        foreach (char c in message)
        {
            scoreText.text += c;
            yield return new WaitForSeconds(0.05f); // Adjust speed as needed
        }
        scoreText.text += "\n" + originalScore;
    }
}
