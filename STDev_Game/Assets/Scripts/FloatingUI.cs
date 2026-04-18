using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("둥둥이 설정")]
    public float speed = 5f;   // 위아래로 움직이는 속도 (클수록 빠름)
    public float height = 10f; // 위아래로 움직이는 폭 (클수록 높이 뜀)

    private Vector3 startPos;

    void Start()
    {
        // 게임 시작할 때 원래 내 위치를 기억해둔다!
        startPos = transform.localPosition;
    }

    void Update()
    {
        // 🌟 수학의 마법: Sin(사인) 그래프를 이용해 부드럽게 위아래로 왕복!
        float newY = startPos.y + (Mathf.Sin(Time.time * speed) * height);

        // 내 위치를 새로운 Y값으로 계속 업데이트
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}