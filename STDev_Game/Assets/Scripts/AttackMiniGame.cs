using UnityEngine;
using System.Collections;

public class AttackMiniGame : MonoBehaviour
{
    [Header("UI References")]
    public RectTransform gaugeBar;
    public RectTransform pointer;
    public RectTransform perfectZone;

    [Header("Settings")]
    public float moveSpeed = 600f;
    public int perfectDamage = 20;
    public int goodDamage = 10;

    private float minX;
    private float maxX;
    private bool movingRight;
    private bool canHit;

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
        canHit = true;
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
    // 공격 버튼 클릭
    // =========================
    public void OnAttackButton()
    {

        if (!canHit) return;
        canHit = false;

        if (IsPerfectByOverlap())
            BossController.Instance.TakeDamage(perfectDamage);
        else
            BossController.Instance.TakeDamage(goodDamage);

        // ✅ 판단은 GameManager에게 위임
        GameManager.Instance.EnterMathState();

    }

    // =========================
    // 판정 로직 (겹침 기반)
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





