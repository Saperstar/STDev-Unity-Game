using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public LineRenderer pathLine;
    public float speed = 2f;

    private bool isMoving = false;
    private int currentPointIndex = 0;
    private int collectedCoins = 0; // [변경] collectedStars -> collectedCoins

    public bool IsMoving => isMoving;
    public bool HasHeartKey { get; private set; } = false;

    public void StopForcefully()
    {
        isMoving = false;
        currentPointIndex = 0;
    }

    public void StartMoving()
    {
        if (pathLine == null || pathLine.positionCount < 2) return;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    public void RequestStop()
    {
        if (!isMoving) return;

        if (GameManager.Instance != null && GameManager.Instance.TryUseStop())
        {
            StopMoving();
            Debug.Log("우클릭 정지 성공!");
        }
        else
        {
            Debug.Log("정지 횟수가 없어 멈출 수 없습니다!");
        }
    }

    void Update()
    {
        // 1. 안 움직이는 상태면 아무것도 안 함
        if (!isMoving) return;

        // 2. [방어막] 선(pathLine)이 없거나 점이 다 지워졌으면 중지!
        // (lineRenderer -> pathLine 으로 수정 완료!)
        if (pathLine == null || pathLine.positionCount <= currentPointIndex)
        {
            currentPointIndex = 0; // 인덱스 초기화
            isMoving = false;      // 에러 나기 전에 이동 강제 종료!
            return;
        }

        // 3. 안전하게 타겟 위치 가져오기 
        Vector3 targetPos = pathLine.GetPosition(currentPointIndex);

        // 4. 타겟을 향해 회전
        Vector3 direction = targetPos - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 5. 타겟을 향해 뚜벅뚜벅 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // 6. 타겟(점)에 도착했는지 확인
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPointIndex++; // 다음 점으로 타겟 변경!

            // 만약 방금 도착한 곳이 마지막 점이었다면?
            if (currentPointIndex >= pathLine.positionCount)
            {
                isMoving = false;      // 이동 끝!
                CheckFinalCondition(); // 도착 후 판정 로직 실행
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 동전(Coin) 획득
        if (other.CompareTag("Coin"))
        {
            collectedCoins++; // [변경]
            // GameManager에 있는 함수 이름도 AddCoin으로 바꾸셔야 에러가 안 납니다!
            if (GameManager.Instance != null) GameManager.Instance.AddCoin();

            CoinItem coin = other.GetComponent<CoinItem>();
            if (coin != null) coin.PopAndDestroy();
            else other.gameObject.SetActive(false);
        }
        // 2. 보물지도 획득
        else if (other.CompareTag("TreasureMap"))
        {
            other.gameObject.SetActive(false);
            isMoving = false;
            if (GameManager.Instance != null) GameManager.Instance.CheckClearCondition();
        }
        // 3. 하트(입장권) 획득
        else if (other.CompareTag("Heart"))
        {
            HasHeartKey = true;
            other.gameObject.SetActive(false);
            Debug.Log("❤️ 하트 스테이지 입장권 획득!");
        }
        // 4. 몬스터 충돌
        else if (other.CompareTag("Monster"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            isMoving = false;
            transform.position = transform.position;

            if (GameManager.Instance != null) GameManager.Instance.LoseHeart();
        }
        if (other.CompareTag("Coin"))
        {
            CoinItem coin = other.GetComponent<CoinItem>();
            if (coin != null)
            {
                // ★ 동전의 번호를 매니저에게 알려줍니다.
                if (GameManager.Instance != null)
                    GameManager.Instance.AddSpecificCoin(coin.coinIndex);

                coin.PopAndDestroy();
            }
        }
    }

    void CheckFinalCondition()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Practice_Stage")
        {
            return;
        }
        if (GameManager.Instance != null) GameManager.Instance.LoseHeart();
    }
}