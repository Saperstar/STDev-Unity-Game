using UnityEngine;

public class StageInfo : MonoBehaviour
{
    [Header("스테이지 기본 설정")]
    public string nextStageName;
    public int maxStops = 3;
    public bool[] allowedWands = new bool[3] { true, false, false };

    [Header("이미지 설정")]
    public Sprite coinImage;
    public Sprite monsterImage;
    public Sprite stageBackground;

    [Header("맵 이동 및 챕터 설정 🚀")]
    public Vector3 nextStageMapPos;         // 다음 스테이지 자물쇠의 맵 좌표
    public string returnMapName = "SelectMenu1"; // 돌아갈 메인 메뉴 씬 이름 (예: SelectMenu2)
    public int chapterID = 1;               // 현재 스테이지의 챕터 번호
    public int nextChapterID = 1;           // 다음 스테이지의 챕터 번호
}