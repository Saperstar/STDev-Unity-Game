using UnityEngine;
using UnityEngine.UI;

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
        currentHP = maxHP;
        UpdateHPUI();

        if (bossAlive != null) bossAlive.SetActive(true);
        if (bossDefeated != null) bossDefeated.SetActive(false);
    }

    public void TakeDamage(int damage)
    {

        if (currentHP <= 0)
            return;

        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        UpdateHPUI();

        // ✅ 1️⃣ 보스 사망 판정이 최우선
        if (currentHP <= 0)
        {
            OnBossDefeated();
            return;
        }

        // ✅ 2️⃣ 산성 패턴 발동 (HP 50% 이하 최초 1회)
        if (!AcidAttackManager.Instance.HasActivated &&
            currentHP <= maxHP / 2)
        {
            Debug.Log("🧪 산성 패턴 트리거!");
            AcidAttackManager.Instance.StartAcidAttack();
            return;
        }

    }

    void UpdateHPUI()
    {
        if (hpFillImage == null) return;
        hpFillImage.fillAmount = (float)currentHP / maxHP;
    }

    void OnBossDefeated()
    {
        Debug.Log("🎉 보스 처치!");

        if (bossAlive != null) bossAlive.SetActive(false);
        if (bossDefeated != null) bossDefeated.SetActive(true);

        // 게임 종료 처리
        GameManager.Instance.isGameFinished = true;
        GameManager.Instance.SetAllGameUIOff();
        EndingManager.Instance.ShowEnding();
    }
}

