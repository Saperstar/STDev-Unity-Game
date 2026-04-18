using UnityEngine;

public class InfiniteLoop : MonoBehaviour
{
    [Header("무한 루프 설정")]
    public float resetX = 20f;  // 화면 오른쪽 밖으로 나가면
    public float startX = -10f; // 화면 왼쪽 시작점으로 텔레포트!

    private PlayerController player;

    void Start()
    {
        // 텔레포트할 때 멱살 잡고 멈추기 위해 플레이어 컨트롤러를 찾아옵니다.
        player = GetComponent<PlayerController>();
    }

    void Update()
    {
        // 오른쪽으로 나갔거나, 실수로 너무 위/아래로 날아갔을 때 (우주 미아 방지)
        if (transform.position.x > resetX || transform.position.y > 15f || transform.position.y < -15f)
        {
            // 1. 위치 초기화 (왼쪽 처음 위치, Y축도 0으로 깔끔하게 리셋)
            transform.position = new Vector3(startX, 0f, transform.position.z);

            // 2. 비행 고집 꺾기! (강제 정지)
            if (player != null)
            {
                player.StopForcefully();   // 엔진 끄기
            }

            // 3. 만약 GraphManager로 선을 그렸다면, 여기서 선도 같이 지워주면 완벽합니다!
            // GraphManager.Instance.ClearGraph(); (이런 식의 함수가 있다면 추가해 주세요)
        }
    }
}