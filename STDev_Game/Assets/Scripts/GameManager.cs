using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("결과 UI 연결")]
    public GameObject resultCanvas;
    public TextMeshProUGUI starCountText;

    [Header("하트 UI 연결 (애니메이션)")]
    // ★ 수정됨: Image 대신 Animator 배열을 사용합니다.
    public Animator[] heartAnimators;

    [Header("데이터")]
    public int currentStars = 0;
    public string nextStageName;

    private int currentHearts;
    private int maxHearts = 3; // 기본 최대 체력

    void Awake()
    {
        if (Instance == null) Instance = this;
        if (resultCanvas != null) resultCanvas.SetActive(false);
        currentHearts = PlayerPrefs.GetInt("PlayerHearts", maxHearts);

        // 게임 시작 시 컴퓨터 메모리에 저장된 하트 개수를 불러옵니다.
        currentHearts = PlayerPrefs.GetInt("PlayerHearts", maxHearts);

        // ★ 중요: Awake에서는 애니메이션을 재생하지 않고, 
        // 씬 로드 시 멀쩡한 상태(Idle)로 시작하도록 세팅만 합니다.
        // 만약 게임 시작 시 이미 깎인 하트가 있다면, 
        // 나중에 UpdateHeartUIOnLoad() 같은 함수를 만들어 
        // 깎인 상태(애니메이션 끝 프레임)로 강제 고정하는 로직이 필요합니다.
    }

    void Start()
    {
        for (int i = 0; i < heartAnimators.Length; i++)
        {
            // 내 현재 체력보다 순서가 뒤에 있는 하트라면? (이미 깎인 하트)
            if (i >= currentHearts)
            {
                // 애니메이션의 '1.0f(100%, 즉 마지막 프레임)' 위치로 강제 점프시킵니다!
                // 주의: "Heart_break_Anim" 부분은 애니메이터 창에 있는 회색 박스 이름과 똑같아야 합니다.
                heartAnimators[i].Play("Heart_break_Anim", 0, 1.0f);
            }
        }
    }

    public void AddStar()
    {
        currentStars++;
    }

    public void CheckClearCondition()
    {
        if (currentStars > 0)
        {
            Debug.Log("스테이지 클리어 성공!");
            ShowClearUI();
        }
        else
        {
            Debug.Log("별을 하나도 못 먹었습니다! 하트 감소!");
            LoseHeart();
        }
    }

    public void LoseHeart()
    {
        if (currentHearts <= 0) return; // 이미 죽었으면 무시

        // ★ 하트가 깎이기 전에, 현재 깎일 하트의 애니메이션을 재생합니다.
        // 예: 3개 중 하나 깎일 때, 2번 인덱스(세 번째 하트)의 'OnBreak' 트리거를 켭니다.
        int heartToBreakIndex = currentHearts - 1;
        if (heartToBreakIndex >= 0 && heartToBreakIndex < heartAnimators.Length)
        {
            // 애니메이터에게 'OnBreak' 신호를 보냅니다 -> 파사삭 애니메이션 재생!
            heartAnimators[heartToBreakIndex].SetTrigger("OnBreak");
        }

        currentHearts--; // 하트 1개 마이너스
        PlayerPrefs.SetInt("PlayerHearts", currentHearts); // 저장

        if (currentHearts <= 0)
        {
            Debug.Log("하트가 0개가 되었습니다. 게임 오버!");
            PlayerPrefs.SetInt("PlayerHearts", maxHearts); // 하트 초기화

            // ★ 중요: 애니메이션이 재생될 시간을 주기 위해 약간 지연 후 메인 메뉴로 이동합니다.
            Invoke("GoToMainMenu", 1.5f); // 1.5초 후 실행
        }
        else
        {
            // 아직 목숨이 남았다면 현재 스테이지 다시 도전!
            // ★ 역시 애니메이션 재생 시간을 확보하기 위해 지연 후 리셋합니다.
            Invoke("RestartStage", 1.0f); // 1.0초 후 실행
        }
    }

    // ★ 하트 이미지를 갈아끼우는 UpdateHeartUI 함수는 이제 필요 없습니다.
    // 애니메이터가 자동으로 처리하기 때문입니다.

    void ShowClearUI()
    {
        resultCanvas.SetActive(true);
        starCountText.text = "Stars: " + currentStars;
        Time.timeScale = 0f;
    }

    public void RestartStage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToNextStage()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(nextStageName)) SceneManager.LoadScene(nextStageName);
        else Debug.LogError("다음 스테이지 이름이 안 적혀있습니다!");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}