using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class MainMenuController : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    // 이렇게 public으로 열어두면 코드 수정 없이 유니티 화면에서 타겟 씬을 바꿀 수 있습니다!
    public string nextSceneName = "Chapter1_Stage01";

    public void OnClickStartButton()
    {
        // 입력된 씬 이름으로 이동
        SceneManager.LoadScene(nextSceneName);
    }
}