using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance; // 싱글톤으로 전역 접근 가능하게

    public int coins = 0;
    public int hearts = 3;
    public int shield = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCoin(int amount)
    {
        coins += amount;
    }

    public void AddHeart(int amount)
    {
        hearts += amount;
    }
    public void AddShield(int amount)
    {
        shield += amount;
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            return true;
        }
        return false;
    }
}
