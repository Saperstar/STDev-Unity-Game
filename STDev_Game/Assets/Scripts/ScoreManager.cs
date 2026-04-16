using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private SpriteNumberDisplay scoreDisplay;

    private float score = 0f;
    private int displayedScore = 0;

    public int CurrentScore => displayedScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateDisplay();
    }

    public void AddScorePerSecond(float amountPerSecond)
    {
        score += amountPerSecond * Time.deltaTime;

        int newScore = Mathf.FloorToInt(score);

        if (newScore != displayedScore)
        {
            displayedScore = newScore;
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (scoreDisplay != null)
        {
            scoreDisplay.SetNumber(displayedScore);
        }
    }
}