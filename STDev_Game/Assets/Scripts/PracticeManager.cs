using UnityEngine;
using UnityEngine.SceneManagement;

public class PracticeManager : MonoBehaviour
{
    [Header("진짜 첫 스테이지 이름")]
    public string firstStageName = "Chapter1_Stage01";

    // Let's Go 버튼을 누르면 실행될 함수
    public void GoToFirstStage()
    {
        // 1스테이지로 슝!
        SceneManager.LoadScene(firstStageName);
    }
}