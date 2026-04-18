using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

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
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        if (currentHP < 0) currentHP = 0;

        UpdateHPUI();

        if (currentHP <= 0)
        {
            OnPlayerDead();
        }
    }

    void UpdateHPUI()
    {
        if (hpFillImage == null) return;
        hpFillImage.fillAmount = (float)currentHP / maxHP;
    }

    void OnPlayerDead()
    {
        Debug.Log("💀 플레이어 사망!");
        // TODO: 게임 오버 처리
    }
}

