using UnityEngine;

public class Heart_GameStartController : MonoBehaviour
{
    public static Heart_GameStartController Instance { get; private set; }

    [SerializeField] private GameObject startPopup;

    public bool HasStarted { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HasStarted = false;
        Time.timeScale = 0f;

        if (startPopup != null)
        {
            startPopup.SetActive(true);
        }
    }

    private void Update()
    {
        if (HasStarted)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (HasStarted)
        {
            return;
        }

        HasStarted = true;

        if (startPopup != null)
        {
            startPopup.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
