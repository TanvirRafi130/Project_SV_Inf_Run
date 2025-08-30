using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] Transform gemCanvasPosition;
    [SerializeField] RectTransform canvasRectTransform;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gemText;

    public Action<int> onScoreUpdate;
    public Action<int> onGemUpdate;

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

        onScoreUpdate += ScoreUpdate;
        onGemUpdate += GemUpdate;

    }

    private void OnDestroy()
    {
        onScoreUpdate -= ScoreUpdate;
        onGemUpdate -= GemUpdate;
    }
    void ScoreUpdate(int score)
    {
        scoreText.text = score.ToString();
    }
    void GemUpdate(int gemCount)
    {
        gemText.text = gemCount.ToString();
    }
}
