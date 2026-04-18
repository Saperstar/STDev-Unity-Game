using UnityEngine;
using TMPro;

public class Shop1CoinGamble : MonoBehaviour
{
    [Header("설정")]
    public int gambleCost = 10;     // 도박 1회 비용

    [Header("UI 연결")]
    public TextMeshProUGUI alertText;

   public void DoubleOrNothing()
    {
        if (PlayerStats.Instance == null) return;

        // 1. 최소 비용(10코인)이 있는지 확인
        if (PlayerStats.Instance.coins < gambleCost)
        {
            ShowAlert("도박 비용(10코인)이 부족합니다!");
            return;
        }

        // 2. 일단 도박 비용 10코인을 먼저 지불 (실패하든 성공하든 일단 냄)
        PlayerStats.Instance.coins -= gambleCost;
        
        // 3. 50% 확률 계산
        int result = Random.Range(0, 2);

        if (result == 0)
        {
            // [실패] 그냥 10원만 잃고 끝!
            // (기존의 coins = 0 코드를 삭제했습니다.)
            ShowAlert($"꽝! {gambleCost}코인을 잃었습니다.");
            Debug.Log($"도박 실패: 남은 코인 {PlayerStats.Instance.coins}");
        }
        else
        {
            // [성공] 건 돈의 2배(20원)를 돌려받음
            int reward = gambleCost * 2;
            PlayerStats.Instance.coins += reward; 
            
            ShowAlert($"성공! {reward}코인을 획득했습니다!");
            Debug.Log($"도박 성공: 현재 코인 {PlayerStats.Instance.coins}");
        }
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