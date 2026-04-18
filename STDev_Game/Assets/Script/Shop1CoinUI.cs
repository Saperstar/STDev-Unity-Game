using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class Shop1CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinText; // 코인 개수를 표시할 UI 텍스트 연결

    void Start()
    {
        UpdateCoinDisplay();
    }

    void Update()
    {
        // 상점에서 물건을 샀을 때 실시간으로 반영되도록 Update에서도 호출하거나,
        // 필요할 때만 호출하도록 최적화할 수 있습니다.
        UpdateCoinDisplay();
    }

    public void UpdateCoinDisplay()
    {
        if (PlayerStats.Instance != null && coinText != null)
        {
            // PlayerStats에 저장된 전체 코인 개수를 가져와서 텍스트에 반영
            coinText.text = PlayerStats.Instance.coins.ToString();
        }
    }
}