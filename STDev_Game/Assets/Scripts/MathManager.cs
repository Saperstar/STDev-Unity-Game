using UnityEngine;
using TMPro;
using System.Linq;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance;

    // 인스펙터에서 씬마다 모드를 설정할 수 있습니다.
    public enum QuestionMode { Calculus, Trigonometry }
    [Header("Settings")]
    public QuestionMode currentMode;

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;

    private string[] correctAnswers;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public void StartQuestion()
    {
        answerInput.text = "";

        // 현재 씬에 설정된 모드에 따라 다른 문제 생성
        if (currentMode == QuestionMode.Calculus)
            GenerateCalculusQuestion();
        else
            GenerateTrigQuestion();
    }

    // --- [모드 1] 미분 문제 (씬 1용) ---
    void GenerateCalculusQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int c = Random.Range(1, 10);

        // f(x) = ax² + bx + c -> f'(x) = 2ax + b
        string formula = $"{FormatTerm(a, "x²")} + {FormatTerm(b, "x")} + {c}";
        string answer = $"{2 * a}x + {b}"; // 계수 1 생략 로직은 필요시 추가 가능

        questionText.text = $"f(x) = {formula}\nf'(x) = ?";
        correctAnswers = new[] { answer.Replace(" ", "").ToLower() };
    }

    // --- [모드 2] 삼각함수 문제 (씬 2용) ---
    void GenerateTrigQuestion()
    {
        (string q, string[] a)[] trigProblems = {
            ("sin(30°)", new[] {"1/2", "0.5"}),
            ("cos(60°)", new[] {"1/2", "0.5"}),
            ("tan(45°)", new[] {"1"})
        };

        int idx = Random.Range(0, trigProblems.Length);
        questionText.text = $"{trigProblems[idx].q} = ?";
        correctAnswers = trigProblems[idx].a;
    }

    string FormatTerm(int coef, string var) => (coef == 1) ? var : coef + var;

    public void SubmitAnswer()
    {
        string input = answerInput.text.Replace(" ", "").ToLower();
        if (correctAnswers.Any(ans => ans.ToLower() == input))
            GameManager_wand.Instance.StartAttackMiniGame();
        else
            GameManager_wand.Instance.StartDodgeMiniGame();
    }
}

