using UnityEngine;
using TMPro;
using System.Linq;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance;

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

    void Start()
    {
        StartQuestion();
    }

    public void StartQuestion()
    {
        answerInput.text = "";
        answerInput.ActivateInputField(); // 입력창 자동 포커스

        if (currentMode == QuestionMode.Calculus)
            GenerateCalculusQuestion();
        else
            GenerateTrigQuestion();
    }

    // --- [미분 문제] ---
    void GenerateCalculusQuestion()
    {
        int a = Random.Range(1, 10);
        int b = Random.Range(1, 10);
        int c = Random.Range(1, 10);

        string formula = $"{FormatTerm(a, "x²")} + {FormatTerm(b, "x")} + {c}";
        string answer = $"{2 * a}x + {b}";

        questionText.text = $"f(x) = {formula}\nf'(x) = ?";
        correctAnswers = new[] { answer.Replace(" ", "").ToLower() };
    }

    // --- [삼각함수 문제] 랜덤 조합 버전 (tan 90 제외) ---
    void GenerateTrigQuestion()
    {
        string[] funcs = { "sin", "cos", "tan" };
        int[] angles = { 30, 45, 60, 90 };

        string func = funcs[Random.Range(0, funcs.Length)];
        int angle = angles[Random.Range(0, angles.Length)];

        // tan(90°)는 정의되지 않으므로 제외하고 다시 뽑기
        if (func == "tan" && angle == 90)
        {
            GenerateTrigQuestion();
            return;
        }

        questionText.text = $"{func}({angle}°) = ?";

        // 정답 테이블 (r/R 입력 지원)
        if (func == "sin")
        {
            if (angle == 30) correctAnswers = new[] { "1/2", "0.5" };
            else if (angle == 45) correctAnswers = new[] { "r2/2", "R2/2" };
            else if (angle == 60) correctAnswers = new[] { "r3/2", "R3/2" };
            else if (angle == 90) correctAnswers = new[] { "1" };
        }
        else if (func == "cos")
        {
            if (angle == 30) correctAnswers = new[] { "r3/2", "R3/2" };
            else if (angle == 45) correctAnswers = new[] { "r2/2", "R2/2" };
            else if (angle == 60) correctAnswers = new[] { "1/2", "0.5" };
            else if (angle == 90) correctAnswers = new[] { "0" };
        }
        else if (func == "tan")
        {
            if (angle == 30) correctAnswers = new[] { "r3/3", "R3/3" };
            else if (angle == 45) correctAnswers = new[] { "1" };
            else if (angle == 60) correctAnswers = new[] { "r3", "R3" };
        }
    }

    string FormatTerm(int coef, string var) => (coef == 1) ? var : coef + var;

    // ✅ GameManager_wand 로 이름 변경 적용
    public void SubmitAnswer()
    {
        string input = answerInput.text.Replace(" ", "").ToLower();

        if (correctAnswers != null && correctAnswers.Any(ans => ans.ToLower() == input))
        {
            Debug.Log("정답!");
            // GameManager_wand 인스턴스를 참조합니다.
            GameManager_wand.Instance.StartAttackMiniGame();
        }
        else
        {
            Debug.Log("오답!");
            // GameManager_wand 인스턴스를 참조합니다.
            GameManager_wand.Instance.StartDodgeMiniGame();
        }
    }
}