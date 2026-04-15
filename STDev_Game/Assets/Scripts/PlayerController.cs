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

    public bool IsMoving => isMoving;

    public void StartMoving()
    {
        if (pathLine == null || pathLine.positionCount < 2) return;
        isMoving = true;
    }

    public void StopMoving()
    {
        isMoving = false;
    }

    // ★ WandController가 무조건 이 함수를 부르도록 할 겁니다!
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
        // [수정됨] 여기서 우클릭 감지하던 코드를 지웠습니다! 이제 WandController가 혼자 담당합니다.

        if (!isMoving) return;

        Vector3 targetPos = pathLine.GetPosition(currentPointIndex);

        // 회전 로직
        Vector3 direction = targetPos - transform.position;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // 이동 로직
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathLine.positionCount)
            {
                isMoving = false;
                CheckFinalCondition();
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
        }
        else if (other.CompareTag("TreasureMap"))
        {
            other.gameObject.SetActive(false);
            isMoving = false;
            if (GameManager.Instance != null) GameManager.Instance.CheckClearCondition();
        }
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
    }

    void CheckFinalCondition()
    {
        if (GameManager.Instance != null) GameManager.Instance.LoseHeart();
    }
}