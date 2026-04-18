using UnityEngine;
using TMPro; // 텍스트가 TextMeshPro라면 이 줄이 필요합니다.
using UnityEngine.UI; // 일반 Legacy Text를 사용 중이라면 이 줄이 필요합니다.

public class CoinDisplay : MonoBehaviour
{
    // 유니티 에디터에서 연결할 텍스트 변수
    // 만약 TextMeshPro를 쓰시면 TMP_Text, 일반 텍스트면 Text라고 적으세요.
    public TMP_Text coinText; 

    // 씬이 시작되거나, 오브젝트가 활성화될 때 실행됩니다.
    void Start()
    {
        UpdateCoinDisplay();
    }

    // 이 함수가 실행되면 현재 저장된 코인 값을 텍스트에 적용합니다.
    public void UpdateCoinDisplay()
    {
        if (PlayerStats.Instance != null)
        {
            // PlayerStats에 저장된 숫자를 문자로 바꿔서 텍스트에 넣습니다.
            coinText.text = PlayerStats.Instance.coins.ToString();
        }
        else
        {
            Debug.LogWarning("PlayerStats 인스턴스를 찾을 수 없습니다!");
        }
    }
}