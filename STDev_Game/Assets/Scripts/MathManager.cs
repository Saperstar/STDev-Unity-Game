using UnityEngine;
using TMPro;

public class MathManager : MonoBehaviour
{
    public static MathManager Instance;

    [Header("UI References")]
    public TMP_Text questionText;
    public TMP_InputField answerInput;

    private int correctAnswer;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 새로운 수학 문제 생성
    /// </summary>
    public void StartQuestion()
    {
        int a = Random.Range(10, 100);
        int b = Random.Range(10, 100);

        correctAnswer = a + b;

        if (questionText != null)
            questionText.text = a + " + " + b + " = ?";

        if (answerInput != null)
            answerInput.text = "";
    }

    /// <summary>
    /// 답 제출
    /// </summary>
    public void SubmitAnswer()
    {
        int playerAnswer;

        if (int.TryParse(answerInput.text, out playerAnswer))
        {
            if (playerAnswer == correctAnswer)
            {
                Debug.Log("✅ 정답!");
                GameManager.Instance.StartAttackMiniGame();
            }
            else
            {
                Debug.Log("❌ 오답!");
                GameManager.Instance.StartDodgeMiniGame();
            }
        }
        else
        {
            Debug.Log("숫자를 입력하세요.");
        }
    }
}


