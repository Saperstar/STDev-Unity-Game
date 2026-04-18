using UnityEngine;
using TMPro;

public class Shop1ShieldPurchase : MonoBehaviour
{
    [Header("설정")]
    public int shieldPrice = 15;     // 방패 가격 (15코인)
    public int maxHearts = 3;       // 조건: 하트가 3개여야 함

    [Header("UI 연결")]
    public TextMeshProUGUI alertText; 

    public void BuyShield()
    {
        // 1. 데이터 보관소 확인
        if (PlayerStats.Instance == null) return;

        int currentCoins = PlayerStats.Instance.coins;
        int currentHearts = PlayerStats.Instance.hearts;

        // 2. 하트가 3개인지 체크 (하트가 모자라면 방패 구매 불가)
        if (currentHearts < maxHearts)
        {
            ShowAlert("하트가 가득 차야 방패를 살 수 있습니다!");
            return;
        }

        // 3. 코인이 15개 이상인지 체크
        if (currentCoins < shieldPrice)
        {
            ShowAlert("코인이 부족합니다! (15개 필요)");
            return;
        }

        // 4. 구매 확정
        PlayerStats.Instance.coins -= shieldPrice;
        PlayerStats.Instance.shield += 1; // PlayerStats에 만든 변수 증가

        ShowAlert("방패 구매 완료! 위기 시 1회 보호됩니다.");
        
        Debug.Log($"방패 구매 완료! 남은 코인: {PlayerStats.Instance.coins}, 현재 방패: {PlayerStats.Instance.shield}");
    }

    void ShowAlert(string message)
    {
        if (alertText != null)
        {
            alertText.text = message;
            CancelInvoke("ClearAlert");
            Invoke("ClearAlert", 2f);
        }
    }

    void ClearAlert()
    {
        if (alertText != null) alertText.text = "";
    }
}