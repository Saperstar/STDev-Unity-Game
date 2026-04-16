using UnityEngine;

public class Heart_GameTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float startTimeSeconds = 30f;
    [SerializeField] private SpriteNumberDisplay timerDisplay;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPopup;

    private float remainingTime;
    private bool isFinished = false;

    private void Awake()
    {
        remainingTime = startTimeSeconds;

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(false);
        }

        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (Heart_GameStartController.Instance != null && !Heart_GameStartController.Instance.HasStarted)
        {
            return;
        }

        if (isFinished)
        {
            return;
        }

        if (Time.timeScale <= 0f)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(0f, remainingTime);

        UpdateTimerDisplay();

        if (remainingTime <= 0f)
        {
            isFinished = true;

            if (ScoreManager.Instance == null || !ScoreManager.Instance.HasWon)
            {
                if (gameOverPopup != null)
                {
                    gameOverPopup.SetActive(true);
                }

                Time.timeScale = 0f;
            }
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerDisplay != null)
        {
            int displayValue = Mathf.CeilToInt(remainingTime);
            timerDisplay.SetNumber(displayValue);
        }
    }
}