using UnityEngine;
using TMPro; // TextMeshPro 사용을 위해 필요

public class ShieldDisplay : MonoBehaviour
{
    // 유니티 에디터에서 방패 숫자를 표시할 TMP 텍스트를 연결하세요.
    public TMP_Text shieldText; 

    void Start()
    {
        UpdateShieldDisplay();
    }

    void Update()
    {
        // 실시간으로 방패 개수가 변하는 것을 확인하려면 Update에서도 호출합니다.
        UpdateShieldDisplay();
    }

    public void UpdateShieldDisplay()
    {
        if (PlayerStats.Instance != null && shieldText != null)
        {
            // PlayerStats에 저장된 방패(shield) 숫자를 텍스트에 적용합니다.
            shieldText.text = PlayerStats.Instance.shield.ToString();
        }
        else if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("PlayerStats 인스턴스를 찾을 수 없습니다!");
        }
    }
}