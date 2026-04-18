using UnityEngine;
using System.Collections;

public class DodgeMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform gaugeBar;
    public RectTransform pointer;
    public RectTransform perfectZone;

    [Header("Settings")]
    public float moveSpeed = 600f;
    public int goodDamage = 10;

    private float minX;
    private float maxX;
    private bool movingRight;
    private bool canCheck;

    void OnEnable()
    {
        StartCoroutine(SetupAfterLayout());
    }

    IEnumerator SetupAfterLayout()
    {
        yield return null;

        float halfWidth = gaugeBar.rect.width * 0.5f;
        minX = -halfWidth;
        maxX = halfWidth;

        pointer.localPosition = new Vector3(minX, 0f, 0f);
        movingRight = true;
        canCheck = true;
    }

    void Update()
    {
        MovePointer();
    }

    void MovePointer()
    {
        Vector3 pos = pointer.localPosition;
        pos.x += (movingRight ? 1f : -1f) * moveSpeed * Time.deltaTime;

        if (pos.x >= maxX)
        {
            pos.x = maxX;
            movingRight = false;
        }
        else if (pos.x <= minX)
        {
            pos.x = minX;
            movingRight = true;
        }

        pointer.localPosition = pos;
    }

    // =========================
    // 회피 버튼 클릭
    // =========================
    public void OnDodgeButton()
    {

        if (!canCheck) return;
        canCheck = false;

        // 회피 판정
        if (!IsPerfectByOverlap())
        {
            PlayerController_wand.Instance.TakeDamage(goodDamage);
            Debug.Log("❌ 회피 실패");
        }
        else
        {
            Debug.Log("✅ 회피 성공");
        }

        // ✅ 회피 미니게임의 역할은 여기까지
        // ✅ 이후 상태 전환은 GameManager가 판단
        GameManager_wand.Instance.EnterMathState();

    }

    // =========================
    // 판정 로직
    // =========================
    bool IsPerfectByOverlap()
    {
        float pointerLeft = pointer.localPosition.x - pointer.rect.width * 0.5f;
        float pointerRight = pointer.localPosition.x + pointer.rect.width * 0.5f;

        float perfectLeft = perfectZone.localPosition.x - perfectZone.rect.width * 0.5f;
        float perfectRight = perfectZone.localPosition.x + perfectZone.rect.width * 0.5f;

        return IsOverlap(pointerLeft, pointerRight, perfectLeft, perfectRight);
    }

    bool IsOverlap(float aLeft, float aRight, float bLeft, float bRight)
    {
        return aLeft <= bRight && aRight >= bLeft;
    }
}

