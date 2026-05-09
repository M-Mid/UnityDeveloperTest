using System.Collections; // NEW: Required for Coroutines!
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Connections")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    [Header("Game Settings")]
    public float timeRemaining = 120f;
    public int scoreToWin = 5;

    private int currentScore = 0;
    private bool isGameOver = false;

    void Start()
    {
        // CRUCIAL: If we froze time in the last game, we must unfreeze it when the level restarts!
        Time.timeScale = 1f;

        UpdateScoreDisplay();
    }

    void Update()
    {
        if (!isGameOver)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                TriggerGameOver("Time's Up!");
            }
        }
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        currentScore += points;
        UpdateScoreDisplay();

        if (currentScore >= scoreToWin)
        {
            TriggerWin();
        }
    }

    public void PlayerFell()
    {
        if (!isGameOver)
        {
            TriggerGameOver("You Fell!");
        }
    }

    void TriggerWin()
    {
        isGameOver = true;
        if (timeText != null)
        {
            timeText.color = Color.green;
            timeText.text = "YOU WIN!";
        }

        // 1. FREEZE TIME!
        Time.timeScale = 0f;

        // 2. Start the real-world countdown
        StartCoroutine(RestartAfterDelay());
    }

    void TriggerGameOver(string message)
    {
        isGameOver = true;
        if (timeText != null)
        {
            timeText.color = Color.red;
            timeText.text = message;
        }

        // 1. FREEZE TIME!
        Time.timeScale = 0f;

        // 2. Start the real-world countdown
        StartCoroutine(RestartAfterDelay());
    }

    // --- NEW: The Coroutine to handle real-world waiting ---
    IEnumerator RestartAfterDelay()
    {
        // Because Time.timeScale is 0, normal game time is paused. 
        // We must tell Unity to use Realtime seconds here!
        yield return new WaitForSecondsRealtime(5f);

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
            scoreText.text = "Points: " + currentScore + " / " + scoreToWin;
    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        if (timeText != null)
            timeText.text = "Time: " + timeToDisplay.ToString("F2");
    }
}