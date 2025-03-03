using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] TextMeshProUGUI scoreNumberText;

    private void Start()
    {
        GameEvents.OnScoreIncrease += UpdateScoreUI;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        int score = scoreManager.Score;
        scoreNumberText.text = score.ToString();
    }
}
