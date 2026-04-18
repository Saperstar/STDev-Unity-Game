using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BossController : MonoBehaviour
{
    public static BossController Instance;

    // ===== Boss Visuals =====
    [Header("Boss Visuals")]
    public GameObject bossAlive;
    public GameObject bossDefeated;

    // ===== HP =====
    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    // ===== HP UI =====
    [Header("Boss HP UI")]
    public Image bossHPFill;          // Canvas/BossHPUI/HP_Fill
    public float hpAnimSpeed = 3f;    // 클수록 빠름

    private Coroutine hpAnimCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        currentHP = maxHP;

        // HP 바 초기화
        if (bossHPFill != null)
            bossHPFill.fillAmount = 1f;

        if (bossAlive != null)
            bossAlive.SetActive(true);

        if (bossDefeated != null)
            bossDefeated.SetActive(false);
    }

    // ===== 데미지 처리 =====
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        AnimateHPBar();

        // 산성 패턴 트리거 (HP 50% 이하, 1회)
        if (currentHP <= maxHP / 2 &&
            AcidAttackManager.Instance != null &&
            !AcidAttackManager.Instance.HasActivated)
        {
            AcidAttackManager.Instance.StartAcidAttack();
        }

        if (currentHP == 0)
            OnBossDefeated();
    }

    // ===== HP Bar 애니메이션 =====
    void AnimateHPBar()
    {
        if (bossHPFill == null)
            return;

        if (hpAnimCoroutine != null)
            StopCoroutine(hpAnimCoroutine);

        float target = (float)currentHP / maxHP;
        hpAnimCoroutine = StartCoroutine(LerpHP(bossHPFill.fillAmount, target));
    }

    IEnumerator LerpHP(float from, float to)
    {
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * hpAnimSpeed;
            bossHPFill.fillAmount = Mathf.Lerp(from, to, t);
            yield return null;
        }
        bossHPFill.fillAmount = to;
    }

    // ===== 보스 사망 =====
    void OnBossDefeated()
    {
        Debug.Log("🎉 보스 처치!");

        if (bossAlive != null)
            bossAlive.SetActive(false);

        if (bossDefeated != null)
            bossDefeated.SetActive(true);

        GameManager.Instance.isGameFinished = true;
        GameManager.Instance.SetAllGameUIOff();

        EndingManager.Instance.ShowEnding();
    }
}
