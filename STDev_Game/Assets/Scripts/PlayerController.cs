using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    [Header("UI")]
    public Image hpFillImage;   // PlayerHPUI의 HP_Fill

    // =========================
    // HP Bar Animation (방식 1)
    // =========================
    [Header("HP Animation")]
    public float hpAnimDuration = 0.3f;   // 체력바 줄어드는 시간
    private Coroutine hpAnimCoroutine;

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
        // 플레이어 체력 초기화
        currentHP = maxHP;

        // HP UI 초기 반영
        UpdateHPUI();
    }

    // =========================
    // 데미지 처리
    // =========================

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        // 실제 체력 즉시 감소
        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        // HP UI는 애니메이션으로 감소
        UpdateHPUI();

        // 플레이어 사망 판정 (필요하면 여기서 처리)
        if (currentHP <= 0)
        {
            OnPlayerDefeated();
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
    // 플레이어 사망 처리
    // =========================

    void OnPlayerDefeated()
    {
        Debug.Log("💀 플레이어 사망");

        // 나중에 여기서 게임오버 처리
        // 예: GameOverUI 띄우기, 입력 차단 등
    }
}

