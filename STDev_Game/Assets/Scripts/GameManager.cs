using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("캐릭터 설정")]
    public GameObject player;

    [Header("결과 UI 연결")]
    public GameObject resultCanvas;
    public TextMeshProUGUI coinCountText;
    public Animator[] heartAnimators;

    [Header("UI 요소 연결")]
    public TextMeshProUGUI stopCountText;
    public UnityEngine.UI.Image backgroundImageUI;

    // --- [StageInfo 데이터] ---
    [HideInInspector] public string nextStageName;
    [HideInInspector] public int maxStops;
    [HideInInspector] public bool[] allowedWands;
    [HideInInspector] public Sprite currentCoinSprite;
    [HideInInspector] public Sprite currentMonsterSprite;
    [HideInInspector] public Vector3 nextStageMapPos;
    [HideInInspector] public string returnMapName;
    [HideInInspector] public int nextChapterID;

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

        StageInfo info = FindAnyObjectByType<StageInfo>();
        if (info != null)
        {
            this.nextStageName = info.nextStageName;
            this.maxStops = info.maxStops;
            this.allowedWands = info.allowedWands;
            this.currentCoinSprite = info.coinImage;
            this.currentMonsterSprite = info.monsterImage;

            // 맵 이동 데이터 받아오기
            this.nextStageMapPos = info.nextStageMapPos;
            this.returnMapName = info.returnMapName;
            this.nextChapterID = info.nextChapterID;

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
        if (heartAnimators != null)
        {
            for (int i = 0; i < heartAnimators.Length; i++)
            {
                if (i >= currentHearts && heartAnimators[i] != null)
                {
                    heartAnimators[i].Play("Heart_break_Anim", 0, 1.0f);
                }
            }
        }
    }

    public void AddCoin()
    {
        currentCoins++;
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddCoin(1);
        }
    }

    public bool TryUseStop()
    {
        if (currentStops > 0)
        {
            currentStops--;
            UpdateStopUI();
            return true;
        }
        return false;
    }

    void UpdateStopUI()
    {
        if (stopCountText != null) stopCountText.text = "STOP LEFT : " + currentStops;
    }

    public void CheckClearCondition()
    {
        if (currentCoins > 0) ShowClearUI();
        else LoseHeart();
    }

    public void LoseHeart()
    {
        // 🛡️ [추가] 방패(Shield) 체크 로직: 하트 로직보다 먼저 실행되어야 함
        if (PlayerStats.Instance != null && PlayerStats.Instance.shield > 0)
        {
            PlayerStats.Instance.shield--; // 방패 1개 감소
            Debug.Log("방패 사용! 남은 방패: " + PlayerStats.Instance.shield);

            // 하트는 잃지 않지만, 장애물에서 떼어놓기 위해 스테이지는 재시작함
            RestartStage();
            return; // ★ 여기서 함수를 끝내서 아래 하트 감소 로직이 안 돌게 함
        }

        // --- 여기서부터 기존 하트 감소 로직 ---
        if (heartAnimators == null || heartAnimators.Length == 0) return;
        if (currentHearts <= 0) return;

        int heartToBreakIndex = currentHearts - 1;
        if (heartToBreakIndex >= 0 && heartToBreakIndex < heartAnimators.Length && heartAnimators[heartToBreakIndex] != null)
        {
            heartAnimators[heartToBreakIndex].SetTrigger("OnBreak");
        }

        currentHearts--;

        if (PlayerStats.Instance != null) PlayerStats.Instance.hearts--;

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

    public void AddSpecificCoin(int index)
    {
        if (index >= 0 && index < collectedCoins.Length) collectedCoins[index] = true;
    }

    void ShowClearUI()
    {
        resultCanvas.SetActive(true);
        Time.timeScale = 0f;

        for (int i = 0; i < collectedCoins.Length; i++)
        {
            if (collectedCoins[i] == true && resultCoinAnimators.Length > i && resultCoinAnimators[i] != null)
            {
                resultCoinAnimators[i].SetTrigger("OnPop");
            }
        }
    }

    public void RestartStage() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); }

    public void GoToNextStage()
    {
        if (!string.IsNullOrEmpty(nextStageName))
        {
            // 다음 맵 좌표 및 챕터 ID 금고에 저장
            PlayerPrefs.SetFloat("MapPosX", nextStageMapPos.x);
            PlayerPrefs.SetFloat("MapPosY", nextStageMapPos.y);
            PlayerPrefs.SetInt("SavedChapterID", nextChapterID);
            PlayerPrefs.SetInt("HasMapSaved", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(nextStageName);
        }
    }

    public void GoToMainMenu()
    {
        // 내 챕터에 맞는 맵으로 귀환
        if (!string.IsNullOrEmpty(returnMapName))
        {
            SceneManager.LoadScene(returnMapName);
        }
        else
        {
            SceneManager.LoadScene("SelectMenu1");
        }
    }
}