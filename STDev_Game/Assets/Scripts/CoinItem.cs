using UnityEngine;

public class CoinItem : MonoBehaviour
{
    public int coinIndex;
    public float popForce = 5f;     // 위로 튀어 오르는 힘
    public float destroyTime = 0.5f; // 튀어 오르고 며칠 뒤에(몇 초 뒤에) 사라질지

    public void PopAndDestroy()
    {
        // 1. 충돌체 끄기 (튀어 오르는 동안 캐릭터가 또 먹는 버그 방지)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 2. 물리 엔진(Rigidbody2D) 가져오기 (없으면 달아주기)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();

        // 3. 중력 켜고 위로 뻥! 걷어차기
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.AddForce(Vector2.up * popForce, ForceMode2D.Impulse);

        // 4. 지정된 시간(destroyTime) 뒤에 깔끔하게 삭제
        Destroy(gameObject, destroyTime);
    }
}