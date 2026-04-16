using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("결과 UI 연결")]
    public GameObject resultCanvas;
    public TextMeshProUGUI coinCountText;

    [Header("하트 UI 연결 (애니메이션)")]
    public Animator[] heartAnimators;

    [Header("정지 횟수 설정")]
    // ★ 유니티 인스펙터에서 직접 지정 가능!
    public TextMeshProUGUI stopCountText;
    public int maxStops = 3;
    private int currentStops;

    [Header("데이터")]
    public int currentCoins = 0;
    public string nextStageName;

    private int currentHearts;
    private int maxHearts = 3;

    [Header("코인 수집 상태")]
    public bool[] collectedCoins = new bool[3]; // [False, False, False] 로 시작
    public Animator[] resultCoinAnimators;    // 결과창에 있는 코인 3개의 애니메이터 연결

    [Header("스테이지 지팡이 개별 설정")]
    // 인스펙터에서 체크박스 3개가 뜰 겁니다. (0번: 1차, 1번: 2차, 2번: 3차)
    public bool[] allowedWands = new bool[3] { true, false, false };



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

    public void AddCoin()
    {
        currentCoins++;
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
        if (currentCoins > 0) ShowClearUI();
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

    // 특정 인덱스의 코인을 획득했을 때 호출
    public void AddSpecificCoin(int index)
    {
        if (index >= 0 && index < collectedCoins.Length)
        {
            collectedCoins[index] = true;
            Debug.Log(index + "번 코인 획득!");
        }
    }

    // 결과창을 띄울 때 호출할 함수
    void ShowClearUI()
    {
        resultCanvas.SetActive(true);
        Time.timeScale = 0f;

        // ★ 결과창 코인 애니메이션 연출 루프
        for (int i = 0; i < collectedCoins.Length; i++)
        {
            if (collectedCoins[i] == true)
            {
                // 먹은 코인이라면 '띵띵' 애니메이션 실행!
                resultCoinAnimators[i].SetTrigger("OnPop");
            }
            else
            {
                // 안 먹은 코인이라면 회색(비활성화) 상태 유지 (애니메이션 안 함)
                // 기본 이미지를 회색 코인으로 해두면 됩니다.
            }
        }
    }

    public void RestartStage() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void GoToNextStage() { if (!string.IsNullOrEmpty(nextStageName)) SceneManager.LoadScene(nextStageName); }
    public void GoToMainMenu() { SceneManager.LoadScene("MainMenu"); }
}