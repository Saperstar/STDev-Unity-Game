using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

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
    public GameObject gameOverUI;   // Canvas / GameOverUI

    // =========================
    // Unity Life Cycle
    // =========================
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 상태 초기화
        isGameFinished = false;
        isInAcidAttack = false;

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // ✅ 게임 시작은 항상 수학 상태
        EnterMathState();
    }

    // =========================
    // State Transitions
    // =========================

    /// <summary>
    /// 수학 문제 상태로 진입
    /// </summary>
    public void EnterMathState()
    {
        if (isGameFinished || isInAcidAttack)
            return;

        if (mathUI != null) mathUI.SetActive(true);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);

        // ✅ 핵심: 수학 문제 시작
        MathManager.Instance.StartQuestion();
    }

    /// <summary>
    /// 공격 미니게임 시작
    /// </summary>
    public void StartAttackMiniGame()
    {
        if (isGameFinished || isInAcidAttack)
            return;

        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(true);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);
    }

    /// <summary>
    /// 회피 미니게임 시작
    /// </summary>
    public void StartDodgeMiniGame()
    {
        if (isGameFinished || isInAcidAttack)
            return;

        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(true);
    }

    // =========================
    // Game Over (Player Defeat)
    // =========================

    /// <summary>
    /// 플레이어 패배 처리
    /// </summary>
    public void GameOver()
    {
        if (isGameFinished)
            return;

        Debug.Log("💀 GAME OVER");

        isGameFinished = true;

        SetAllGameUIOff();

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    // =========================
    // Retry
    // =========================

    /// <summary>
    /// 처음부터 다시 시작
    /// </summary>
    public void RetryGame()
    {
        Debug.Log("🔁 Retry Game");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // =========================
    // Common UI Control
    // =========================

    public void SetAllGameUIOff()
    {
        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);
    }
}

