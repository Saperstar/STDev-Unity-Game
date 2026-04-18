using UnityEngine;
using TMPro;

public class Shop1HeartPurchase : MonoBehaviour
{
    [Header("설정")]
    public int heartPrice = 10;     // 하트 가격
    public int maxHearts = 3;       // 최대 하트 개수

    [Header("UI 연결 (선택사항)")]
    public TextMeshProUGUI alertText; // 경고 메시지를 띄울 텍스트

    void Start()
    {
        if (alertText != null) alertText.text = ""; // 시작할 때 경고창 비우기
    }

    // 하트 구매 버튼에 연결할 함수
    public void BuyHeart()
    {
        // 1. PlayerStats 인스턴스가 있는지 확인
        if (PlayerStats.Instance == null) return;

        int currentCoins = PlayerStats.Instance.coins;
        int currentHearts = PlayerStats.Instance.hearts;

        // 2. 조건 체크: 하트가 이미 풀(3개)인지 확인
        if (currentHearts >= maxHearts)
        {
            ShowAlert("하트가 이미 가득 찼습니다!");
            return;
        }

        // 3. 조건 체크: 코인이 충분한지 확인
        if (currentCoins < heartPrice)
        {
            ShowAlert("코인이 부족합니다!");
            return;
        }

        // 4. 구매 진행
        // 코인 차감
        PlayerStats.Instance.coins -= heartPrice;
        // 하트 추가
        PlayerStats.Instance.hearts += 1;

        ShowAlert("하트를 구매했습니다!");
        
        // (참고) 이전 단계에서 만든 코인 표시 UI가 있다면 자동으로 업데이트됩니다.
        Debug.Log($"구매 완료! 남은 코인: {PlayerStats.Instance.coins}, 현재 하트: {PlayerStats.Instance.hearts}");
    }

    // 경고 메시지 표시 및 사라지게 하기
    void ShowAlert(string message)
    {
        if (alertText != null)
        {
            alertText.text = message;
            CancelInvoke("ClearAlert"); // 이미 실행 중인 예약이 있다면 취소
            Invoke("ClearAlert", 2f);   // 2초 뒤에 메시지 삭제
        }
        else
        {
            Debug.Log(message);
        }
    }

    void ClearAlert()
    {
        if (alertText != null) alertText.text = "";
    }
}