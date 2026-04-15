using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public LineRenderer pathLine;
    public float speed = 2f;

    private bool isMoving = false;
    private int currentPointIndex = 0;
    private int collectedStars = 0;
    private Vector3 startPosition;

    private List<GameObject> allStars = new List<GameObject>();
    private List<GameObject> allHearts = new List<GameObject>();

    public bool IsMoving => isMoving;
    public bool HasHeartKey { get; private set; } = false;

    void Start()
    {
        startPosition = transform.position;
        allStars.AddRange(GameObject.FindGameObjectsWithTag("Star"));
        allHearts.AddRange(GameObject.FindGameObjectsWithTag("Heart"));
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

        // --- [★ 추가된 회전 로직: 가야 할 방향 쳐다보기] ---
        // 1. 방향 벡터 구하기 (목표 위치 - 현재 위치)
        Vector3 direction = targetPos - transform.position;

        // 2. 캐릭터가 제자리에 멈춰있지 않을 때만 회전
        if (direction != Vector3.zero)
        {
            // 3. Atan2 함수로 정확한 각도(Angle) 계산
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 4. 계산된 각도를 캐릭터의 Z축 회전에 덮어씌우기
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        // --------------------------------------------------

        // 기존 이동 로직 (이건 건드리지 마세요!)
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
            if (GameManager.Instance != null) GameManager.Instance.AddStar();

            other.gameObject.SetActive(false);
            Debug.Log("별 획득! 현재: " + collectedStars);
        }
        else if (other.CompareTag("Heart"))
        {
            HasHeartKey = true;
            other.gameObject.SetActive(false);
            Debug.Log("❤️ 하트 획득!");
        }
        else if (other.CompareTag("TreasureMap"))
        {
            other.gameObject.SetActive(false);
            isMoving = false;

            // 보물지도를 먹으면 심판에게 판정 요청
            if (GameManager.Instance != null) GameManager.Instance.CheckClearCondition();
        }
        // ★ 두 개로 나뉘어 있던 몬스터 코드를 완벽하게 하나로 합쳤습니다!
        else if (other.CompareTag("Monster"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // 물리적 속도 제거 (최신 API)
                rb.bodyType = RigidbodyType2D.Kinematic; // 물리 엔진 영향 일시 정지 (최신 API)
            }

            // [1단 잠금] 이동 플래그 끄기
            isMoving = false;

            // [2단 잠금] 현재 위치 고정
            transform.position = transform.position;

            // 하트 깎기
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoseHeart();
            }

            // [3단 잠금] 즉시 종료
            return;
        }
    }

    void CheckWinCondition()
    {
        // 선의 끝(도착점)에 도달했는데 이 함수가 실행됐다는 건, 
        // 중간에 보물지도를 먹지(터치하지) 못하고 지나쳤다는 뜻입니다! (즉, 실패)
        if (GameManager.Instance != null)
        {
            Debug.Log("보물지도를 놓치고 선의 끝에 도달했습니다! 하트 감소!");

            // 기존 코드: GameManager.Instance.CheckClearCondition(); 
            // ⬇️ 새로운 코드: 독수리에 닿은 것처럼 자비 없이 하트를 깎습니다!
            GameManager.Instance.LoseHeart();
        }
    }

    void ResetStage()
    {
        isMoving = false;
        transform.position = startPosition;

        foreach (GameObject star in allStars)
        {
            if (star != null) star.SetActive(true);
        }

        foreach (GameObject heart in allHearts)
        {
            if (heart != null) heart.SetActive(true);
        }

        collectedStars = 0;
        HasHeartKey = false;
    }
}