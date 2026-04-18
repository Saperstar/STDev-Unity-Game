using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossController : MonoBehaviour
{
    public static BossController Instance;

    [Header("Boss Visuals")]
    public GameObject bossAlive;
    public GameObject bossDefeated;

    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Image hpFillImage;

    // =========================
    // HP Bar Animation (방식 1)
    // =========================
    [Header("HP Animation")]
    public float hpAnimDuration = 0.3f;   // 체력바 줄어드는 시간
    private Coroutine hpAnimCoroutine;

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
        // 보스 체력 초기화
        currentHP = maxHP;

        // 보스 비주얼 초기 상태
        if (bossAlive != null) bossAlive.SetActive(true);
        if (bossDefeated != null) bossDefeated.SetActive(false);

        // HP UI 초기 반영
        UpdateHPUI();
    }

    // =========================
    // Damage 처리
    // =========================

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        // 실제 체력은 즉시 감소
        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        // HP UI는 애니메이션으로 갱신
        UpdateHPUI();

        // ✅ 1️⃣ 보스 사망 판정 (최우선)
        if (currentHP <= 0)
        {
            OnBossDefeated();
            return;
        }

        // ✅ 2️⃣ 산성 패턴 트리거 (HP 50% 이하, 최초 1회)
        if (!AcidAttackManager.Instance.HasActivated &&
            currentHP <= maxHP / 2)
        {
            Debug.Log("🧪 산성 패턴 트리거");
            AcidAttackManager.Instance.StartAcidAttack();
        }
    }

    // =========================
    // HP UI 처리 (애니메이션)
    // =========================

    void UpdateHPUI()
    {
        if (hpFillImage == null)
            return;

        float targetFill = (float)currentHP / maxHP;

        // 기존 애니메이션 중단
        if (hpAnimCoroutine != null)
            StopCoroutine(hpAnimCoroutine);

        // 새 애니메이션 시작
        hpAnimCoroutine = StartCoroutine(AnimateHPBar(targetFill));
    }

    IEnumerator AnimateHPBar(float targetFill)
    {
        float startFill = hpFillImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < hpAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hpAnimDuration;

            hpFillImage.fillAmount = Mathf.Lerp(startFill, targetFill, t);
            yield return null;
        }

        // 마지막 값 보정
        hpFillImage.fillAmount = targetFill;
    }

    // =========================
    // 보스 처치 처리
    // =========================

    void OnBossDefeated()
    {
        Debug.Log("🎉 보스 처치!");

        // 보스 비주얼 전환
        if (bossAlive != null) bossAlive.SetActive(false);
        if (bossDefeated != null) bossDefeated.SetActive(true);

        // 게임 종료 처리
        GameManager.Instance.isGameFinished = true;
        GameManager.Instance.SetAllGameUIOff();

        // 엔딩 UI 표시
        EndingManager.Instance.ShowEnding();
    }
}

