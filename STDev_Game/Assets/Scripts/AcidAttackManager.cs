using UnityEngine;

public class AcidAttackManager : MonoBehaviour
{
    public static AcidAttackManager Instance;

    [Header("UI")]
    public GameObject acidAttackUI;

    private bool hasActivated = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 보스 HP 50% 이하 최초 진입 시 호출
    /// </summary>
    public void StartAcidAttack()
    {

        GameManager.Instance.isInAcidAttack = true;

        GameManager.Instance.SetAllGameUIOff();
        acidAttackUI.SetActive(true);

    }

    // =========================
    // 화학 선택 처리
    // =========================

    // ✅ 정답: NaCl
    public void SelectNaCl()
    {
        Debug.Log("✅ NaCl 선택 - 산성 공격 회피 성공");
        EndAcidAttack();
    }

    // ❌ 오답: 다른 화학물질
    public void SelectWrongChemical()
    {
        Debug.Log("❌ 잘못된 화학물질 선택 - 플레이어 피해 -20");
        PlayerController.Instance.TakeDamage(20);
        EndAcidAttack();
    }

    // =========================
    // 이벤트 종료 처리
    // =========================

    void EndAcidAttack()
    {

        acidAttackUI.SetActive(false);

        GameManager.Instance.isInAcidAttack = false;

        // ✅ 여기서만 Math 복귀 가능
        GameManager.Instance.EnterMathState();

    }
}

