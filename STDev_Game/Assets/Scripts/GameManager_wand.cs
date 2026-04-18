using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager_wand : MonoBehaviour
{
    public static GameManager_wand Instance;

    // =========================
    // Game State
    // =========================
    [Header("Game State")]
    public bool isGameFinished = false;   // 승리/패배 시 true
    public bool isInAcidAttack = false;   // 산성 패턴 중인가

    // =========================
    // UI Objects
    // =========================
    [Header("UI Objects")]
    public GameObject mathUI;
    public GameObject attackMiniGame;
    public GameObject dodgeMiniGame;

    // =========================
    // Game Over UI
    // =========================
    [Header("Game Over UI")]
    public GameObject gameOverUI;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            // 만약 씬을 이동해도 점수 등을 유지하고 싶다면 아래 주석을 해제하세요.
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 씬이 시작될 때마다 초기화
        isGameFinished = false;
        isInAcidAttack = false;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // 게임 시작 시 수학 문제 화면으로 진입
        EnterMathState();
    }

    // =========================
    // State Transitions
    // =========================

    public void EnterMathState()
    {
        if (isGameFinished || isInAcidAttack) return;

        SetAllGameUIOff();
        if (mathUI != null) mathUI.SetActive(true);

        // ✅ 수정된 MathManager (뒤에 1 없음)를 호출합니다.
        if (MathManager.Instance != null)
        {
            MathManager.Instance.StartQuestion();
        }
        else
        {
            Debug.LogWarning("현재 씬에 MathManager가 없습니다!");
        }
    }

    public void StartAttackMiniGame()
    {
        if (isGameFinished || isInAcidAttack) return;

        SetAllGameUIOff();
        if (attackMiniGame != null) attackMiniGame.SetActive(true);
    }

    public void StartDodgeMiniGame()
    {
        if (isGameFinished || isInAcidAttack) return;

        SetAllGameUIOff();
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(true);
    }

    // =========================
    // Game Control
    // =========================

    public void GameOver()
    {
        if (isGameFinished) return;

        Debug.Log("💀 GAME OVER");
        isGameFinished = true;
        SetAllGameUIOff();

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    public void RetryGame()
    {
        Debug.Log("🔁 Retry Game");
        // 현재 씬을 다시 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetAllGameUIOff()
    {
        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);
    }
}
