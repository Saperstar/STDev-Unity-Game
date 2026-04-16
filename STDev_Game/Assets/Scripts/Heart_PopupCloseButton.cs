using UnityEngine;

public class Heart_PopupCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject popup;

    public void ClosePopup()
    {
        popup.SetActive(false);

        Time.timeScale = 1f; // 게임 다시 진행
    }
}
