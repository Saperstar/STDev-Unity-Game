using UnityEngine;

public class Monster : MonoBehaviour
{
    void Start()
    {
        // 게임매니저가 가진 이번 스테이지 몬스터 이미지로 내 옷을 갈아입는다!
        if (GameManager.Instance != null && GameManager.Instance.currentMonsterSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = GameManager.Instance.currentMonsterSprite;
        }
    }

    // 나중에 여기에 '플레이어랑 닿으면 하트 깎이는 코드(OnTriggerEnter2D)'를 추가하시면 됩니다!
}