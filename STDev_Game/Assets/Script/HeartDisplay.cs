using UnityEngine;
using TMPro; // 텍스트메시 프로 사용

public class HeartDisplay : MonoBehaviour
{
    // TMP_Text 대신 TextMeshProUGUI라고 풀네임을 적어줍니다.
    // 이렇게 하면 유니티가 훨씬 더 잘 찾아냅니다.
    public TextMeshProUGUI heartText; 

    void Start()
    {
        UpdateHeartUI();
    }

    public void UpdateHeartUI()
    {
        // 안전하게 Instance와 heartText가 있는지 확인 후 실행
        if (PlayerStats.Instance != null && heartText != null)
        {
            heartText.text = "x " + PlayerStats.Instance.hearts.ToString();
        }
    }
}