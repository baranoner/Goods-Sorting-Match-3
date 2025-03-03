using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
   [SerializeField] private GameObject winUI;
   [SerializeField] private GameObject gameOverUI;
   [SerializeField] private TextMeshProUGUI winScoreText;
   [SerializeField] private ScoreManager scoreManager;
   [SerializeField] private InputManager inputManager;
   [SerializeField] private Timer timer;
   [SerializeField] private ComboUI comboUI;

   public void RetryLevel()
   {
      int currentLevelSceneIndex = SceneManager.GetActiveScene().buildIndex;
      SceneManager.LoadScene(currentLevelSceneIndex);
   }

   public void ExitGame()
   {
      Application.Quit();
   }

   public void NextLevel()
   {
      int currentLevelSceneIndex = SceneManager.GetActiveScene().buildIndex;

      if (currentLevelSceneIndex != SceneManager.sceneCountInBuildSettings - 1)
      {
         SceneManager.LoadScene(currentLevelSceneIndex + 1);
      }
      else
      {
         SceneManager.LoadScene(0);
      }
   }

   public void GameOver()
   {
      gameOverUI.SetActive(true);
      inputManager.enabled = false;
      timer.PauseTimer();
      comboUI.PauseSliderTimer();
   }

   public void Win()
   {
      winUI.SetActive(true);
      inputManager.enabled = false;
      timer.PauseTimer();
      comboUI.PauseSliderTimer();
      winScoreText.text = $"Score: {scoreManager.Score}";


   }
   
}
