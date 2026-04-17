using UnityEngine;

public class Heart_GameStartController : MonoBehaviour
{
    public static Heart_GameStartController Instance { get; private set; }

    [Header("Tutorial Popups")]
    [SerializeField] private GameObject photosynthesisPopup;
    [SerializeField] private GameObject dashPopup;
    [SerializeField] private GameObject startPopup;

    public bool HasStarted { get; private set; }

    private int tutorialStep = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        HasStarted = false;
        tutorialStep = 0;

        Time.timeScale = 0f;

        ShowOnlyPopup(photosynthesisPopup);
    }

    private void Update()
    {
        if (HasStarted)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            AdvanceTutorial();
        }
    }

    private void AdvanceTutorial()
    {
        switch (tutorialStep)
        {
            case 0:
                tutorialStep = 1;
                ShowOnlyPopup(dashPopup);
                break;

            case 1:
                tutorialStep = 2;
                ShowOnlyPopup(startPopup);
                break;

            case 2:
                StartGame();
                break;
        }
    }

    private void ShowOnlyPopup(GameObject popupToShow)
    {
        if (photosynthesisPopup != null)
        {
            photosynthesisPopup.SetActive(popupToShow == photosynthesisPopup);
        }

        if (dashPopup != null)
        {
            dashPopup.SetActive(popupToShow == dashPopup);
        }

        if (startPopup != null)
        {
            startPopup.SetActive(popupToShow == startPopup);
        }
    }

    public void StartGame()
    {
        if (HasStarted)
        {
            return;
        }

        HasStarted = true;

        if (photosynthesisPopup != null)
        {
            photosynthesisPopup.SetActive(false);
        }

        if (dashPopup != null)
        {
            dashPopup.SetActive(false);
        }

        if (startPopup != null)
        {
            startPopup.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}