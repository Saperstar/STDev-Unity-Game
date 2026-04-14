using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public LineRenderer pathLine;
    public float speed = 2f;

    // 내부에서만 쓰는 변수들
    private bool isMoving = false;
    private int currentPointIndex = 0;
    private int collectedStars = 0;
    private Vector3 startPosition;

    // 리셋을 위해 맵의 별과 하트를 기억해둘 리스트
    private List<GameObject> allStars = new List<GameObject>();
    private List<GameObject> allHearts = new List<GameObject>(); // ★ 하트용 리스트 추가

    // ★ WandController나 다른 곳에서 읽을 수 있는 공개용 속성들
    public bool IsMoving => isMoving;
    public bool HasHeartKey { get; private set; } = false; // ★ 하트 스테이지 입장 권한 (기본값: 없음)

    void Start()
    {
        startPosition = transform.position;

        // 시작할 때 맵의 모든 별과 하트를 찾아둠
        allStars.AddRange(GameObject.FindGameObjectsWithTag("Star"));
        allHearts.AddRange(GameObject.FindGameObjectsWithTag("Heart")); // ★ 하트 찾기 추가
    }

    public void StartMoving()
    {
        if (pathLine == null || pathLine.positionCount < 2) return;

        int nextIndex = 0;
        for (int i = 0; i < pathLine.positionCount; i++)
        {
            if (pathLine.GetPosition(i).x >= transform.position.x - 0.05f)
            {
                nextIndex = i;
                break;
            }
        }

        currentPointIndex = nextIndex;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    void Update()
    {
        if (!isMoving) return;

        Vector3 targetPos = pathLine.GetPosition(currentPointIndex);
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathLine.positionCount)
            {
                isMoving = false;
                CheckWinCondition();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star"))
        {
            collectedStars++;
            other.gameObject.SetActive(false);
            Debug.Log("별 획득! 현재: " + collectedStars);
        }
        else if (other.CompareTag("Heart"))
        {
            HasHeartKey = true;
            other.gameObject.SetActive(false);
            Debug.Log("❤️ 하트 획득!");
        }
        // ★ 보물지도 로직 추가
        else if (other.CompareTag("TreasureMap"))
        {
            other.gameObject.SetActive(false);
            Debug.Log("🗺️ 보물지도를 찾았습니다! 즉시 클리어!");

            isMoving = false; // 즉시 이동 멈춤

            // 지도를 먹었을 때는 별이 0개라도 성공으로 처리하기 위해 직접 성공 함수 호출
            MissionComplete(true);
        }
        else if (other.CompareTag("Gear"))
        {
            ResetStage();
        }
    }

    void CheckWinCondition()
    {
        // 선의 끝에 도달했을 때 호출되는 기본 체크
        MissionComplete(collectedStars > 0);
    }

    // ★ 성공/실패 처리를 통합한 함수
    void MissionComplete(bool isSuccess)
    {
        if (isSuccess)
        {
            Debug.Log("🎉 스테이지 클리어!");
            // 여기에 다음 스테이지 이동 로직 추가
        }
        else
        {
            Debug.Log("별을 하나도 못 먹고 끝에 도달했습니다. 실패!");
            ResetStage();
        }
    }

    void ResetStage()
    {
        isMoving = false;
        transform.position = startPosition; // 제자리로

        // 먹었던 별들 다시 켜기
        foreach (GameObject star in allStars)
        {
            if (star != null) star.SetActive(true);
        }

        // ★ 먹었던 하트도 다시 켜고, 권한 몰수하기
        foreach (GameObject heart in allHearts)
        {
            if (heart != null) heart.SetActive(true);
        }

        collectedStars = 0;
        HasHeartKey = false; // 리셋됐으니 하트 권한도 다시 사라짐
    }
}