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
    public Image bossHPFill;
    public float hpAnimSpeed = 3f;

    // =========================
    // ✅ 추가된 보스 사운드 설정
    // =========================
    [Header("Boss Sounds")]
    public AudioSource audioSource;    // 보스 전용 오디오 소스
    public AudioClip bossHurtSound;    // 보스 피격음
    public AudioClip bossDeathSound;   // 보스 사망음

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

        if (bossHPFill != null)
            bossHPFill.fillAmount = 1f;

        if (bossAlive != null)
            bossAlive.SetActive(true);

        if (bossDefeated != null)
            bossDefeated.SetActive(false);

        // 오디오 소스가 설정 안 되어 있으면 자동으로 찾아줌
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    // ===== 데미지 처리 =====
    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        currentHP -= damage;
        if (currentHP < 0)
            currentHP = 0;

        // ✅ 보스 피격 소리 재생
        PlayBossSound(bossHurtSound);

        AnimateHPBar();

        if (currentHP <= maxHP / 2 &&
            AcidAttackManager.Instance != null &&
            !AcidAttackManager.Instance.HasActivated)
        {
            AcidAttackManager.Instance.StartAcidAttack();
        }

        if (currentHP == 0)
            OnBossDefeated();
    }

    // ✅ 보스 전용 소리 재생 함수
    void PlayBossSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // 보스니까 피치를 살짝 낮게(0.8~0.9) 설정하면 더 묵직한 소리가 납니다.
            audioSource.pitch = Random.Range(0.8f, 1.0f);
            audioSource.PlayOneShot(clip);
        }
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

        // ✅ 보스 사망 소리 재생
        PlayBossSound(bossDeathSound);

        if (bossAlive != null)
            bossAlive.SetActive(false);

        if (bossDefeated != null)
            bossDefeated.SetActive(true);

        GameManager.Instance.isGameFinished = true;
        GameManager.Instance.SetAllGameUIOff();

        EndingManager.Instance.ShowEnding();
    }
}
