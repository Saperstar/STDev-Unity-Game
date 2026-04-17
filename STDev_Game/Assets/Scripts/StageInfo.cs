using UnityEngine;

public class StageInfo : MonoBehaviour
{
    [Header("--- 스테이지 기본 설정 ---")]
    public string nextStageName;
    public int maxStops = 3;
    public bool[] allowedWands = new bool[3] { true, false, false };

    [Header("--- 스테이지 비주얼 (이미지) ---")]
    public Sprite stageBackground;
    public Sprite coinImage;
    public Sprite monsterImage; // birdImage 였던 것을 변경!
}