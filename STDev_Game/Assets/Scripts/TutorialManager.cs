using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [Header("UI 연결")]
    // 하이어라키의 Dialogue (TextMeshPro)를 연결할 곳
    public TextMeshProUGUI tutorialText;

    [Header("튜토리얼 내용")]
    // 엔터 치면서 대사를 입력할 수 있게 해줍니다.
    [TextArea] public string[] dialogues;

    // 각 대사 순서에 맞춰 띄울 화살표들을 넣는 곳
    public GameObject[] stepEffects;

    [Header("이동할 다음 스테이지")]
    public string nextStageName = "Chapter1_Stage01";

    // 현재 몇 번째 대사를 보고 있는지 기억하는 숫자 (0부터 시작)
    private int currentStep = 0;

    void Start()
    {
        // 게임 시작하자마자 첫 번째 대사와 화살표 세팅!
        UpdateTutorialUI();
    }

    // ★ 화면(버튼)을 클릭했을 때 실행될 함수!
    public void OnTapToContinue()
    {
        currentStep++; // 클릭했으니 다음 스텝으로 +1

        if (currentStep < dialogues.Length)
        {
            // 아직 대사가 남았다면, 다음 대사와 화살표로 교체!
            UpdateTutorialUI();
        }
        else
        {
            // 대사가 다 끝났다면, 진짜 게임 스테이지로 슝!
            SceneManager.LoadScene(nextStageName);
        }
    }

    // 대사와 화살표를 갈아끼우는 핵심 로직
    // 대사와 화살표를 갈아끼우는 핵심 로직 (★버그 수정 완료!)
    void UpdateTutorialUI()
    {
        // 1. 텍스트 내용물 교체
        if (currentStep < dialogues.Length)
        {
            tutorialText.text = dialogues[currentStep];
        }

        // 2. 화살표 관리: 일단 무식하게 싹 다 끕니다!
        for (int i = 0; i < stepEffects.Length; i++)
        {
            if (stepEffects[i] != null)
            {
                stepEffects[i].SetActive(false);
            }
        }

        // 3. 그리고 오직 '현재 스텝'에 들어있는 화살표 딱 하나만 켭니다!
        if (currentStep < stepEffects.Length && stepEffects[currentStep] != null)
        {
            stepEffects[currentStep].SetActive(true);
        }
    }
}