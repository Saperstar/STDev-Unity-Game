using UnityEngine;

public enum GameState
{
    Math,            // 수학 문제 상태
    AttackMiniGame,  // 공격 미니게임
    DodgeMiniGame    // 회피 미니게임
}

public class GameManager : MonoBehaviour
{
    public bool isGameFinished = false;
    public static GameManager Instance;

    [Header("Current State")]
    public GameState currentState;

    [Header("UI Objects")]
    public GameObject mathUI;            // 수학 문제 UI
    public GameObject attackMiniGame;    // 공격 미니게임 UI
    public GameObject dodgeMiniGame;     // 회피 미니게임 UI

    // ✅ 산성 공격(특수 이벤트) 중인지 여부
    [HideInInspector]
    public bool isInAcidAttack = false;

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
        EnterMathState();
    }

    // =========================
    // 상태 전환 함수
    // =========================

    public void EnterMathState()
    {

        // ✅ 게임 종료면 절대 Math로 안 감
        if (isGameFinished)
        {
            Debug.Log("❌ 게임 종료 상태 - Math 차단");
            return;
        }

        // ✅ 산성 패턴 중이면 절대 Math로 안 감
        if (isInAcidAttack)
        {
            Debug.Log("❌ 산성 패턴 중 - Math 차단");
            return;
        }

        currentState = GameState.Math;

        mathUI.SetActive(true);
        attackMiniGame.SetActive(false);
        dodgeMiniGame.SetActive(false);

        MathManager.Instance.StartQuestion();

        Debug.Log("✅ 상태 전환: Math");

    }

    public void StartAttackMiniGame()
    {
        // ❗ 산성 공격 중 게임 흐름 차단
        if (isInAcidAttack) return;

        currentState = GameState.AttackMiniGame;

        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(true);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);

        Debug.Log("상태 전환: AttackMiniGame");
    }

    public void StartDodgeMiniGame()
    {
        // ❗ 산성 공격 중 게임 흐름 차단
        if (isInAcidAttack) return;

        currentState = GameState.DodgeMiniGame;

        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(true);

        Debug.Log("상태 전환: DodgeMiniGame");
    }

    // =========================
    // 강제 UI 제어 (특수 이벤트용)
    // =========================

    /// <summary>
    /// 산성 공격 같은 특수 이벤트 진입 시
    /// 모든 게임 UI를 강제로 끈다
    /// </summary>
    public void SetAllGameUIOff()
    {
        if (mathUI != null) mathUI.SetActive(false);
        if (attackMiniGame != null) attackMiniGame.SetActive(false);
        if (dodgeMiniGame != null) dodgeMiniGame.SetActive(false);
    }
}

