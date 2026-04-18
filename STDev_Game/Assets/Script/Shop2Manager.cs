using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 이 줄을 추가해야 합니다!

public class Shop2Manager : MonoBehaviour
{
    void Update()
    {
        // 새로운 Input System 방식의 키 입력 체크
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("새로운 Input System으로 ESC 감지!");
            SceneManager.LoadScene("SelectMenu2");
        }
    }
}