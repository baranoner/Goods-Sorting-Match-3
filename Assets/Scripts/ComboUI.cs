using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [SerializeField] private float sliderStartingValue = 20f;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private Slider comboSlider;
    private bool _isSliderPaused;

    private void Start()
    {
        GameEvents.OnComboIncrease += UpdateComboUI;
        comboSlider.value = 0f;
        UpdateComboUI();
    }

    private void Update()
    {
        if (!_isSliderPaused)
        {
            comboSlider.value -= Time.deltaTime;
        }

        if (comboSlider.value <= 0)
        {
            scoreManager.DefaultCombo();
            UpdateComboUI();
        }
    }

    private void UpdateComboUI()
    {
        comboText.text = $"Combo: {scoreManager.ComboNumber}x";
    }

    public void DefaultSlider()
    {
        comboSlider.value = sliderStartingValue;
    }

    public void PauseSliderTimer()
    {
        _isSliderPaused = true;
    }
    
}
