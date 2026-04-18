using UnityEngine;

public class AcidAttackManager : MonoBehaviour
{
    public static AcidAttackManager Instance;

    [Header("UI")]
    public GameObject acidAttackUI;   // Canvas 안의 AcidAttackUI

    // ✅ 산성 패턴이 이미 한 번 발동됐는지
    private bool hasActivated = false;

    // ✅ 외부(BossController)에서 읽기 전용으로 사용
    public bool HasActivated => hasActivated;

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

    /// <summary>
    /// 보스 HP 50% 이하 최초 진입 시 호출
    /// </summary>
    public void StartAcidAttack()
    {
        // 이미 발동했으면 다시 안 함
        if (hasActivated)
            return;

        hasActivated = true;

        Debug.Log("🧪 산성 공격 패턴 시작");

        // ✅ 산성 패턴 상태로 전환
        GameManager_wand.Instance.isInAcidAttack = true;

        // ✅ 기존 게임 UI 전부 끄기
        GameManager_wand.Instance.SetAllGameUIOff();

        // ✅ 산성 패턴 UI 켜기
        if (acidAttackUI != null)
            acidAttackUI.SetActive(true);
    }

    // =========================
    // 버튼 선택 처리
    // =========================

    /// <summary>
    /// 정답 선택 (NaCl)
    /// </summary>
    public void SelectNaCl()
    {
        Debug.Log("✅ NaCl 선택 - 중화 성공");
        EndAcidAttack();
    }

    /// <summary>
    /// 오답 선택 (NaOH, H2SO4 등)
    /// </summary>
    public void SelectWrongChemical()
    {
        Debug.Log("❌ 잘못된 화학물질 선택 - 피해 발생");

        if (PlayerController_wand.Instance != null)
            PlayerController_wand.Instance.TakeDamage(20);

        EndAcidAttack();
    }

    // =========================
    // 산성 패턴 종료
    // =========================

    void EndAcidAttack()
    {
        Debug.Log("🧪 산성 공격 패턴 종료");

        // ✅ UI 끄기
        if (acidAttackUI != null)
            acidAttackUI.SetActive(false);

        // ✅ 산성 패턴 상태 해제
        GameManager_wand.Instance.isInAcidAttack = false;

        // ✅ 다시 수학 문제로 복귀
        GameManager_wand.Instance.EnterMathState();
    }
}

