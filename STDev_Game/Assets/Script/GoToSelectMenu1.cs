using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

// 반드시 MonoBehaviour을 상속해야 함
public class GoToSelectMenu1 : MonoBehaviour
{
    // 버튼 OnClick 이벤트에 연결할 메서드
    public void LoadSelectMenu1()
    {
        SceneManager.LoadScene("SelectMenu1");
    }
}
