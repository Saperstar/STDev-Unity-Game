using UnityEngine;

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    public GameObject endingUI;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowEnding()
    {
        if (endingUI == null)
        {
            Debug.LogError("EndingUI가 연결되지 않았습니다.");
            return;
        }

        endingUI.SetActive(true);
    }
}
