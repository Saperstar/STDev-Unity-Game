using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("결과 UI 연결")]
    public GameObject resultCanvas;
    public TextMeshProUGUI coinCountText;
    public Animator[] heartAnimators;

    [Header("UI 요소 연결")]
    public TextMeshProUGUI stopCountText;
    public UnityEngine.UI.Image backgroundImageUI;

    // --- [StageInfo에서 받아올 데이터들: 인스펙터에서는 숨김!] ---
    [HideInInspector] public string nextStageName;
    [HideInInspector] public int maxStops;
    [HideInInspector] public bool[] allowedWands;
    [HideInInspector] public Sprite currentCoinSprite;
    // 선언부 수정
    [HideInInspector] public Sprite currentMonsterSprite; // currentBirdSprite 였던 것을 변경!

    // --------------------------------------------------------

    [Header("실시간 데이터")]
    public int currentCoins = 0;
    private int currentStops;
    private int currentHearts;
    private int maxHearts = 3;

    [Header("코인 수집 상태")]
    public bool[] collectedCoins = new bool[3];
    public Animator[] resultCoinAnimators;

    void Awake()
    {
        if (Instance == null) Instance = this;

        // ★ 지시서(StageInfo)에서 데이터 스포이트로 빨아들이기
        StageInfo info = FindAnyObjectByType<StageInfo>();
        if (info != null)
        {
            this.nextStageName = info.nextStageName;
            this.maxStops = info.maxStops;
            this.allowedWands = info.allowedWands;
            this.currentCoinSprite = info.coinImage;
            this.currentMonsterSprite = info.monsterImage; // 이름 맞춰주기

            // 배경화면은 여기서 바로 교체!
            if (backgroundImageUI != null && info.stageBackground != null)
            {
                backgroundImageUI.sprite = info.stageBackground;
            }
        }

        if (resultCanvas != null) resultCanvas.SetActive(false);
        Time.timeScale = 1f;
        currentHearts = PlayerPrefs.GetInt("PlayerHearts", maxHearts);
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
        // [추가할 방어막] 하트 UI 배열이 아예 없거나 비어있으면 그냥 무시하고 함수 종료! (연습장이니까 안 죽음)
        if (heartAnimators == null || heartAnimators.Length == 0)
        {
            return;
        }
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
            }
        }
    }

    public void RestartStage() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }
    public void GoToNextStage() { if (!string.IsNullOrEmpty(nextStageName)) SceneManager.LoadScene(nextStageName); }
    public void GoToMainMenu() { SceneManager.LoadScene("MainMenu"); }
}