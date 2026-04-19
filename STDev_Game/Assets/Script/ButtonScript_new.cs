using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript_new : MonoBehaviour
{
    [Header("이동할 스테이지 이름")]
    public string stageToLoad = "Chapter1_Stage01";

    [Header("이 자물쇠가 속한 챕터 번호")]
    public int chapterID = 1;

    [Header("하트 검사 무시 (회복소/상점 등 입장 시)")]
    public bool ignoreHeartCheck = false;

    [Header("하트 1칸 회복 (회복소에서 나갈 때)")]
    public bool isHealButton = false;

    public void GoNext()
    {
        // 1. 하트 1칸 회복 로직 (회복 버튼일 때만 작동)
        if (isHealButton && PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddHeart(1); // 무조건 1칸 고정 회복

            // 최대 하트 개수(3개)를 넘지 않게 안전장치
            if (PlayerStats.Instance.hearts > 3)
            {
                PlayerStats.Instance.hearts = 3;
            }

            Debug.Log("하트 1칸 회복! 현재 하트: " + PlayerStats.Instance.hearts);
        }

        // 2. 하트 0개 입장 컷 (일반 스테이지 입장 시 작동)
        if (!ignoreHeartCheck && PlayerStats.Instance != null && PlayerStats.Instance.hearts <= 0)
        {
            Debug.Log("하트가 부족해서 입장할 수 없습니다!");
            return; // 씬 이동 취소
        }

        // 3. 클릭한 자물쇠의 UI 좌표 및 챕터 ID 저장
        Vector2 btnPos = GetComponent<RectTransform>().anchoredPosition;
        PlayerPrefs.SetFloat("MapPosX", btnPos.x);
        PlayerPrefs.SetFloat("MapPosY", btnPos.y);
        PlayerPrefs.SetInt("SavedChapterID", chapterID);

        PlayerPrefs.SetInt("HasMapSaved", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene(stageToLoad);
    }
}