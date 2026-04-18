using UnityEngine;
using TMPro;
using System.Linq; // For .Any()

public class MathManager : MonoBehaviour
{
    public static MathManager Instance;

    [Header("UI")]
    public TextMeshProUGUI questionText;
    public TMP_InputField answerInput;

    private string[] correctAnswers;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartQuestion()
    {
        answerInput.text = "";
        GenerateTrigQuestion();
    }

    void GenerateTrigQuestion()
    {
        // (Question, Allowed Answers)
        (string question, string[] answers)[] problems =
        {
            // sin
            ("sin(30°)", new[] { "1/2", "0.5" }),
            ("sin(45°)", new[] { "R2/2", "0.7", "0.707" }),
            ("sin(60°)", new[] { "R3/2", "0.8", "0.86", "0.866" }),
            ("sin(90°)", new[] { "1", "1.0" }),

            // cos
            ("cos(30°)", new[] { "R3/2", "0.8", "0.86", "0.866" }),
            ("cos(45°)", new[] { "R2/2", "0.7", "0.707" }),
            ("cos(60°)", new[] { "1/2", "0.5" }),
            ("cos(90°)", new[] { "0", "0.0" }),

            // tan
            ("tan(30°)", new[] { "1/R3", "R3/3", "0.57", "0.577" }),
            ("tan(45°)", new[] { "1", "1.0" }),
            ("tan(60°)", new[] { "R3", "1.7", "1.73", "1.732" })
        };

        int index = Random.Range(0, problems.Length);

        // 영어 가이드 문구로 교체
        // Use 'R' for Square Root (e.g., √3/2 -> R3/2)
        questionText.text = $"{problems[index].question} = ?\n<size=60%>(Use 'R' for √, e.g., R3/2)</size>";

        correctAnswers = problems[index].answers;
    }

    public void SubmitAnswer()
    {
        if (CheckAnswer(answerInput.text))
        {
            Debug.Log("Correct!");
            GameManager.Instance.StartAttackMiniGame();
        }
        else
        {
            Debug.Log("Wrong!");
            GameManager.Instance.StartDodgeMiniGame();
        }
    }

    bool CheckAnswer(string userAnswer)
    {
        string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace(" ", "").Replace("\n", "").Replace("\r", "").ToUpper();
        }

        string normalizedUserAnswer = Normalize(userAnswer);

        return correctAnswers.Any(ans => Normalize(ans) == normalizedUserAnswer);
    }
}



