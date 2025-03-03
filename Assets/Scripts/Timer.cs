using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float remainingTime;
    [SerializeField] private LevelManager levelManager;
    private bool _isPaused;

    private void Update()
    {
        if (remainingTime > 0 && !_isPaused)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime <= 0)
        {
            remainingTime = 0;
            levelManager.GameOver();
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void PauseTimer()
    {
        _isPaused = true;
    }

    
}
