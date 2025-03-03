using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private int startingScore;
    [SerializeField] private int scoreIncreaseAmount = 1;
    [SerializeField] private int comboNumber;
    [SerializeField] private int comboThresholdNumber = 3;
    [SerializeField] private ComboUI comboUI;
    public int ComboNumber => comboNumber;
    private int _score;
    public int Score => _score;

    private void Start()
    {
        GameEvents.OnScoreIncrease += IncreaseScore;
        GameEvents.OnComboIncrease += IncreaseCombo;
        _score = startingScore;
    }

    private void IncreaseScore()
    {
        _score += scoreIncreaseAmount;
    }

    private void IncreaseCombo()
    {
        comboNumber++;
        comboUI.DefaultSlider();
        if ((comboNumber - 2) % comboThresholdNumber == 0)
        {
            scoreIncreaseAmount++;
        }
    }


    public void DefaultCombo()
    {
        comboNumber = 0;
        scoreIncreaseAmount = 1;
    }
    
}
