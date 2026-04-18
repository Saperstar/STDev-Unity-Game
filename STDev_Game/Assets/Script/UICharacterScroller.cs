using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 제어를 위해 추가

public class UICharacterScroller : MonoBehaviour
{
    public float speed = 200f;
    public float startX = -1000f;
    public float endX = 1000f;

    // 각 구역에서 사용할 스프라이트 (인스펙터에서 넣어주세요)
    public Sprite walkSprite;   // 숲 구역
    public Sprite flySprite;    // 사막 구역
    public Sprite swimSprite;   // 물 구역

    RectTransform rectTransform;
    Image characterImage;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        characterImage = GetComponent<Image>();
    }

    void Update()
    {
        // 1. 오른쪽으로 이동
        rectTransform.anchoredPosition += Vector2.right * speed * Time.deltaTime;

        // 2. 현재 위치에 따른 상태 체크 (이미지 변경)
        UpdateStateByPosition();

        // 3. 끝에 도착하면 다시 시작점으로
        if (rectTransform.anchoredPosition.x > endX)
        {
            rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
        }
    }

    void UpdateStateByPosition()
    {
        float totalWidth = endX - startX;
        float currentPos = rectTransform.anchoredPosition.x - startX;
        float progress = currentPos / totalWidth; // 0.0 ~ 1.0 사이의 값

        // 구역을 3등분 (0~33% 숲, 33~66% 사막, 66~100% 물)
        if (progress < 0.33f)
        {
            if (walkSprite != null) characterImage.sprite = walkSprite;
            // 필요하다면 여기서 y값을 살짝 조정해서 걷는 높이를 맞출 수 있습니다.
        }
        else if (progress < 0.66f)
        {
            if (flySprite != null) characterImage.sprite = flySprite;
            // 날아갈 때는 y값을 약간 위아래로 흔들리게(Sin함수) 하면 더 좋습니다.
        }
        else
        {
            if (swimSprite != null) characterImage.sprite = swimSprite;
        }
    }
}