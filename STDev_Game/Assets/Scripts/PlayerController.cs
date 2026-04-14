using UnityEngine;
using System.Collections.Generic; // List를 쓰기 위해 필요

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public LineRenderer pathLine;
    public float speed = 2f;

    private bool isMoving = false;
    private int currentPointIndex = 0;
    private int collectedStars = 0;
    private Vector3 startPosition;

    // ★ 맵에 있는 모든 별들을 담아둘 리스트
    private List<GameObject> allStars = new List<GameObject>();

    void Start()
    {
        startPosition = transform.position;

        // ★ 시작할 때 맵에서 "Star" 태그가 붙은 모든 물체를 찾아서 리스트에 저장합니다.
        GameObject[] starsInScene = GameObject.FindGameObjectsWithTag("Star");
        allStars.AddRange(starsInScene);
    }

    public void StartMoving()
    {
        if (pathLine == null || pathLine.positionCount < 2) return;

        transform.position = pathLine.GetPosition(0);
        currentPointIndex = 1;
        collectedStars = 0;
        isMoving = true;
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
            other.gameObject.SetActive(false); // 별 먹기 (비활성화)
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

    // ★ 리셋할 때 실행되는 함수
    void ResetStage()
    {
        isMoving = false;
        transform.position = startPosition; // 위치 초기화

        // ★ 리스트에 담아둔 모든 별을 다시 활성화합니다.
        foreach (GameObject star in allStars)
        {
            if (star != null)
            {
                star.SetActive(true);
            }
        }

        collectedStars = 0; // 점수 초기화
    }
}