using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("결과 UI 연결")]
    public GameObject resultCanvas;
    public TextMeshProUGUI starCountText;

    [Header("하트 UI 연결 (애니메이션)")]
    public Animator[] heartAnimators;

    [Header("정지 횟수 설정")]
    // ★ 유니티 인스펙터에서 직접 지정 가능!
    public TextMeshProUGUI stopCountText;
    public int maxStops = 3;
    private int currentStops;

    [Header("데이터")]
    public int currentStars = 0;
    public string nextStageName;

    private int currentHearts;
    private int maxHearts = 3;

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (resultCanvas != null) resultCanvas.SetActive(false);

        // 씬 시작 시 시간 흐름 정상화
        Time.timeScale = 1f;

        // 하트 데이터 로드
        currentHearts = PlayerPrefs.GetInt("PlayerHearts", maxHearts);

        // 정지 횟수 초기화
        currentStops = maxStops;
        UpdateStopUI();
    }

    void Start()
    {
        // 씬 로드 시 이미 깎여있어야 할 하트들을 'Break' 상태로 강제 고정
        for (int i = 0; i < heartAnimators.Length; i++)
        {
            if (i >= currentHearts)
            {
                heartAnimators[i].Play("Heart_break_Anim", 0, 1.0f);
            }
        }
    }

    public void AddStar()
    {
        currentStars++;
    }

    // 정지 사용 요청 처리
    public bool TryUseStop()
    {
        if (currentStops > 0)
        {
            currentStops--;
            UpdateStopUI();
            return true;
        }
        else
        {
            Debug.Log("정지 횟수 부족!");
            return false;
        }
    }

    void UpdateStopUI()
    {
        if (stopCountText != null)
        {
            stopCountText.text = "STOP LEFT : " + currentStops;
        }
    }

    public void CheckClearCondition()
    {
        if (currentStars > 0) ShowClearUI();
        else LoseHeart();
    }

    public void LoseHeart()
    {
        if (currentHearts <= 0) return;

        int heartToBreakIndex = currentHearts - 1;
        if (heartToBreakIndex >= 0 && heartToBreakIndex < heartAnimators.Length)
        {
            heartAnimators[heartToBreakIndex].SetTrigger("OnBreak");
        }

        currentHearts--;
        PlayerPrefs.SetInt("PlayerHearts", currentHearts);

        if (currentHearts <= 0)
        {
            PlayerPrefs.SetInt("PlayerHearts", maxHearts);
            Invoke("GoToMainMenu", 1.2f);
        }
        else
        {
            Invoke("RestartStage", 1.0f);
        }
    }

    void ShowClearUI()
    {
        resultCanvas.SetActive(true);
        starCountText.text = "Stars: " + currentStars;
        Time.timeScale = 0f;
    }

    public void RestartStage() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void GoToNextStage() { if (!string.IsNullOrEmpty(nextStageName)) SceneManager.LoadScene(nextStageName); }
    public void GoToMainMenu() { SceneManager.LoadScene("MainMenu"); }
}