using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        // "GameScene"이라는 이름의 씬으로 이동
        SceneManager.LoadScene("GameScene");
    }
}