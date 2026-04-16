using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Display")]
    [SerializeField] private SpriteNumberDisplay scoreDisplay;

    [Header("Win")]
    [SerializeField] private int winScore = 9999;
    [SerializeField] private GameObject heartPopup;

    private float score = 0f;
    private int displayedScore = 0;
    private bool hasWon = false;

    public int CurrentScore => displayedScore;
    public bool HasWon => hasWon;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        score = 0f;
        displayedScore = 0;
        hasWon = false;

        if (heartPopup != null)
        {
            heartPopup.SetActive(false);
        }

        UpdateDisplay();
    }

    public void AddScorePerSecond(float amountPerSecond)
    {
        if (hasWon)
        {
            return;
        }

        score += amountPerSecond * Time.deltaTime;
        score = Mathf.Max(0f, score);

        int newScore = Mathf.FloorToInt(score);

        if (newScore != displayedScore)
        {
            displayedScore = newScore;
            UpdateDisplay();
            CheckWin();
        }
    }

    private void UpdateDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.SetNumber(displayedScore);
        }
    }

    private void CheckWin()
    {
        if (hasWon)
        {
            return;
        }

        if (displayedScore >= winScore)
        {
            hasWon = true;
            displayedScore = winScore;
            score = winScore;
            UpdateDisplay();

            if (heartPopup != null)
            {
                heartPopup.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }
}