using UnityEngine;

public class PlayerPositionReloader : MonoBehaviour
{
    [Header("현재 맵의 챕터 번호")]
    public int myChapterID = 1;

    [Header("처음 들어왔을 때 기본 시작 위치")]
    public Vector2 defaultPosition = new Vector2(-1000f, -400f);

    void Start()
    {
        int savedID = PlayerPrefs.GetInt("SavedChapterID", 0);

        // 금고에 저장된 챕터 번호가 지금 이 맵의 챕터 번호와 같을 때만 이동!
        if (PlayerPrefs.GetInt("HasMapSaved", 0) == 1 && savedID == myChapterID)
        {
            float x = PlayerPrefs.GetFloat("MapPosX");
            float y = PlayerPrefs.GetFloat("MapPosY");

            GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        }
        else
        {
            // 챕터가 다르거나 처음 시작했다면 기본 위치로!
            GetComponent<RectTransform>().anchoredPosition = defaultPosition;
        }
    }
}