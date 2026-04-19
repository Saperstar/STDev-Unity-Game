using UnityEngine;

public class Heart_PauseManager : MonoBehaviour
{
    public static Heart_PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pausePopup;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        IsPaused = false;

        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsPaused)
        {
            return;
        }

        // 아무 곳이나 클릭하면 Resume
        if (Input.GetMouseButtonDown(0))
        {
            ResumeGame();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (IsPaused)
        {
            return;
        }

        if (!Heart_GameStartController.Instance.HasStarted)
        {
            return;
        }

        IsPaused = true;

        if (pausePopup != null)
        {
            pausePopup.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
        {
            return;
        }

        IsPaused = false;

        if (pausePopup != null)
        {
            pausePopup.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}