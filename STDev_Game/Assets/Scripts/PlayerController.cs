using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    // =========================
    // HP 설정
    // =========================
    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    // =========================
    // 캐릭터 비주얼
    // =========================
    [Header("Visuals")]
    public GameObject playerAlive;   // 평상시 모습
    public GameObject playerDead;    // 쓰러진 모습

    // =========================
    // UI
    // =========================
    [Header("UI")]
    public Image hpFillImage;        // PlayerHPUI / HP_Fill

    // =========================
    // HP Bar Animation
    // =========================
    [Header("HP Animation")]
    public float hpAnimDuration = 0.3f;
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
        // 체력 초기화
        currentHP = maxHP;

        // 비주얼 초기 상태
        if (playerAlive != null) playerAlive.SetActive(true);
        if (playerDead != null) playerDead.SetActive(false);

        UpdateHPUI();
    }

    // =========================
    // 데미지 처리
    // =========================
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            UpdateHPUI();
            OnPlayerDefeated();
            return;
        }

        UpdateHPUI();
    }

    // =========================
    // HP UI 애니메이션
    // =========================
    void UpdateHPUI()
    {
        if (hpFillImage == null)
            return;

        float targetFill = (float)currentHP / maxHP;

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

        hpFillImage.fillAmount = targetFill;
    }

    // =========================
    // 플레이어 사망 처리
    // =========================
    void OnPlayerDefeated()
    {
        Debug.Log("💀 플레이어 쓰러짐");

        if (playerAlive != null) playerAlive.SetActive(false);
        if (playerDead != null) playerDead.SetActive(true);

        // ✅ 게임 종료 처리
        GameManager.Instance.GameOver();
    }
}

