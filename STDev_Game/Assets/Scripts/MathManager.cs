
using UnityEngine;
using TMPro;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance;

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;

    private string correctAnswer;
    private string currentFunction;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartQuestion()
    {
        GenerateDifferentiationQuestion();

        if (answerInput != null)
            answerInput.text = "";
    }

    void GenerateDifferentiationQuestion()
    {
        bool isCubic = Random.value > 0.5f;

        if (isCubic)
            GenerateCubic();
        else
            GenerateQuadratic();

        questionText.text =
            $"f(x) = {currentFunction}\n\nf'(x) = ?";
    }

    void GenerateQuadratic()
    {
        int a = RandomCoeff();
        int b = RandomCoeff();
        int c = RandomCoeff();

        currentFunction =
            $"{a}x^2 {Sign(b)} {Mathf.Abs(b)}x {Sign(c)} {Mathf.Abs(c)}";

        correctAnswer =
            $"{2 * a}x {Sign(b)} {Mathf.Abs(b)}";
    }

    void GenerateCubic()
    {
        int a = RandomCoeff();
        int b = RandomCoeff();
        int c = RandomCoeff();
        int d = RandomCoeff();

        currentFunction =
            $"{a}x^3 {Sign(b)} {Mathf.Abs(b)}x^2 {Sign(c)} {Mathf.Abs(c)}x {Sign(d)} {Mathf.Abs(d)}";

        correctAnswer =
            $"{3 * a}x^2 {Sign(2 * b)} {Mathf.Abs(2 * b)}x {Sign(c)} {Mathf.Abs(c)}";
    }

    public void SubmitAnswer()
    {
        if (answerInput == null)
            return;

        string userAnswer = answerInput.text;

        Debug.Log($"입력한 답: {userAnswer}");
        Debug.Log($"정답: {correctAnswer}");

        bool isCorrect = CheckAnswer(userAnswer);

        if (isCorrect)
        {
            Debug.Log("✅ 정답 → 공격 미니게임");
            GameManager.Instance.StartAttackMiniGame();
        }
        else
        {
            Debug.Log("❌ 오답 → 회피 미니게임");
            GameManager.Instance.StartDodgeMiniGame();
        }
    }

    bool CheckAnswer(string userAnswer)
    {

        string Normalize(string s)
        {
            return s
                .Replace(" ", "")
                .Replace("\n", "")
                .Replace("\r", "")
                .Replace("\t", "")
                .Replace("−", "-")   // 유니코드 마이너스
                .ToLower();
        }

        string u = Normalize(userAnswer);
        string c = Normalize(correctAnswer);

        Debug.Log($"[ANSWER CHECK]");
        Debug.Log($"User   : '{u}'");
        Debug.Log($"Correct: '{c}'");

        return u == c;

    }

    int RandomCoeff()
    {
        int value = Random.Range(10, 100);
        return Random.value > 0.5f ? value : -value;
    }

    string Sign(int value)
    {
        return value >= 0 ? "+" : "-";
    }
}



