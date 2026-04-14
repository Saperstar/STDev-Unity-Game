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
    private List<GameObject> allStars = new List<GameObject>();

    // ★ WandController가 에러 없이 읽을 수 있도록 만든 '공개용 속성' (에러 해결 핵심!)
    public bool IsMoving => isMoving;

    void Start()
    {
        startPosition = transform.position;

        // 시작할 때 맵의 모든 별을 찾아둠
        GameObject[] starsInScene = GameObject.FindGameObjectsWithTag("Star");
        allStars.AddRange(starsInScene);
    }

    public void StartMoving()
    {
        // 선이 없으면 출발 안 함
        if (pathLine == null || pathLine.positionCount < 2) return;

        // 1. 이어가기 로직: 내 위치에서 가장 가까운 다음 점 찾기
        int nextIndex = 0;
        for (int i = 0; i < pathLine.positionCount; i++)
        {
            // 선의 x좌표들을 검사해서, 내 캐릭터의 x좌표보다 같거나 큰(오른쪽에 있는) 첫 번째 점을 찾음
            if (pathLine.GetPosition(i).x >= transform.position.x - 0.05f)
            {
                nextIndex = i;
                break; // 찾았으면 반복문 종료
            }
        }

        // 2. 찾은 지점(내 발밑)부터 다시 롤러코스터 타기 시작!
        currentPointIndex = nextIndex;
        isMoving = true;
    }

    // ★ 우클릭을 눌렀을 때 멈추게 하는 함수
    public void StopMoving()
    {
        isMoving = false; // 이동 멈춤!
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
        else if (other.CompareTag("Gear"))
        {
            Debug.Log("톱니바퀴 충돌! 리셋합니다.");
            ResetStage();
        }
    }

    void CheckWinCondition()
    {
        if (collectedStars > 0)
        {
            Debug.Log("성공! 다음 스테이지로.");
        }
        else
        {
            Debug.Log("실패! 별을 못 먹었습니다.");
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
        collectedStars = 0;
    }
}