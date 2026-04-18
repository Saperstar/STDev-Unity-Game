using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    [Header("HP Settings")]
    public int maxHP = 100;
    public int currentHP;

    [Header("Visuals")]
    public GameObject playerAlive;
    public GameObject playerDead;

    [Header("UI")]
    public Image hpFillImage;

    [Header("HP Animation")]
    public float hpAnimDuration = 0.3f;
    private Coroutine hpAnimCoroutine;

    // =========================
    // 추가된 사운드 설정
    // =========================
    [Header("Sound Settings")]
    public AudioSource audioSource;    // 소리를 내는 스피커 역할
    public AudioClip hurtSound;        // 데미지 입었을 때 소리
    public AudioClip deathSound;       // 사망 시 소리

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

        if (playerAlive != null) playerAlive.SetActive(true);
        if (playerDead != null) playerDead.SetActive(false);

        // 오디오 소스가 없을 경우를 대비해 자동으로 찾아주는 안전 코드
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        UpdateHPUI();
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0)
            return;

        currentHP -= damage;

        // ✅ 데미지 사운드 재생 (사망하지 않았을 때만 혹은 공통으로)
        PlaySound(hurtSound);

        if (currentHP <= 0)
        {
            currentHP = 0;
            UpdateHPUI();
            OnPlayerDefeated();
            return;
        }

        UpdateHPUI();
    }

    // ✅ 소리 재생용 함수
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            // PlayOneShot은 여러 소리가 겹쳐도 자연스럽게 들립니다.
            audioSource.PlayOneShot(clip);
        }
    }

    void UpdateHPUI()
    {
        if (hpFillImage == null) return;
        float targetFill = (float)currentHP / maxHP;
        if (hpAnimCoroutine != null) StopCoroutine(hpAnimCoroutine);
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

    void OnPlayerDefeated()
    {
        Debug.Log("💀 플레이어 쓰러짐");

        // ✅ 사망 사운드 재생
        PlaySound(deathSound);

        if (playerAlive != null) playerAlive.SetActive(false);
        if (playerDead != null) playerDead.SetActive(true);

        GameManager.Instance.GameOver();
    }
}

