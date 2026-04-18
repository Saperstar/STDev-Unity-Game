using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript_new : MonoBehaviour
{
    [Header("이동할 스테이지 이름")]
    public string stageToLoad = "Chapter1_Stage01";

    [Header("이 자물쇠가 속한 챕터 번호")]
    public int chapterID = 1;

    public void GoNext()
    {
        // 클릭한 자물쇠의 UI 좌표 및 챕터 ID 저장
        Vector2 btnPos = GetComponent<RectTransform>().anchoredPosition;
        PlayerPrefs.SetFloat("MapPosX", btnPos.x);
        PlayerPrefs.SetFloat("MapPosY", btnPos.y);
        PlayerPrefs.SetInt("SavedChapterID", chapterID);

        PlayerPrefs.SetInt("HasMapSaved", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(stageToLoad);
    }
}